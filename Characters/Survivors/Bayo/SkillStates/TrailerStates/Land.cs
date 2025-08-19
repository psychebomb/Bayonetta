using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using BayoMod.Survivors.Bayo;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.TrailerStates
{
    public class Land : BaseSkillState
    {
        public float duration = 1.03f;
        public float animDur = 0.56f;
        private float startSlowdown = 0.09f;
        private bool slowed = false;
        private Animator animator;
        private RootMotionAccumulator rootmotion;
        //private CameraController cam;
        private HitStopCachedState hitStopCachedState;
        public override void OnEnter()
        {
            PlayAnimation("Body", "land", "Roll.playbackRate", animDur);
            rootmotion = GetModelRootMotionAccumulator();
            if(isAuthority)EffectManager.SimpleMuzzleFlash(BayoAssets.land, gameObject, "SwingCenter", true);

            animator = GetModelAnimator();
            Util.PlaySound("batland", this.gameObject);
            //cam = this.gameObject.GetComponent<CameraController>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Vector3 vector = rootmotion.ExtractRootMotion();
            if (isAuthority && characterMotor)
            {
                //if (half) vector *= .5f;
                characterMotor.rootMotion = vector;
            }

            if (fixedAge >= startSlowdown)
            {
                if (!slowed)
                {
                    slowed = true;
                    hitStopCachedState = CreateHitStopCachedState(this.characterMotor, this.animator, "Roll.playbackRate");
                }
                if (this.animator) this.animator.SetFloat("Roll.playbackRate", 0.5f);
            }

            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            //cam.useCamObj = false;
            //cam.useFeetAnim = false;
            //cam.UnsetCam();
            ConsumeHitStopCachedState(hitStopCachedState, this.characterMotor, this.animator);
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.OnExit();
        }
    }
}
