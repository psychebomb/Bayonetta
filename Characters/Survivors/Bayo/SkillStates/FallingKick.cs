using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Audio;
using EntityStates.Loader;
using EntityStates.Merc;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKick : BaseSkillState
    {
        public static float airControl = GroundSlam.airControl;
        public static float minimumDuration = GroundSlam.minimumDuration;
        public static string enterSoundString = GroundSlam.enterSoundString;
        public static float initialVerticalVelocity = GroundSlam.initialVerticalVelocity;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration *0.8f;
        private float previousAirControl;


        protected string hitboxGroupName = "FallGroup";
        private OverlapAttack fallAttack;
        protected DamageType damageType = DamageType.Generic;
        protected float fallDamage = 3f;
        protected float fireAge = 0f;
        protected float procCoefficient = 1f;
        protected Vector3 downForce =  Vector3.down * Uppercut.upwardForceStrength * 0.8f;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        protected string kickSoundString = GroundLight.comboAttackSoundString;
        protected string hitSoundString = "";
        protected float attackRecoil = 1f;
        protected float hitStopDuration = 0.012f;

        private bool hasFired;
        private float hitPauseTimer;
        protected bool inHitPause;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        protected string playbackRateParam = "Emote.playbackRate";
        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            hasFired = false;
            SetupFallAttack();
            if (isAuthority)
            {
                characterMotor.velocity.y = initialVerticalVelocity;
            }
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = airControl;
            characterDirection.forward = GetAimRay().direction;
            PlayAnimation("Body", "FallKick");
            Util.PlaySound(kickSoundString, gameObject);

        }

        private void SetupFallAttack()
        {
            fallAttack = InitMeleeOverlap(fallDamage, GroundLight.comboHitEffectPrefab, GetModelTransform(), hitboxGroupName);
            fallAttack.damageType = damageType;
            fallAttack.procCoefficient = procCoefficient;
            fallAttack.forceVector = downForce;
            fallAttack.isCrit = RollCrit();
            fallAttack.impactSound = impactSound;
        }
        private void FireFallAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                if (isAuthority)
                {
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }
            if (fireAge >= 0.25f)
            {
                fireAge = 0f;
                fallAttack.ResetIgnoredHealthComponents();
                fallAttack.Fire();
                hasFired = false;
            }
            if (isAuthority && fallAttack.Fire())
            {
                OnHitEnemyAuthority();
            }
        }

        protected bool CanDodge()
        {
            if (inputBank.skill3.down && skillLocator.utility && (!skillLocator.utility.mustKeyPress || !inputBank.skill3.hasPressBeenClaimed) && skillLocator.utility.ExecuteIfReady())
            {
                return true;
            }
            return false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (isAuthority && characterMotor)
            {
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterDirection.forward = GetAimRay().direction;
                characterMotor.velocity.y += verticalAcceleration * Time.fixedDeltaTime;
                if (fixedAge >= minimumDuration && characterMotor.Motor.GroundingStatus.IsStableOnGround)
                {
                    outer.SetNextState(new FallingKickEnd());
                }
            }
            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                fireAge += Time.fixedDeltaTime;
            }
            else
            {
                hitPauseTimer -= Time.fixedDeltaTime;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            FireFallAttack();

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

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
        }

        public override void OnExit()
        {
            characterMotor.airControl = previousAirControl;
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }
    }
}
