using RoR2;
using UnityEngine;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine.Networking;
using BayoMod.Modules.Components;

namespace BayoMod.Survivors.Bayo.SkillStates.PunishStates
{
    public class Grab : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq = 0.44f;
        protected float damage = 1.75f;
        protected float dur = 2.64f;
        protected float attackStart = 0;
        protected float attackEnd = 1f;
        private int counter = 8;
        private float curSpeed = 1f;
        private float curAngle = 0f;
        public float y;
        private float modifY = 0;
        private bool voiced = false;

        public CharacterBody enemyBody;
        public Vector3 forwardDir;
        private Vector3 direction;
        public static CharacterCameraParams cameraParams;
        protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        protected ModelLocator component;
        protected ChildLocator component2;
        private Transform modelTrans;
        public override void OnEnter()
        {
            duration = dur;
            attackStartPercentTime = attackStart;
            attackEndPercentTime = attackEnd;
            hitSoundString = "";

            damageCoefficient = damage;
            procCoefficient = 0.5f;
            pushForce = 0f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            exitToStance = false;
            durOverride = true;
            fireAge = 0f;
            damageType = DamageType.NonLethal;

            enemyBody = base.GetComponent<PunishTracker>().GetTrackingTarget().healthComponent.body;

            if (enemyBody && enemyBody.healthComponent && enemyBody.healthComponent.alive)
            {
                modelTrans = enemyBody.modelLocator.modelTransform;
                if (modelTrans)
                {
                    Quaternion quat = characterBody.modelLocator.modelTransform.rotation;
                    quat *= Quaternion.AngleAxis(135f, Vector3.up);
                    modelTrans.rotation = quat * Quaternion.AngleAxis(90f, Vector3.right);

                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    if (this.characterBody.HasBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility)) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
                }
                if (base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
                outer.SetNextStateToMain();
                return;
            }

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "PunishZoomFrFr";
            cameraParams.data.wallCushion = 0.1f;
            if (y >= 1) modifY = (y - 1);

            cameraParams.data.idealLocalCameraPos = new Vector3(0, 0, (-10f - (modifY *2f)));
            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 1.5f);
            }

            PlayAnim();

            base.OnEnter();

            fireFreq /= this.attackSpeedStat;
            characterDirection.forward = forwardDir;

        }

        protected virtual void PlayAnim()
        {
            PlayAnimation("Body", "Grab");
        }

        protected override void FireAttack()
        {
            fireAge += Time.fixedDeltaTime;
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                Util.PlaySound("p4", this.gameObject);
                attack.ResetIgnoredHealthComponents();
                hasFired = false;
            }
            base.FireAttack();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float rotSpeed = 360f / fireFreq;
            rotSpeed *= -1f;
            curAngle += rotSpeed * Time.fixedDeltaTime;
            Quaternion rot = Quaternion.Euler(0, curAngle, 0);

            if (isAuthority && characterDirection) characterDirection.forward = transform.TransformVector(rot * forwardDir);

            Quaternion quat = characterBody.modelLocator.modelTransform.rotation;

            if (enemyBody && ((enemyBody.healthComponent && enemyBody.healthComponent.alive) || (isAuthority && stopwatch >= 2f)))
            {
                if (modelTrans)
                {
                    quat *= Quaternion.AngleAxis(135f, Vector3.up);// * Quaternion.Euler(0, curAngle, 0);
                    modelTrans.rotation = quat * Quaternion.AngleAxis(90f, Vector3.right);

                }

                component = gameObject.GetComponent<ModelLocator>();
                component2 = component.modelTransform.GetComponent<ChildLocator>();
                if ((bool)component2)
                {
                    int childIndex = component2.FindChildIndex("muzrh");
                    Transform transformm = component2.FindChild(childIndex);
                    Vector3 pos = transformm.position;

                    CharacterMotor motor = enemyBody.characterMotor;
                    Rigidbody rigidbody = enemyBody.rigidbody;
                    if (motor)
                    {
                        motor.disableAirControlUntilCollision = true;
                        motor.velocity = Vector3.zero;
                        motor.rootMotion = Vector3.zero;

                        motor.Motor.SetPosition(pos, true);
                    }
                    else if (rigidbody)
                    {
                        rigidbody.velocity = Vector3.zero;
                        pos += Quaternion.AngleAxis(135f, Vector3.up) * characterDirection.forward * y;
                        rigidbody.position = pos;
                    }
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    if (this.characterBody.HasBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility)) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
                }
                if (base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
                outer.SetNextStateToMain();
                return;
            }

            if (inputBank.skill1.justPressed && counter > 0)
            {
                counter--;
                float idealFire = fireFreq / 6;
                float multi = (fireFreq - idealFire) / 8f;
                fireFreq -= multi;
            }

            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    characterMotor.moveDirection = inputBank.moveVector;
                    characterDirection.moveVector = characterMotor.moveDirection;
                }
            }

                /*
                if (Camera)
                {
                    Quaternion rotation2 = Quaternion.AngleAxis(160f, Vector3.up);
                    Vector3 targetAngles = characterDirection.forward;
                    Vector3 targetAngles2 = characterDirection.forward;
                    targetAngles.y = 0f;
                    targetAngles = rotation * targetAngles;
                    //targetAngles2 = rotation2 * targetAngles2;
                    targetAngles.y = lookY;
                    cameraDir = Vector3.Lerp(targetAngles2, targetAngles, Mathf.SmoothStep(0.0f, 1.0f, (stopwatch + oldDur)));
                    ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(cameraDir);
                }
                */

            if(isAuthority && stopwatch >= duration - 0.2f && !voiced)
            {
                voiced = true;
                Util.PlaySound("punishend", this.gameObject);
            }

            if (isAuthority && stopwatch >= duration)
            {
                SetNext();
            }
        }

        protected virtual void SetNext()
        {
            outer.SetNextState(new Throw
            {
                forwardDir = characterDirection.forward,
            });
        }

        /*
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
        */

        public override void OnExit()
        {
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.5f);
            }
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDir);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDir = reader.ReadVector3();
        }
    }
}