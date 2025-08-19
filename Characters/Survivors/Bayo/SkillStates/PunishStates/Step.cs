using RoR2;
using UnityEngine;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine.Networking;
using RoR2.CameraModes;
using BayoMod.Modules.Components;

namespace BayoMod.Survivors.Bayo.SkillStates.PunishStates
{
    public class Step : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq = 0.44f;
        protected float damage = 1.75f;
        protected float dur = 2.2f;
        protected float attackStart = 0.1286f;
        protected float attackEnd = 1f;
        private int counter = 8;
        private float curSpeed = 1f;

        public CharacterBody enemyBody;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public Vector3 forwardDir;
        public Vector3 cameraDir;
        public float lookY;
        public float oldDur;
        private CameraRigController Camera;
        protected Quaternion rotation = Quaternion.AngleAxis(120f, Vector3.up);
        protected Quaternion rotation2 = Quaternion.AngleAxis(165f, Vector3.up);
        protected float zoomOut = 1f;
        public override void OnEnter()
        {
            duration = dur;
            attackStartPercentTime = attackStart;
            attackEndPercentTime = attackEnd;

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

            if (base.cameraTargetParams && characterBody.master.playerCharacterMasterController.networkUser)
            {
                Camera = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;

            }

            PlayAnim();

            forwardDir = characterDirection.forward;

            inputBank.moveVector = Vector3.zero;
            characterMotor.moveDirection = forwardDir;
            characterDirection.moveVector = forwardDir;

            base.OnEnter();

            fireFreq /= this.attackSpeedStat;

        }

        protected virtual void PlayAnim()
        {
            PlayAnimation("Body", "Step", "Slash.playbackRate", fireFreq);
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
        }

        protected override void FireAttack()
        {
            fireAge += Time.fixedDeltaTime;
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                attack.ResetIgnoredHealthComponents();
                hasFired = false;
            }

            if (inputBank.skill1.justPressed && counter > 0)
            {
                counter--;
                float idealFire = fireFreq / 6;
                float multi = (fireFreq - idealFire) / 8f;
                fireFreq -= multi;
                float animSpeed = 1 * this.attackSpeedStat;
                float idealSpeed = animSpeed * 3f;
                multi = (idealSpeed - animSpeed) / 8f;
                curSpeed += multi;
                if (animator) animator.SetFloat("Slash.playbackRate", curSpeed);
            }
            base.FireAttack();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (characterDirection) characterDirection.forward = forwardDir;
            if (inputBank) inputBank.moveVector = Vector3.zero;
            if (characterMotor) characterMotor.moveDirection = Vector3.zero;

            if (Camera)
            {
                Vector3 targetAngles = characterDirection.forward;
                Vector3 targetAngles2 = characterDirection.forward;
                targetAngles.y = 0f;
                targetAngles = rotation * targetAngles;
                targetAngles2 = rotation2 * targetAngles2;
                targetAngles.y = lookY;
                cameraDir = Vector3.Lerp(targetAngles2, targetAngles, Mathf.SmoothStep(0.0f, 1.0f, (stopwatch + oldDur)));
                ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(cameraDir);
            }

            if(!enemyBody || ((!enemyBody.healthComponent || !enemyBody.healthComponent.alive) && (isAuthority && stopwatch <= 2f)))
            {
                if (NetworkServer.active)
                {
                    if (this.characterBody.HasBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility)) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
                }
                if (base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
                outer.SetNextStateToMain();
                return;
            }

            if (isAuthority && stopwatch >= duration)
            {
                SetNext();
            }
        }

        protected virtual void SetNext()
        {
            outer.SetNextState(new StepEnd
            {
                cameraDir = cameraDir,
                cameraParamsOverrideHandle = cameraParamsOverrideHandle,
            });
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(cameraDir);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            cameraDir = reader.ReadVector3();
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
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 1f);
            }
            base .OnExit();
        }
    }
}
