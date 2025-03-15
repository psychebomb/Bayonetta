using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class e2temp : BaseState
    {
        public float animDuration;
        public float cancelDuration;
        public string animString;
        private RootMotionAccumulator rootmotion;
        protected bool cancel;
        protected float stopwatch;

        private bool camActiv = false;
        private bool voiced = false;

        protected bool jumped;
        protected bool flag1;
        private bool zoom;

        private CharacterCameraParams cameraParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        public override void OnEnter()
        {
            rootmotion = GetModelRootMotionAccumulator();
            animString = "urhalo";
            animDuration = 1.96f;
            flag1 = false;
            characterBody.hideCrosshair = true;
            PlayAnimation("FullBody, Override", animString, "Emote.playbackRate", animDuration);
            base.OnEnter();

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "BreakFirst";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(1.35f, -1.75f, -5f);

            zoom = Modules.Config.eZoom.Value;
            if (base.cameraTargetParams && zoom)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.01f);
            }

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
            DetermineCancel();
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

            if (!camActiv)
            {
                camActiv = true;
                /*
                if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 1.96f);
                }
                */

                cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
                cameraParams.name = "BreakSec";
                cameraParams.data.wallCushion = 0.1f;
                cameraParams.data.idealLocalCameraPos = new Vector3(-0.7f, -2.75f, -5f);

                zoom = Modules.Config.eZoom.Value;
                if (base.cameraTargetParams && zoom)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                    {
                        cameraParamsData = cameraParams.data,
                        priority = 1.1f
                    }, 1.96f);
                }
            }

            if (rootmotion && !flag1)
            {
                Vector3 vector = rootmotion.ExtractRootMotion();
                if (isAuthority && characterMotor)
                {
                    characterMotor.rootMotion = vector;
                }
            }

            if (fixedAge > 0.33f && !voiced)
            {
                Util.PlaySound("ktaunt", this.gameObject);
                voiced = true;
            }

            if (isAuthority && stopwatch >= animDuration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.hideCrosshair = false;
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow60);

            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.5f);
            }
            PlayAnimation("FullBody, Override", "BufferEmpty");

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}