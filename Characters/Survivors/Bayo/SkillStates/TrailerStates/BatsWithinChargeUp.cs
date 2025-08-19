using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.CameraModes;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.TrailerStates
{
    public class BatsWithinChargeUp : BaseSkillState
    {
        public GameObject batPrefab = BayoAssets.bats;
        public GameObject batsInstance;
        public ShakeEmitter vfxShaker;

        public float duration = 4.2f;
        public float zoominStart = 1f;
        public float zoominDur = 3.2f;
        public float shakeStart = 1.5f;
        public float vfxStart = 3.5f;
        public float soundStart = 1.648f;

        public bool shakeStarted = false;
        public bool zoominStarted = false;
        public bool vfxStarted = false;
        public bool soundStarted = false;
        public float stopwatch = 0f;

        private CharacterModel characterModel;
        private CameraController cam;
        private CameraRigController cameraRig;
        //protected CharacterCameraParams cameraParams;
        //protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterMotor.rootMotion.y += 8f;

            if (base.rigidbody && !base.rigidbody.isKinematic)
            {
                base.rigidbody.velocity = Vector3.zero;
                if (base.rigidbodyMotor)
                {
                    base.rigidbodyMotor.moveVector = Vector3.zero;
                }
            }

            if (characterBody.characterMotor) characterMotor.velocity = Vector3.zero;

            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            this.characterBody.isSprinting = false;

            ChildLocator childLocator = GetModelChildLocator();
            if (childLocator)
            {
                Transform transform = childLocator.FindChild("SwingCenter") ?? base.characterBody.coreTransform;
                if (transform)
                {
                    batsInstance = Object.Instantiate(batPrefab, transform.position, transform.rotation);
                    batsInstance.transform.parent = transform;
                    vfxShaker = batsInstance.transform.Find("shaker").gameObject.GetComponent<ShakeEmitter>();
                }
            }

            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
            }

            /*
            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "BreakFirst";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(0, -2.25f, -6);
            */
            if ((bool)characterModel)
            {
                characterModel.invisibilityCount++;
            }

            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            //cam = this.gameObject.GetComponent<CameraController>();
            //cam.fov = 60f;
            //cam.useCamObj = true;
            //cam.SetCam();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            if (characterBody.characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                if (!characterMotor.isGrounded) characterMotor.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y;
            }

            if (!vfxStarted && stopwatch >= vfxStart)
            {
                vfxStarted = true;
                batsInstance.transform.Find("vfx").gameObject.SetActive(true);
            }

            if (stopwatch >= shakeStart)
            {
                if (!shakeStarted)
                {
                    shakeStarted = true;
                    batsInstance.transform.Find("shaker").gameObject.SetActive(true);
                }
                vfxShaker.wave.amplitude = Mathf.Lerp(0f, 1.15f, (stopwatch - shakeStart) / (duration - shakeStart));
                vfxShaker.wave.frequency = Mathf.Lerp(2.5f, 7.5f, (stopwatch - shakeStart) / (duration - shakeStart));
            }

            if (!soundStarted && stopwatch >= soundStart)
            {
                soundStarted = true;
                Util.PlaySound("bats", this.gameObject);
            }

            if (isAuthority)
            {
                /*
                if(stopwatch >= zoominStart && !zoominStarted)
                {
                    zoominStarted = true;
                    if (base.cameraTargetParams)
                    {
                        cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                        {
                            cameraParamsData = cameraParams.data,
                            priority = 1f
                        }, zoominDur);
                    }
                }
                */
                if(stopwatch >= 0.01f && !zoominStarted)
                {
                    zoominStarted = true;
                    cam = this.gameObject.GetComponent<CameraController>();
                    cam.fov = 60f;
                    cam.useCamObj = true;
                    cam.SetCam();
                }

                if (stopwatch >= duration)
                {
                    outer.SetNextState(new BatsWithin());
                    return;
                }
            }

        }

        public override void OnExit()
        {
            vfxShaker.amplitudeTimeDecay = true;
            vfxShaker.wave.amplitude = 2f;
            if ((bool)characterModel)
            {
                characterModel.invisibilityCount--;
            }
            /*
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.5f);
            }
            */

            if (characterBody.master.playerCharacterMasterController.networkUser)
            {
                cameraRig = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                Vector3 rotateAngle = this.characterDirection.forward * -1;
                ((CameraModePlayerBasic.InstanceData)cameraRig.cameraMode.camToRawInstanceData[cameraRig]).SetPitchYawFromLookVector(rotateAngle);
            }

            base.OnExit();
        }
    }
}
