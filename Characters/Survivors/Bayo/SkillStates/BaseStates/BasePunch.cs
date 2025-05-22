using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using EntityStates.Loader;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BasePunch : BaseMeleeAttack
    {
        public int swingIndex;
        protected string animStart;
        protected string animEnd;
        protected float exitTime;
        protected float holdTime;
        protected float endDuration;
        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private bool hasEnded;
        protected string gunStr;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;
        protected float hopVelocity = 2.5f;
        protected Vector3 forwardDir;
        private Vector3 vfxPos;
        public override void OnEnter()
        {
            damageCoefficient = 2f;
            attackEndPercentTime = 0.6f;
            //damageCoefficient = 3f;  wtf is wrong with me
            procCoefficient = 1f;
            damageType = DamageTypeCombo.GenericPrimary;
            pushForce = 100f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;
            hasEnded = false;
            shootRay = GetAimRay();
            gunName = gunStr;
            gunDamage = 0.5f;
            launch = false;
            fireTime = 0.166f;
            destroyvfx = false;

            forwardDir = GetAimRay().direction;
            characterDirection.forward = forwardDir;
            rootMotionAccumulator = GetModelRootMotionAccumulator();

            base.OnEnter();

            holdTime /= this.attackSpeedStat;
            earlyExitPercentTime /= this.attackSpeedStat;
            endDuration /= this.attackSpeedStat;
            playSwing /= this.attackSpeedStat;
            exitTime = holdTime + earlyExitPercentTime;
            duration = exitTime + endDuration;
            attackStartPercentTime = earlyExitPercentTime /duration;
            PlayAnimation("Body", animStart, playbackRateParam, earlyExitPercentTime * 2);

            if (characterMotor && !characterMotor.isGrounded && hopVelocity > 0f)
            {
                characterMotor.velocity.y = 0f;
                characterMotor.airControl = characterMotor.airControl;
                SmallHop(characterMotor, hopVelocity);
                launch = true;
                juggleHop = 7f / this.attackSpeedStat;
                exitToStance = false;
            }

            if(characterBody && characterBody.isSprinting) characterBody.isSprinting = false;

        }

        protected override void EnterAttack()
        {
            hasFired = true;
            if (voice) { Util.PlaySound(voiceString, gameObject); }

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
                if (hasEnded)
                {
                    if (inputBank.skill2.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.skill4.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
                if (inputBank.jump.justPressed)
                {
                    cancel = true;
                    jumped = true;
                }
                //if (stopwatch >= exitTime && inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }
        public override void FixedUpdate()
        {
            cancel = false;
            jumped = false;
            if (stopwatch >= earlyExitPercentTime)
            {
                DetermineCancel();
                if (jumped)
                {
                    inputBank.jump.PushState(false);
                }

                if (cancel)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
            if (CanDodge())
            {
                cancel = true;
                outer.SetNextState(new Dodge { currentSwing = swingIndex });
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }
            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    inputBank.moveVector = Vector3.zero;
                    characterMotor.moveDirection = Vector3.zero;
                }

                if (rootMotionAccumulator)
                {
                    Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                    if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                    {
                        if (!hasEnded) vector *= 2f;
                        base.characterMotor.rootMotion += vector;
                    }
                }
            }
            else
            {
                rootMotionAccumulator.accumulatedRootMotion = Vector3.zero;
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
            }

            characterDirection.forward = forwardDir;

            shootRay = GetAimRay();

            if (stopwatch >= exitTime)
            {
                if (inputBank.skill1.down)
                {
                    SetStep();
                    return;
                }
                if (!hasEnded)
                {
                    hasEnded = true;
                    fireTime = 9999f;
                    PlayAnimation("Body", animEnd, playbackRateParam, endDuration);
                }
            }

            base.FixedUpdate();


        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
            if (loopEffectInstance)
            {
                if(animStart != "P1") loopEffectInstance.transform.Find("swing1").gameObject.GetComponent<MoveOffset>().atSpeedMult = this.attackSpeedStat;
            }

        }
        public void SetStep()
        {
            switch (swingIndex)
            {
                case 0:
                    outer.SetNextState(new Punch2
                    {
                        swingIndex = 1
                    });
                    break;
                case 1:
                    outer.SetNextState(new Punch3
                    {
                        swingIndex = 2
                    });
                    break;
                case 2:
                    outer.SetNextState(new Punch4
                    {
                        swingIndex = 3
                    });
                    break;
                case 3:
                    outer.SetNextState(new FlurryStart());
                    break;

            }
        }

        public override void OnExit()
        {
            if (swingEffectPrefab) swingEffectPrefab.GetComponentInChildren<ParticleSystem>().Stop();
            base.OnExit();
        }
    }
}
