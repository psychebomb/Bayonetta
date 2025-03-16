using RoR2;
using UnityEngine;
using EntityStates.Loader;
using RoR2.CameraModes;
using BayoMod.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine.Networking;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Modules.Components;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class StepStart : BaseMeleeAttack
    {

        public static string enterSoundString = PreGroundSlam.enterSoundString;

        protected float distanceMult = 3f;

        private RootMotionAccumulator rootMotionAccumulator;

        protected string animName = "StepStart";

        protected float dur = 0.68f;

        protected float atStart = 0.647f;

        protected float stunTime = 4.8f;

        public CharacterBody enemyBody;

        public static CharacterCameraParams cameraParams;

        protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        private CameraRigController Camera;

        protected Vector3 rotateAngle;

        private Transform modelTrans = null;

        public float lookY;

        protected Quaternion rotation = Quaternion.AngleAxis(120f, Vector3.up);

        protected float x = -1;
        protected float y = -2.25f;
        protected float z = -5;
        protected bool strongModif = false;


        public override void OnEnter()
        {
            duration = dur;
            attackStartPercentTime = atStart;
            attackEndPercentTime = 1f;
            damageCoefficient = 1.25f;
            hitStopDuration = 0.012f;
            durOverride = true;
            exitToStance = false;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            damageType = DamageType.NonLethal;

            enemyBody = base.GetComponent<PunishTracker>().GetTrackingTarget().healthComponent.body;

            if (enemyBody && enemyBody.healthComponent && enemyBody.healthComponent.alive)
            {
                CharacterMotor motor = enemyBody.characterMotor;
                if (motor)
                {
                    motor.disableAirControlUntilCollision = true;
                    motor.velocity = Vector3.zero;
                    motor.rootMotion = Vector3.zero;
                    Vector3 pos = characterDirection.forward * (enemyBody.GetComponent<CapsuleCollider>().bounds.extents.x * 1.25f + 1);
                    Vector3 modif = Quaternion.AngleAxis(67.5f, Vector3.up) * characterDirection.forward * enemyBody.GetComponent<CapsuleCollider>().bounds.extents.y;
                    if (strongModif) modif *= 1.5f;

                    motor.Motor.SetPosition(characterBody.transform.position + (pos - modif), true);
                }
                modelTrans = enemyBody.modelLocator.modelTransform;
                if (modelTrans)
                {
                    Quaternion quat = characterBody.modelLocator.modelTransform.rotation;
                    quat *= Quaternion.AngleAxis(67.5f, Vector3.up);
                    modelTrans.rotation = quat * Quaternion.AngleAxis(90f, Vector3.right);

                }
            }
            else
            {
                if (base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
                outer.SetNextStateToMain();
                return;
            }

            Util.PlaySound("pstart", this.gameObject);

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "PunishZoom";
            cameraParams.data.wallCushion = 0.1f;

            if(enemyBody.GetComponent<CapsuleCollider>() != null)
            {
                x -= enemyBody.GetComponent<CapsuleCollider>().bounds.extents.x * 0.8f;
                //y += enemyBody.GetComponent<CapsuleCollider>().bounds.extents.x * 0.25f;
                z -= enemyBody.GetComponent<CapsuleCollider>().bounds.extents.y * 0.8f;
                lookY = enemyBody.GetComponent<CapsuleCollider>().bounds.extents.y * -0.02f;
            }

            cameraParams.data.idealLocalCameraPos = new Vector3(x, y, z);
            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.5f);
                if (characterBody.master.playerCharacterMasterController.networkUser)
                {
                    Camera = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;

                }
            }

            if (NetworkServer.active)
            {
                enemyBody.AddTimedBuff(BayoBuffs.punishable, stunTime);
                this.characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, stunTime);
                enemyBody.healthComponent.GetComponent<SetStateOnHurt>()?.SetStun(stunTime);
            }

            PlayAnimation("Body", animName);

            base.OnEnter();

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Camera)
            {
                Quaternion rotation2 = Quaternion.AngleAxis(165f, Vector3.up);
                Vector3 targetAngles = characterDirection.forward;
                Vector3 targetAngles2 = characterDirection.forward;
                targetAngles.y = 0f;
                targetAngles = rotation * targetAngles;
                targetAngles2 = rotation2 * targetAngles2;
                targetAngles.y = lookY;
                rotateAngle = Vector3.Lerp(targetAngles2, targetAngles, Mathf.SmoothStep(0.0f, 1.0f, stopwatch));
                ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(rotateAngle);
            }

            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    inputBank.moveVector = Vector3.zero;
                    characterMotor.moveDirection = Vector3.zero;
                }

                if (rootMotionAccumulator)
                {
                    Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                    if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                    {
                        base.characterMotor.rootMotion += vector;
                    }
                }
            }

            if (isAuthority && stopwatch >= duration)
            {
                SetNext();
            }
        }

        protected virtual void SetNext()
        {
            outer.SetNextState(new Step
            {
                cameraDir = rotateAngle,
                cameraParamsOverrideHandle = cameraParamsOverrideHandle,
                lookY = lookY,
                oldDur = duration
            });
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(rotateAngle);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            rotateAngle = reader.ReadVector3();
        }
        protected override void EnterAttack()
        {
            hasFired = true;
            PlaySwingEffect();
            if (voice) { Util.PlaySound(voiceString, gameObject); }

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
