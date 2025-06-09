using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Survivors.Bayo.Components;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BaseEmote : BaseState
    {
        public float animDuration;
        public float cancelDuration;
        public string animString;
        private RootMotionAccumulator rootmotion;
        protected bool cancel;
        protected float stopwatch;
        protected float zoomDur = 1f;
        protected float zoomOutDur = 0.5f;
 
        protected bool jumped;
        protected bool flag1;
        protected bool half = false;
        protected bool zoom;

        protected float x = 0;
        protected float y = -1.5f;
        protected float z = -7f;

        protected CharacterCameraParams cameraParams;
        protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        protected string playbackString = "Emote.playbackRate";
        protected string bodyName = "FullBody, Override";
        protected bool stance = false;
        protected bool canCancel = true;

        private BayoWeaponComponent bwc;
        protected bool hideWeapon = true;
        public override void OnEnter()
        {
            rootmotion = GetModelRootMotionAccumulator();
            flag1 = false;
            characterBody.hideCrosshair = true;
            PlayAnimation(bodyName, animString, playbackString, animDuration);
            base.OnEnter();

            this.characterBody.isSprinting = false;
            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "BreakFirst";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(x, y, z);

            zoom = Modules.Config.eZoom.Value;
            if (base.cameraTargetParams && zoom)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, zoomDur);
            }

            bwc = this.gameObject.GetComponent<BayoWeaponComponent>();
            if(hideWeapon) bwc.currentWeapon = BayoWeaponComponent.WeaponState.Open;

        }

        private void DetermineCancel()
        {
            if (characterMotor)
            {
                if (!characterMotor.isGrounded) cancel = true;
            }
            if (inputBank)
            {
                if (inputBank.skill1.down) cancel = true;
                if (inputBank.skill2.down) cancel = true;
                if (inputBank.skill3.down) cancel = true;
                if (inputBank.skill4.down) cancel = true;
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }

                if (inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }
        public override void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            cancel = false;
            jumped = false;
            if(canCancel) DetermineCancel();
            base.FixedUpdate();

            if (jumped)
            {
                inputBank.jump.PushState(false);
            }

            if (cancel)
            {
                outer.SetNextStateToMain();
                return;
            }

            if (rootmotion && !flag1)
            {
                Vector3 vector = rootmotion.ExtractRootMotion();
                if (isAuthority && characterMotor)
                {
                    //if (half) vector *= .5f;
                    characterMotor.rootMotion = vector;
                }
            }

            if (isAuthority && stopwatch >= animDuration)
            {
                if (stance)
                {
                    outer.SetNextState(new Stance());
                }
                else
                {
                    outer.SetNextStateToMain();
                }
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.hideCrosshair = false;
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow60);
            if(hideWeapon) bwc.currentWeapon = BayoWeaponComponent.WeaponState.Guns;

            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, zoomOutDur);
            }
            PlayAnimation("FullBody, Override", "BufferEmpty");

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}