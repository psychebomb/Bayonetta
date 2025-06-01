using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine.Networking;
using RoR2.CameraModes;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class Break : RisingFinisher
    {

        private RootMotionAccumulator rootMotionAccumulator;
        public CameraTargetParams.AimRequest aimRequest;
        public static CharacterCameraParams cameraParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        private CharacterCameraParams BreakFirst;
        private CharacterCameraParams BreakSec;
        public CameraRigController Camera;
        private Vector3 rotateAngle;
        protected float pose = 0.88f;
        protected float poseTimer = 999f;
        private bool snapped = false;
        private bool sounded = false;
        private bool inPose = false;
        private bool posed = false;
        private HitStopCachedState poseCachedState;
        private Vector3 forwardDir;
        private bool flip = false;
        public override void OnEnter()
        {
            dur = 4.04f;
            attackStart = 0f;
            earlyEnd = 2.64f;
            attackEnd = 0.653f;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                base.characterBody.AddBuff(BayoBuffs.armorBuff);
            }
            CreateCameras();

            muzName = "muzrf";
            frTime = 0.15f;
            damage = 1.2f;
            blastDamage = 0.5f;
            blastRadius = 30;
            hgn = "CoverGroup2";
            hbn = "Envelop2";
            effect = null;
            effect2 = null;

            base.OnEnter();

            cameraParams = BreakFirst;
            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.3f);
            }
                exitToStance = true;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (inputBank.jump.down)
            {
                inputBank.jump.PushState(false);
                outer.SetNextStateToMain();
                return;
            }
            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor && !hasEnded)
                {
                    characterMotor.moveDirection = inputBank.moveVector;
                    characterDirection.moveVector = characterMotor.moveDirection;
                }

                if (rootMotionAccumulator && hasEnded)
                {
                    if (isAuthority && characterMotor)
                    {
                        inputBank.moveVector = Vector3.zero;
                        characterMotor.moveDirection = Vector3.zero;
                        characterDirection.forward = forwardDir;
                    }
                    Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                    if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                    {
                        base.characterMotor.rootMotion += vector;
                    }
                }
                else
                {
                    rootMotionAccumulator.accumulatedRootMotion = Vector3.zero;
                }
            }

            if (hasEnded)
            {
                if (Camera)
                {
                    Quaternion rotation = Quaternion.AngleAxis(207.5f, Vector3.up);
                    Vector3 targetAngles = forwardDir;
                    targetAngles.y = 0f;
                    targetAngles = rotation * targetAngles;
                    targetAngles.y = 0.15f;
                    rotateAngle = Vector3.Lerp(forwardDir, targetAngles, (stopwatch - earlyEnd)/ (pose + 0.1f));
                    ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(rotateAngle);
                }
            }
            if(stopwatch >= earlyEnd + pose && !posed)
            {
                ApplyPause();
            }
            if(poseTimer <= 0.5f && !sounded)
            {
                Util.PlaySound("snap", this.gameObject);
                sounded = true;
            }
            if (poseTimer <= 0.46f && !snapped)
            {
                characterBody.AddTimedBuff(BayoBuffs.snapBuff, 0.06f);
                snapped = true;
            }

            if (inPose)
            {
                stopwatch -= Time.fixedDeltaTime;
                poseTimer -= Time.fixedDeltaTime;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }
            if(poseTimer <=0 && inPose)
            {
                ConsumeHitStopCachedState(poseCachedState, characterMotor, animator);
                inPose = false;
                if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.8f);
                }
            }
        }
        protected override void FireAttack()
        {
            if (fireAge >= fireFreq)
            {
                if (!flip)
                {
                    flip = true;
                    muzName = "muzlf";
                    gunName = muzName;
                    if ((bool)component2)
                    {
                        int childIndex = component2.FindChildIndex(muzName);
                        Transform transformm = component2.FindChild(childIndex);
                        int childIndex2 = component2.FindChildIndex(muzName + "f");
                        Transform transformm2 = component2.FindChild(childIndex2);
                        gunRay = new Ray(transform.position, transformm2.position - transformm.position);
                    }

                }
                else
                {
                    flip = false;
                    muzName = "muzrf";
                    gunName = muzName;
                    if ((bool)component2)
                    {
                        int childIndex = component2.FindChildIndex(muzName);
                        Transform transformm = component2.FindChild(childIndex);
                        int childIndex2 = component2.FindChildIndex(muzName + "f");
                        Transform transformm2 = component2.FindChild(childIndex2);
                        gunRay = new Ray(transform.position, transformm2.position - transformm.position);
                    }
                }
            }
            base.FireAttack();
        }

        private void CreateCameras()
        {
            BreakFirst = ScriptableObject.CreateInstance<CharacterCameraParams>();
            BreakFirst.name = "BreakFirst";
            BreakFirst.data.wallCushion = 0.1f;
            BreakFirst.data.idealLocalCameraPos = new Vector3(0f, -1.5f, -7f);

            BreakSec = ScriptableObject.CreateInstance<CharacterCameraParams>();
            BreakSec.name = "BreakSec";
            BreakSec.data.wallCushion = 0.1f;
            BreakSec.data.maxPitch = -15f;
            BreakSec.data.minPitch = -15f;
            BreakSec.data.idealLocalCameraPos = new Vector3(0f, -3.6f, -4.5f);

        }

        private void ApplyPause()
        {
            if (!inPose)
            {
                poseCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                inPose = true;
                posed = true;
                poseTimer = 0.8f;
            }
        }
        protected override void EnterAttack()
        {
            hasFired = true;
            PlaySwingEffect();

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
        }
        protected override void DetermineCancel()
        {

            if (inputBank)
            {
                if (stopwatch>= earlyExitPercentTime + 0.88f)
                {
                    if (inputBank.skill1.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.skill4.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
                if (inputBank.jump.justPressed)
                {
                    cancel = true;
                    jumped = true;
                }
                //if (stopwatch >= exitTime && inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }
        protected override void PlayAnim()
        {
            PlayAnimation("Body", "Break", playbackRateParam, 1.32f);
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
        }

        protected override void FinisherSpecific()
        {

            if (stopwatch >= earlyExitPercentTime && !hasEnded)
            {
                hasEnded = true;
                fireTime = 9999f;
                forwardDir = GetAimRay().direction;
                inputBank.moveVector = Vector3.zero;
                characterMotor.moveDirection = forwardDir;
                characterDirection.moveVector = forwardDir;
                PlayAnimation("Body", "BreakExit", playbackRateParam, duration - earlyExitPercentTime);

                if (base.cameraTargetParams & cameraParamsOverrideHandle.isValid)
                {
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.6f);
                    cameraParams = BreakSec;
                    cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                    {
                        cameraParamsData = cameraParams.data,
                        priority = 1f
                    }, 0.6f);
                    if (characterBody.master.playerCharacterMasterController.networkUser)
                    {
                        Camera = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;

                    }
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
                base.characterBody.RemoveBuff(BayoBuffs.armorBuff);
            }
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle);
            }
            base.OnExit();
        }
    }
}
