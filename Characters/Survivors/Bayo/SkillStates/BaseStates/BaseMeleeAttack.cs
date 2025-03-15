using BayoMod.Survivors.Bayo;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Toolbot;
using EntityStates.Wisp1Monster;
using EntityStates.Drone.DroneWeapon;
using RoR2;
using RoR2.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSkillState
    {
        protected string hitboxGroupName = "PunchGroup";

        protected DamageTypeCombo damageType = DamageType.Generic;
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
        protected string hitSoundString = "hit";
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = FireEmbers.hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        public float duration;
        public float playSwing;
        protected bool hasSwung = false;
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
        protected float fireTime = 9999f;
        protected float bulletStopWatch = 0f;
        protected bool launch = false;
        private bool opFired = false;
        protected float juggleHop = 0f;
        protected bool hasJuggled = false;
        protected readonly List<HurtBox> results = new List<HurtBox>();
        protected readonly List<HurtBox> results2 = new List<HurtBox>();
        protected readonly List<HealthComponent> launchList = new List<HealthComponent>();
        protected string voiceString;
        protected bool voice = false;
        protected bool durOverride = false;

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
            fireTime /= attackSpeedStat;
        }

        public override void OnExit()
        {
            if (inHitPause)
            {
                RemoveHitstop();
            }
            results.Clear();
            results2.Clear();
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
                if (attack.Fire(results))
                {
                    OnHitEnemyAuthority();
                    opFired = true;
                    for (int i = 0; i < this.results.Count; i++)
                    {
                        if(!results2.Contains(this.results[i])) results2.Add(this.results[i]);
                    }
                    Chat.AddMessage("number:" + this.results.Count.ToString());
                }
                if (launch && opFired)
                {
                    Chat.AddMessage("number2:" + this.results2.Count.ToString());
                    TeamIndex team = GetTeam();

                    for (int i = 0; i < this.results2.Count; i++)
                    {
                        HealthComponent item = this.results2[i].healthComponent;
                        Chat.AddMessage("number in list:" + (i+1).ToString());
                        if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body")) && item && item.transform)
                        {
                            Chat.AddMessage("attempting force");
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
            if (voice) { Util.PlaySound(voiceString, gameObject); }

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
            playSwing = duration * attackStartPercentTime * .75f;

            if (!hasSwung && stopwatch >= playSwing)
            {
                hasSwung = true;
                Util.PlaySound(swingSoundString, gameObject);
            }
            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                if (!hasFired)
                {
                    EnterAttack();
                }
                FireAttack();
            }

            if (bulletStopWatch >= fireTime)
            {
                bulletStopWatch -= fireTime;
                FireBullet();
            }

            if (stopwatch >= duration && isAuthority && !durOverride)
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

                if (body.characterMotor && !body.characterMotor.isGrounded)
                {
                    if (characterBody.HasBuff(BayoBuffs.wtBuff)) juggleHop /= 3f;
                    body.characterMotor.velocity.x = 0f;
                    body.characterMotor.velocity.z = 0f;
                    item.GetComponent<SetStateOnHurt>()?.SetStun(1f);
                    if (body.HasBuff(BayoBuffs.wtDebuff))
                    {
                        body.characterMotor.velocity.y = 0f;
                    }
                    else
                    {
                        SmallHop(body.characterMotor, juggleHop);
                    }
                }
            }
        }

        private void FireBullet()
        {
            Ray aimRay = shootRay;
            EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, gameObject, gunName, false);
            AddRecoil(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            if (isAuthority)
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
                //bulletAttack.isCrit = Util.CheckRoll(0.5f, base.characterBody.master);
                bulletAttack.isCrit = RollCrit();
                bulletAttack.radius = 0.75f;
                bulletAttack.smartCollision = true;
                bulletAttack.damageType = DamageType.Generic;
                bulletAttack.procCoefficient = 0.5f;
                bulletAttack.damageColorIndex = DamageColorIndex.Void;
                bulletAttack.Fire();
            }
            characterBody.AddSpreadBloom(spreadBloomValue);
            Util.PlaySound("shoot", gameObject);
        }
    }
}