using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKickStart : BaseSkillState
    {

        public static string enterSoundString = PreGroundSlam.enterSoundString;

        protected float duration = 0.44f;

        private RootMotionAccumulator rootMotionAccumulator;


        public override void OnEnter()
        {
            base.OnEnter();
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "FallKickStart", "Slash.playbackRate", duration);
            Util.PlaySound(enterSoundString, gameObject);
            characterMotor.Motor.ForceUnground();
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.velocity.y = 0f;
            characterDirection.forward = GetAimRay().direction;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = GetAimRay().direction;
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
                outer.SetNextState(new FallingKick());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
