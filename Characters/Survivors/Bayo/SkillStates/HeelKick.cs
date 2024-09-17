using RoR2.Audio;
using EntityStates.Merc;
using EntityStates;
using RoR2;
using UnityEngine;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class HeelKick : BaseSkillState
    {
        protected string hitboxGroupName = "CoverGroup";

        protected DamageType damageType = DamageType.Stun1s;
        public static float duration = 0.35f;
        protected float attackStartTime = 0.1f;
        protected float attackEndTime = 0.3f;
        protected float earlyEnd = 0.15f;

        protected float damageCoefficient = 3f;
        protected float procCoefficient = 1f;
        protected float hitStopDuration = 0.05f;
        protected float attackRecoil = 1f;
        protected float hitHopVelocity = 4f;
        protected Vector3 upForce = 0.6f * Vector3.up * Uppercut.upwardForceStrength;

        protected string kickSoundString = GroundLight.comboAttackSoundString;
        protected string hitSoundString = "";
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Emote.playbackRate";
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        protected RootMotionAccumulator rootMotionAccumulator;
        protected bool cancel;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            hasFired = false;
            StartAimMode(0.5f + duration, false);

            attack = InitMeleeOverlap(damageCoefficient, GroundLight.comboHitEffectPrefab, GetModelTransform(), hitboxGroupName);

            attack.damageType = damageType;
            attack.procCoefficient = procCoefficient;
            attack.forceVector = upForce;
            attack.impactSound = impactSound;

            Vector3 b = characterMotor ? characterMotor.velocity : Vector3.zero;
            //PlayCrossfade("FullBody, Override", "HeelKick", playbackRateParam, duration, 0.05f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            cancel = false;

            if (inputBank.skill3.justPressed)
            {
                outer.SetNextState(new Dodge());
                return;
            }

            if (rootMotionAccumulator)
            {
                Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                {
                    base.characterMotor.rootMotion += vector;
                }
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
                hitPauseTimer -= Time.fixedDeltaTime;
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            if (stopwatch >= attackStartTime && stopwatch <= attackEndTime)
            {
                FireAttack();
            }

            if (stopwatch >= earlyEnd && inputBank.jump.justPressed)
            {
                outer.SetNextStateToMain();
                cancel = true;
                return;
            }

            if (isAuthority && stopwatch >= duration)
            {
                outer.SetNextState(new Stance());
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
                if (isAuthority)
                {
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
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
            if (cancel) PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
