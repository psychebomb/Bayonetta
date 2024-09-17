using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class FlurryStart : BaseSkillState
    {

        public static string enterSoundString = PreGroundSlam.enterSoundString;

        protected float duration = 0.4f;

        private RootMotionAccumulator rootMotionAccumulator;


        public override void OnEnter()
        {
            base.OnEnter();
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "FlurryStart", "Slash.playbackRate", duration);
            characterDirection.forward = GetAimRay().direction;
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
                outer.SetNextState(new Dodge { currentSwing = 4 });
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
            if (fixedAge > duration)
            {
                outer.SetNextState(new Flurry());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
