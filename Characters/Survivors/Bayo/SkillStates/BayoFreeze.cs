using BayoMod.Survivors.Bayo;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using RoR2.CameraModes;
using RoR2.ConVar;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class BayoFreeze : BaseSkillState
    {
        Animator animator;
        internal float duration;
        internal float previousAttackSpeedStat;
        private Animator modelAnimator;

        public float snap2 = 0.456f;
        private bool snap2ed = false;

        protected CharacterCameraParams cameraParams;
        protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        private CameraRigController Camera;

        private Vector3 lookDir;
        private Vector3 dir1;
        private Vector3 dir2;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelAnimator = base.GetModelAnimator();
            if (this.modelAnimator)
            {
                this.modelAnimator.enabled = false;
            }
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

            lookDir = GetAimRay().direction;
            GetRandomDirections();

            Util.PlaySound("clear", this.gameObject);
            if (NetworkServer.active && characterBody)
            {
                characterBody.AddTimedBuff(BayoBuffs.snapBuff, 0.09f);
                characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, 1.25f);
            }

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "clearzoom";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(0, -2f, -6.5f);

            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0);

                if (characterBody.master.playerCharacterMasterController.networkUser)
                {
                    Camera = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                }
            }
        }

        private void GetRandomDirections()
        {
            List<int> range = new List<int>();
            for(int i= 45; i <= 315; ++i)
            {
                range.Add(i);
            }
            int randRot1 = range[Random.Range(0, range.Count - 1)];
            dir1 = Quaternion.AngleAxis(randRot1, Vector3.up) * lookDir;
            dir1.y = Random.Range(-.5f,.5f);

            List<int> excludedRanges = new List<int>();
            for (int i = randRot1 - 45; i <= randRot1 + 45; ++i)
            {
                excludedRanges.Add(i);
            }

            foreach (var excludedRange in excludedRanges)
            {
                if (range.Contains(excludedRange))
                {
                    range.Remove(excludedRange);
                }
            }

            int randRot2 = range[Random.Range(0, range.Count - 1)];
            dir2 = Quaternion.AngleAxis(randRot2, Vector3.up) * lookDir;
            dir2.y = Random.Range(-.5f, .5f);

        }
        public override void OnExit()
        {
            if (this.modelAnimator)
            {
                this.modelAnimator.enabled = true;
            }
            CharacterModel model = this.GetModelTransform().GetComponent<CharacterModel>();
            if (model)
            {
                model.forceUpdate = true;
            }
            if (NetworkServer.active && characterBody)
            {
                characterBody.AddTimedBuff(BayoBuffs.snapBuff, 0.09f);
            }
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0);
            }
            if (Camera)
            {
                ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(lookDir);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            attackSpeedStat = 0f;


            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            if (characterBody.characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                if(!characterMotor.isGrounded)characterMotor.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y;
            }

            if(base.fixedAge < snap2)
            {
                if (Camera)
                {
                    ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(dir1);
                }
            }
            if (base.fixedAge >= snap2)
            {
                if (Camera)
                {
                    ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(dir2);
                }
                if (NetworkServer.active && characterBody && !snap2ed)
                {
                    snap2ed = true;
                    characterBody.AddTimedBuff(BayoBuffs.snapBuff, 0.09f);
                }

            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            duration = reader.ReadSingle();
        }
    }
}