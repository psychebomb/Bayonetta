using EntityStates;
using RoR2;
using UnityEngine;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKickStart : BaseSkillState
    {

        public static string enterSoundString = "fallslide";

        protected float duration = 0.44f;

        private RootMotionAccumulator rootMotionAccumulator;

        //private GameObject swingEffectPrefab = BayoAssets.fallk;


        public override void OnEnter()
        {
            base.OnEnter();
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "FallKickStart", "Slash.playbackRate", duration);
            Util.PlaySound(enterSoundString, gameObject);
            Util.PlaySound("fallstart", gameObject);
            characterMotor.Motor.ForceUnground();
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.velocity.y = 0f;
            characterDirection.forward = GetAimRay().direction;
            //EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, "SwingCenter", true);
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
