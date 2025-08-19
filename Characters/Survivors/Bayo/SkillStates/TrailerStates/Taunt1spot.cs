using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;
using RoR2.CameraModes;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.TrailerStates
{
    public class Taunt1spot : BaseEmote
    {
        private uint sound;
        private bool voiced = false;

        private float zoomoutTime = 4.88f;
        private bool zoomedOut = false;

        private bool shined = false;
        public GameObject spot = BayoAssets.spotlight2;
        private float freezeTime = 5.28f;
        private float freezeDur = 2.5f;
        private float freezeWatch = 0f;
        private HitStopCachedState poseCachedState;
        public Animator animator;
        private bool freezeDone = false;

        private CameraTargetParams.CameraParamsOverrideHandle cam2;
        private CameraRigController cameraRig;
        public UIController uiController;
        public override void OnEnter()
        {
            animString = "Taunt";
            animDuration = 6.56f;
            y = -2.25f;
            z = -3.5f;
            zoomDur = 0.01f;
            zoomOutDur = 0.25f;
            animator = GetModelAnimator();

            uiController = this.gameObject.GetComponent<UIController>();
            uiController.SetRORUIActiveState(false);
            base.OnEnter();

            if (base.cameraTargetParams)
            {
                if (characterBody.master.playerCharacterMasterController.networkUser)
                {
                    cameraRig = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                }
            }

            cameraParams.name = "zoom2";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(0f, -2.25f, -8f);

            zoom = Modules.Config.eZoom.Value;
            if (base.cameraTargetParams && zoom)
            {
                cam2 = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.01f);
            }
        }

        public override void FixedUpdate()
        {
            if(stopwatch >= zoomoutTime && !zoomedOut)
            {
                zoomedOut = true;
                if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, zoomOutDur);
                }
                EffectManager.SimpleMuzzleFlash(spot, gameObject, "SwingCenter", true);
                //characterBody.AddTimedBuff(BayoBuffs.spotBuff, 2.5f);
            }
            if(stopwatch >= freezeTime && !freezeDone)
            {
                if (!shined)
                {
                    shined = true;
                    poseCachedState = CreateHitStopCachedState(characterMotor, animator, playbackString);
                }
                freezeWatch += Time.fixedDeltaTime;
                stopwatch -= Time.fixedDeltaTime;
                if (animator) animator.SetFloat(playbackString, 0f);

                if (freezeWatch >= freezeDur)
                {
                    freezeDone = true;
                    ConsumeHitStopCachedState(poseCachedState, characterMotor, animator);
                }
            }

            if (cameraRig)
            {
                Quaternion rotation = Quaternion.AngleAxis(180f, Vector3.up);
                Quaternion rotation2 = Quaternion.AngleAxis(270f, Vector3.up);
                Vector3 targetAngles = characterDirection.forward;
                Vector3 targetAngles2 = characterDirection.forward;
                targetAngles = rotation * targetAngles;
                targetAngles2 = rotation2 * targetAngles2;
                targetAngles.y = 0f;
                targetAngles2.y = 0f;
                Vector3 rotateAngle = Vector3.Lerp(targetAngles2, targetAngles, Mathf.SmoothStep(0.0f, 1.0f, stopwatch / zoomoutTime));
                ((CameraModePlayerBasic.InstanceData)cameraRig.cameraMode.camToRawInstanceData[cameraRig]).SetPitchYawFromLookVector(rotateAngle);
            }

            base.FixedUpdate();
        }
        public override void OnExit()
        {
            if (base.cameraTargetParams && cam2.isValid && zoom)
            {
                cam2 = base.cameraTargetParams.RemoveParamsOverride(cam2, zoomOutDur);
            }
            uiController.SetRORUIActiveState(true);
            base.OnExit();
        }

    }
}