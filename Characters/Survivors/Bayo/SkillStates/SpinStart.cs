using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class SpinStart : BaseSkillState
    {


        protected float duration = 0.16f;

        private RootMotionAccumulator rootMotionAccumulator;

        protected string sound = "flurspin";


        public override void OnEnter()
        {
            base.OnEnter();
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "SpinStart", "Slash.playbackRate", duration);
            Util.PlaySound(sound, gameObject);
            characterDirection.forward = GetAimRay().direction;
            characterMotor.velocity.y = 0f;
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
            characterDirection.forward = GetAimRay().direction;

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            characterMotor.velocity.y = 0f;

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

            if (fixedAge >= duration)
            {
                NextState();
            }
        }

        protected virtual void NextState()
        {
            outer.SetNextState(new Spin());
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}