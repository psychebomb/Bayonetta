using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Modules.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;

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
        public override void OnEnter()
        {
            exitTime = holdTime + earlyExitPercentTime;
            duration = exitTime + endDuration;
            attackStartPercentTime = 0.125f;
            attackEndPercentTime = 0.6f;
            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;
            pushForce = 300f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;
            hasEnded = false;

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", animStart);

            base.OnEnter();

        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
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

            if (rootMotionAccumulator)
            {
                Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                {
                    base.characterMotor.rootMotion += vector;
                }
            }

            if (base.characterMotor.isGrounded)
            {
                characterMotor.velocity.y = Mathf.Lerp(0f, -1f, fixedAge / duration);
            }

            if(stopwatch >= exitTime)
            {
                if (inputBank.skill1.down)
                {
                    SetStep();
                }
                if (!hasEnded)
                {
                    hasEnded = true;
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
