using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Modules.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using EntityStates.Loader;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BasePunch : BaseMeleeAttack
    {
        

        protected string animStart;
        protected string animEnd;
        protected float exitTime;
        protected float holdTime;
        protected float endDuration;
        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private bool hasEnded;
        private Vector3 moveVect;
        private Vector3 aimDir;
        private Vector3 normalized;
        private float direction;
        protected string gunStr;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;
        protected float hopVelocity = 2.5f;
        public override void OnEnter()
        {
            exitTime = holdTime + earlyExitPercentTime;
            duration = exitTime + endDuration;
            damageCoefficient = 1.5f;
            attackStartPercentTime = 0.125f;
            attackEndPercentTime = 0.6f;
            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;
            pushForce = 100f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;
            hasEnded = false;
            shootRay = GetAimRay();
            gunName = gunStr;
            gunDamage = 0.5f;
            fireTime = 0.166f;
            launch = false;

            characterDirection.forward = GetAimRay().direction;
            if (characterMotor && !characterMotor.isGrounded && hopVelocity > 0f)
            {
                characterMotor.velocity.y = 0f;
                characterMotor.airControl = characterMotor.airControl;
                SmallHop(characterMotor, hopVelocity);
                launch = true;
                juggleHop = 7f;
            }
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", animStart);

            base.OnEnter();

        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
                if (stopwatch >= exitTime + 0.012)
                {
                    if (inputBank.skill2.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.skill4.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }
                //if (stopwatch >= exitTime && inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }

        private bool HoldingBack()
        {
            moveVect = base.inputBank.moveVector;
            aimDir = base.inputBank.aimDirection;
            normalized = new Vector3(aimDir.x, 0f, aimDir.z).normalized;
            direction = Vector3.Dot(moveVect, normalized);
            if (characterMotor && characterMotor.isGrounded)
            {
                if (direction < -0.5f)
                {
                    return true;
                }
            }
            return false;
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

            shootRay = GetAimRay();

            if (stopwatch >= exitTime)
            {
                if (inputBank.skill1.down)
                {
                    SetStep();
                }
                if (!hasEnded)
                {
                    hasEnded = true;
                    fireTime = 100f;
                    PlayAnimation("Body", animEnd);
                }
            }

            base.FixedUpdate();


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
    }
}
