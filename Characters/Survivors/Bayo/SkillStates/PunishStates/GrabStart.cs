using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2.CameraModes;
using BayoMod.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine.Networking;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using UnityEngine.UIElements;
using System;
using System.ComponentModel;
using Rewired;
using BayoMod.Modules.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class GrabStart : BaseMeleeAttack
    {

        
        protected float distanceMult = 3f;
        private RootMotionAccumulator rootMotionAccumulator;
        protected string animName = "GrabStart";
        protected float dur = 0.96f;
        protected float stunTime = 4.28f;
        public CharacterBody enemyBody;
        public static CharacterCameraParams cameraParams;
        protected CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        protected ModelLocator component;
        protected ChildLocator component2;
        protected Quaternion rotateAngle;
        private Transform modelTrans;
        public float lookY;
        private Vector3 direction;
        private Vector3 forwardDir;
        private float y = 0;

        private float firstSpin = .44f;
        private float secSpin = .7f;


        public override void OnEnter()
        {
            duration = dur;
            attackStartPercentTime = 1.1f;
            attackEndPercentTime = 1f;
            durOverride = true;
            exitToStance = false;
            rootMotionAccumulator = GetModelRootMotionAccumulator();

            enemyBody = base.GetComponent<PunishTracker>().GetTrackingTarget().healthComponent.body;

            if (enemyBody && enemyBody.healthComponent && enemyBody.healthComponent.alive)
            {
                if (enemyBody.GetComponent<CapsuleCollider>())
                {
                    y = enemyBody.GetComponent<CapsuleCollider>().bounds.extents.y;
                    enemyBody.GetComponent<CapsuleCollider>().enabled = false;
                }
                else if (enemyBody.GetComponent<SphereCollider>())
                {
                    y = enemyBody.GetComponent<SphereCollider>().radius;
                    enemyBody.GetComponent<SphereCollider>().enabled = false;
                }


                modelTrans = enemyBody.modelLocator.modelTransform;
                if (modelTrans)
                {
                    Quaternion quat = characterBody.modelLocator.modelTransform.rotation;
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
            //Util.PlaySound("pstart", this.gameObject);

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "PunishZoomFr";
            cameraParams.data.wallCushion = 0.1f;

            cameraParams.data.idealLocalCameraPos = new Vector3(0, -2.25f, -6f);
            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.8f);
            }

            if (NetworkServer.active)
            {
                enemyBody.AddTimedBuff(BayoBuffs.punishable, stunTime);
                this.characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, stunTime);
                enemyBody.healthComponent.GetComponent<SetStateOnHurt>()?.SetStun(stunTime);
            }

            if (characterDirection)
            {
                forwardDir = characterDirection.forward;
                forwardDir.y = 0f;
            }

            PlayAnimation("Body", animName);
            Util.PlaySound("grabstart", this.gameObject);

            base.OnEnter();

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

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
            if (characterDirection)
            {
                characterDirection.forward = forwardDir;
            }

            component = gameObject.GetComponent<ModelLocator>();
            component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex("muzrh");
                Transform transformm = component2.FindChild(childIndex);
                Vector3 pos = transformm.position;

                if (enemyBody && enemyBody.healthComponent && enemyBody.healthComponent.alive)
                {
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
                        pos += rotateAngle * Vector3.zero * y;
                        rigidbody.position = pos;
                    }
                }

                if (modelTrans)
                {
                    Quaternion rotation1 = Quaternion.AngleAxis(-112.5f, Vector3.up);
                    Quaternion rotation2 = Quaternion.AngleAxis(-225f, Vector3.up);
                    if (stopwatch >= secSpin)
                    {
                        Quaternion quattarget = rotation2 * characterBody.modelLocator.modelTransform.rotation;
                        rotateAngle = Quaternion.Lerp(rotation1 * characterBody.modelLocator.modelTransform.rotation, quattarget, (stopwatch - secSpin) / (duration - secSpin));
                    }
                    else if (stopwatch >= firstSpin)
                    {
                        Quaternion quattarget = rotation1 * characterBody.modelLocator.modelTransform.rotation;
                        rotateAngle = Quaternion.Lerp(characterBody.modelLocator.modelTransform.rotation, quattarget, (stopwatch - firstSpin) / (secSpin - firstSpin));
                    }
                    else
                    {
                        rotateAngle = characterBody.modelLocator.modelTransform.rotation;
                    }
                    modelTrans.rotation = rotateAngle * Quaternion.AngleAxis(90f, Vector3.right);

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
            }           

            if (isAuthority && stopwatch >= duration)
            {
                SetNext();
            }
        }

        protected virtual void SetNext()
        {
            outer.SetNextState(new Grab
            {
                forwardDir = characterDirection.forward,
                y = y
            });
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
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 1.5f);
            }
            base.OnExit();
        }
    }
}