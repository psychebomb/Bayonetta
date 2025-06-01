using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo.Components;
using RoR2;
using UnityEngine;
using RoR2.CameraModes;
using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Survivors.Bayo;
using EntityStates;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Kiss : BaseEmote
    {
        private bool voiced = false;
        private BayoWeaponComponent bwc;
        private CameraRigController cameraRig;

        private float kissDone = 1.2f;
        private bool zoomed = false;
        private CameraTargetParams.CameraParamsOverrideHandle cam2;
        public override void OnEnter()
        {
            animString = "Mwah";
            animDuration = 1.88f;
            zoomDur = 0f;
            zoomOutDur = 1.5f;
            bodyName = "Body";
            stance = true;
            canCancel = false;

            y = -1.5f;
            z = -2.25f;

            if (base.cameraTargetParams)
            {
                if (characterBody.master.playerCharacterMasterController.networkUser)
                {
                    cameraRig = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                }
            }

            if (NetworkServer.active && characterBody)
            {
                characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, animDuration + 0.3f);
            }

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "zoom2";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(0.375f, y, z);

            zoom = Modules.Config.eZoom.Value;
            if (base.cameraTargetParams && zoom)
            {
                cam2 = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, kissDone);
            }

            GameObject kissVfx = BayoAssets.hearts;

            if (kissVfx && isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(kissVfx, gameObject, "SwingCenter", true);
            }

            bwc = this.gameObject.GetComponent<BayoWeaponComponent>();
            bwc.currentWeapon = BayoWeaponComponent.WeaponState.Open;

            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > 0.4f && !voiced)
            {
                Util.PlaySound("peck", this.gameObject);
                voiced = true;
            }

            if (fixedAge > kissDone && !zoomed)
            {
                zoomed = true;
                if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, zoomOutDur);
                }
                if (base.cameraTargetParams && cam2.isValid && zoom)
                {
                    cam2 = base.cameraTargetParams.RemoveParamsOverride(cam2, zoomOutDur);
                }
            }

            if (cameraRig)
            {
                Quaternion rotation = Quaternion.AngleAxis(-140f, Vector3.up);
                Quaternion rotation2 = Quaternion.AngleAxis(-165f, Vector3.up);
                Vector3 targetAngles = characterDirection.forward;
                Vector3 targetAngles2 = characterDirection.forward;
                targetAngles = rotation * targetAngles;
                targetAngles2 = rotation2 * targetAngles2;
                targetAngles.y = 0.1f;
                targetAngles2.y = 0.2f;
                Vector3 rotateAngle = Vector3.Lerp(targetAngles2, targetAngles, Mathf.SmoothStep(0.0f, 1.0f, stopwatch / kissDone));
                ((CameraModePlayerBasic.InstanceData)cameraRig.cameraMode.camToRawInstanceData[cameraRig]).SetPitchYawFromLookVector(rotateAngle);
            }
        }

        public override void OnExit()
        {
            bwc.currentWeapon = BayoWeaponComponent.WeaponState.Guns;
            if (base.cameraTargetParams && cam2.isValid && zoom)
            {
                cam2 = base.cameraTargetParams.RemoveParamsOverride(cam2, zoomOutDur);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }


    }
}
