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

        private GameObject swingEffectPrefab = BayoAssets.fallk;

        private GameObject loopEffectInstance;


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
            ChildLocator childLocator = GetModelChildLocator();
            if (childLocator)
            {
                Transform transform = childLocator.FindChild("SwingCenter") ?? base.characterBody.coreTransform;
                Quaternion rot = transform.rotation;
                if (transform)
                {
                    loopEffectInstance = Object.Instantiate(swingEffectPrefab, transform.position, rot);
                    //EffectManager.SpawnEffect(loopEffectPrefab, new EffectData
                    //{
                    //    origin = transform.position,
                    //    rotation = transform.rotation
                    //}, true);
                    loopEffectInstance.transform.parent = transform;
                }
            }
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
            loopEffectInstance.transform.parent = null;
            base.OnExit();
        }
    }
}
