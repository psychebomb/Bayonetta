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
    public class Throw : BaseSkillState
    {
        protected float duration = 0.68f;
        private RootMotionAccumulator rootMotionAccumulator;
        protected string animName = "Throw";
        private float stopwatch;

        public CharacterBody enemyBody;
        public Vector3 forwardDir;
        private bool hasVoiced = false;

        public override void OnEnter()
        {

            rootMotionAccumulator = GetModelRootMotionAccumulator();

            PlayAnimation("Body", animName);

            Util.PlaySound("throw", this.gameObject);

            forwardDir = GetAimRay().direction;

            characterDirection.forward = forwardDir;
            inputBank.moveVector = Vector3.zero;
            characterMotor.moveDirection = forwardDir;
            characterDirection.moveVector = forwardDir;

            enemyBody = base.GetComponent<PunishTracker>().GetTrackingTarget().healthComponent.body;

            if (enemyBody)
            {
                if (enemyBody.GetComponent<CapsuleCollider>())
                {
                    enemyBody.GetComponent<CapsuleCollider>().enabled = true;
                }
                else if (enemyBody.GetComponent<SphereCollider>())
                {
                    enemyBody.GetComponent<SphereCollider>().enabled = true;
                }

                float num = 0f;
                if (enemyBody.characterMotor)
                {
                    num = enemyBody.characterMotor.mass;
                }
                else if (enemyBody.rigidbody)
                {
                    num = enemyBody.rigidbody.mass;
                }
                num *= 100f;
                Vector3 forceVec = forwardDir * num;
                if(enemyBody.healthComponent) enemyBody.healthComponent.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
            }

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

            if (isAuthority && stopwatch >= duration)
            {
                outer.SetNextState(new Stance());
            }
            characterDirection.forward = forwardDir;
            inputBank.moveVector = Vector3.zero;
            characterMotor.moveDirection = Vector3.zero;

        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                if(enemyBody && enemyBody.HasBuff(BayoBuffs.punishable)) enemyBody.RemoveBuff(BayoBuffs.punishable);
                if(this.characterBody.HasBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility)) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
            }
            if (base.GetComponent<PunishTracker>()) base.GetComponent<PunishTracker>().punishing = false;
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

