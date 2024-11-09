using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Toolbot;
using EntityStates.Wisp1Monster;
using EntityStates.Drone.DroneWeapon;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace BayoMod.Modules.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSkillState
    {
        public int swingIndex;

        protected string hitboxGroupName = "PunchGroup";

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 0f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;

        protected float attackStartPercentTime = 0.2f;
        protected float attackEndPercentTime = 0.4f;

        protected float earlyExitPercentTime = 0.4f;

        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 4f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = FireEmbers.hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        public float duration;
        protected bool hasFired;
        private float hitPauseTimer;
        protected OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        protected Vector3 storedVelocity;
        protected bool exitToStance;

        protected Ray shootRay;
        protected string gunName;
        private float recoilAmplitude = FirePistol2.recoilAmplitude;
        protected float gunDamage;
        protected float gunForce = BaseNailgunState.force;
        private GameObject tracerEffectPrefab = FirePistol2.tracerEffectPrefab;
        private GameObject gunEffectPrefab = FireTurret.hitEffectPrefab;
        protected float spreadBloomValue = 0f;
        protected float fireTime = 100f;
        protected float bulletStopWatch = 0f;
        protected bool launch = false;
        protected float juggleHop = 0f;
        protected bool hasJuggled = false;
        protected readonly List<HealthComponent> launchList = new List<HealthComponent>();

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);
            inHitPause = false;

            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = bonusForce;
            attack.pushAwayForce = pushForce;
            attack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
        }

        public override void OnExit()
        {
            if (inHitPause)
            {
                RemoveHitstop();
            }
            launchList.Clear();
            base.OnExit();
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, false);
        }

        protected bool CanDodge()
        {
            if (inputBank.skill3.down && skillLocator.utility && (!skillLocator.utility.mustKeyPress || !inputBank.skill3.hasPressBeenClaimed) && skillLocator.utility.ExecuteIfReady())
            {
                return true;
            }
            return false;
        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            if (!hasHopped)
            {
                if (characterMotor && !characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            ApplyHitstop();
        }

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration;
                inHitPause = true;
            }
        }

        protected virtual void FireAttack()
        {
            if (isAuthority)
            {
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
                if (NetworkServer.active && launch)
                {
                    int num = attack.ignoredHealthComponentList.Count;
                    TeamIndex team = GetTeam();

                    for (int i = 0; i < num; ++i)
                    {
                        HealthComponent item = attack.ignoredHealthComponentList[i];
                        if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))) && item && item.transform)
                        {
                            ApplyForce(item);
                        }
                    }
                }
            }
        }

        protected virtual void EnterAttack()
        {
            hasFired = true;
            characterDirection.forward = GetAimRay().direction;
            PlaySwingEffect();

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            hitPauseTimer -= Time.fixedDeltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            bulletStopWatch += Time.fixedDeltaTime;

            bool fireStarted = stopwatch >= duration * attackStartPercentTime;
            bool fireEnded = stopwatch >= duration * attackEndPercentTime;

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                if (!hasFired)
                {
                    EnterAttack();
                }
                FireAttack();
            }

            if(bulletStopWatch >= fireTime)
            {
                bulletStopWatch -= fireTime;
                FireBullet();
            }

            if (stopwatch >= duration && isAuthority)
            {
                if (exitToStance) { outer.SetNextState(new Stance()); }
                else { outer.SetNextStateToMain(); }
                return;
            }
        }

        protected virtual void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        protected virtual void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            if (!launchList.Contains(item))
            {
                launchList.Add(item);
                //Chat.AddMessage("juggled");
                if (body.characterMotor && !body.characterMotor.isGrounded)
                {
                    if (base.characterBody.HasBuff(BayoBuffs.wtBuff)) juggleHop /= 3f;
                    SmallHop(body.characterMotor, juggleHop);
                    body.characterMotor.velocity.x = 0f;
                    body.characterMotor.velocity.z = 0f;
                    item.GetComponent<SetStateOnHurt>()?.SetStun(1f);
                }
            }
        }

        private void FireBullet()
        {
            Ray aimRay = shootRay;
            EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, gameObject, gunName, false);
            AddRecoil(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                bulletAttack.owner = gameObject;
                bulletAttack.weapon = gameObject;
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
                bulletAttack.minSpread = 0f;
                bulletAttack.maxSpread = 0f;
                bulletAttack.bulletCount = 1;
                bulletAttack.damage = gunDamage * damageStat;
                bulletAttack.force = gunForce;
                bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
                bulletAttack.muzzleName = gunName;
                bulletAttack.hitEffectPrefab = gunEffectPrefab;
                bulletAttack.isCrit = Util.CheckRoll(0.5f, base.characterBody.master);
                bulletAttack.radius = 0.75f;
                bulletAttack.smartCollision = true;
                bulletAttack.damageType = DamageType.Generic;
                bulletAttack.Fire();
            }
            base.characterBody.AddSpreadBloom(spreadBloomValue);
            Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadInt32();
        }
    }
}