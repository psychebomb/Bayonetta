using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Audio;
using EntityStates.Merc;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class RisingFinisher : BaseSkillState
    {
        protected string hitboxGroupName = "CoverGroup";

        protected DamageType damageType = DamageType.Stun1s;
        public static float duration = 1f;
        protected float attackStartTime = 0.1f;
        protected float attackEndTime = 0.9f;

        protected float damageCoefficient = 1f;
        protected float procCoefficient = 0.5f;
        protected float pushForce = 300f;
        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 1f;
        protected float hitHopVelocity = 4f;
        protected float fireAge = 0f;

        protected string kickSoundString = GroundLight.comboAttackSoundString;
        protected string hitSoundString = "";
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            hasFired = false;
            StartAimMode(0.5f + duration, false);

            attack = InitMeleeOverlap(damageCoefficient, GroundLight.comboHitEffectPrefab, GetModelTransform(), hitboxGroupName);

            attack.damageType = damageType;
            attack.procCoefficient = procCoefficient;
            attack.forceVector = Vector3.zero;
            attack.impactSound = impactSound;

            PlayAnimation("Body", "BackKickExit", playbackRateParam, attackStartTime);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (inputBank.skill3.justPressed)
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
                fireAge += Time.fixedDeltaTime;
            }
            else
            {
                hitPauseTimer -= Time.fixedDeltaTime;
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            if (stopwatch >= attackStartTime && stopwatch <= attackEndTime)
            {
                FireAttack();
            }

            if (isAuthority && stopwatch >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
        }

        private void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                Util.PlaySound(kickSoundString, gameObject);
                PlayAnimation("Body", "BackKickExit", playbackRateParam, 0.15f);
                if (isAuthority)
                {
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }
            if (fireAge >= 0.15f)
            {
                fireAge = 0f;
                attack.ResetIgnoredHealthComponents();
                attack.Fire();
                hasFired = false;
            }
            if (isAuthority && attack.Fire())
            {
                OnHitEnemyAuthority();
            }
        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            ApplyHitstop();
        }

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }
        public override void OnExit()
        {
            if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
            if (inHitPause) { RemoveHitstop(); }
            PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

    }
}