using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
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
        protected GameObject hitEffectPrefab;
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

            if (isAuthority && characterMotor)
            {
                inputBank.moveVector = Vector3.zero;
                characterMotor.moveDirection = Vector3.zero;
            }

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
        /*
        private void FireBullet()
        {
            Ray aimRay = shootRay;
            string muzzleName;
            if ((bool)effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, FindModelChild(gunName), gunName, transmit: false);
            }
            AddRecoil(-0.8f * recoilAmplitude, -1f * recoilAmplitude, -0.1f * recoilAmplitude, 0.15f * recoilAmplitude);
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                bulletAttack.owner = FindModelChild(gunName);
                bulletAttack.weapon = FindModelChild(gunName);
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
                bulletAttack.minSpread = minSpread;
                bulletAttack.maxSpread = maxSpread;
                bulletAttack.bulletCount = 1u;
                bulletAttack.damage = bulletDamage * damageStat;
                bulletAttack.force = force;
                bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
                bulletAttack.muzzleName = muzzleName;
                bulletAttack.hitEffectPrefab = hitEffectPrefab;
                bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack.radius = bulletRadius;
                bulletAttack.smartCollision = true;
                bulletAttack.damageType = DamageType.Generic;
                bulletAttack.Fire();
            }
            base.characterBody.AddSpreadBloom(spreadBloomValue);
            Util.PlaySound(fireBarrageSoundString, base.gameObject);
        }
        */

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