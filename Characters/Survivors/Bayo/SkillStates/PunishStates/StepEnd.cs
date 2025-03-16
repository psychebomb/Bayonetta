using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2.CameraModes;
using UnityEngine.Networking;
using BayoMod.Survivors.Bayo;
using EntityStates;
using RoR2.Projectile;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Modules.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class StepEnd : BaseSkillState
    {
        protected float duration = 1.88f;
        protected float fireTime = 0.92f;
        private bool hasFired = false;
        private RootMotionAccumulator rootMotionAccumulator;
        protected string animName = "StepEnd";
        private float stopwatch;

        public static float damageCoefficient = 12f;
        public GameObject projectilePrefab = BayoAssets.footProjectilePrefab;
        protected float force = 1f;
        private float recoilAmplitude = 0.1f;
        private float bloom = 10;

        public CharacterBody enemyBody;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public Vector3 forwardDir;
        public Vector3 cameraDir;
        private CameraRigController Camera;
        private bool hasVoiced = false;

        public override void OnEnter()
        {

            rootMotionAccumulator = GetModelRootMotionAccumulator();
            //Util.PlaySound("flurspin", this.gameObject);

            PlayAnimation("Body", animName);

            if (characterBody.master.playerCharacterMasterController.networkUser)
            {
                Camera = characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;

            }

            enemyBody = base.GetComponent<PunishTracker>().GetTrackingTarget().healthComponent.body;

            forwardDir = characterDirection.forward;
            inputBank.moveVector = Vector3.zero;
            characterMotor.moveDirection = forwardDir;
            characterDirection.moveVector = forwardDir;

            base.OnEnter();

        }
        protected bool CanDodge()
        {
            if (inputBank.skill3.down && skillLocator.utility && (!skillLocator.utility.mustKeyPress || !inputBank.skill3.hasPressBeenClaimed) && skillLocator.utility.ExecuteIfReady())
            {
                return true;
            }
            return false;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            stopwatch += Time.fixedDeltaTime;

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
            if (stopwatch >= fireTime - 0.2f)
            {
                if (!hasVoiced)
                {
                    hasVoiced = true;
                    Util.PlaySound("punishend", this.gameObject);
                }
            }
            if (isAuthority && stopwatch >= fireTime)
            {
                if (!hasFired)
                {
                    hasFired = true;
                    FireProjectile();
                    DoFireEffects();
                }
            }
            if (isAuthority && stopwatch >= duration)
            {
                outer.SetNextState(new Stance());
            }

            if (characterDirection) characterDirection.forward = forwardDir;
            if(Camera && cameraDir != null)((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(cameraDir);
        }

        protected void DoFireEffects()
        {
            Util.PlaySound("weave", base.gameObject);
            AddRecoil(-2f * recoilAmplitude, -3f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            base.characterBody.AddSpreadBloom(bloom);
        }

        public virtual void FireProjectile()
        {
            Vector3 dir = characterDirection.forward;
            dir.y = 0;
            Vector3 pos = characterBody.transform.position + (dir.normalized * 2.25f) + (cameraDir * 1.55f);
            pos.y = pos.y - 1.5f;

            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                if (enemyBody && enemyBody.HasBuff(BayoBuffs.punishable)) enemyBody.RemoveBuff(BayoBuffs.punishable);
                if (this.characterBody.HasBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility)) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
            }
            if(base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
            base.OnExit();
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDir);
            writer.Write(cameraDir);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            cameraDir = reader.ReadVector3();
            forwardDir = reader.ReadVector3();
        }

    }
}

