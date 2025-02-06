using RoR2;
using UnityEngine;
using EntityStates.Merc;
using UnityEngine.Assertions.Must;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class HeelKick : BaseMeleeAttack
    {

        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        protected float earlyExit = 0.3f;
        protected string swing = "heelkick";
        protected Vector3 upForce = 16f * Vector3.up;

        public override void OnEnter()
        {

            duration = 0.75f;
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.75f;

            hitboxGroupName = "CoverGroup";
            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Stun1s;
            pushForce = 0f;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;
            launch = true;
            swingSoundString = swing;

            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "HeelKick", "Slash.playbackRate", duration);

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            base.OnEnter();

        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
                if (stopwatch >= duration * earlyExit + 0.012)
                {
                    if (inputBank.skill1.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.skill4.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }
                //if (stopwatch >= exitTime && inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }

        public override void FixedUpdate()
        {
            cancel = false;
            jumped = false;
            if (stopwatch >= duration * earlyExit)
            {
                DetermineCancel();
                if (jumped)
                {
                    inputBank.jump.PushState(false);
                }

                if (cancel)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
            if (CanDodge())
            {
                cancel = true;
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }



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

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            base.FixedUpdate();

        }
        protected override void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            if (!launchList.Contains(item))
            {
                launchList.Add(item);
                float num = 1f;
                Vector3 forceVec;
                bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.5f;

                if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
                }
                if (body.characterMotor)
                {
                    if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)
                    {
                        num = body.characterMotor.mass;
                    }
                    else
                    {
                        num = 100;
                    }
                    body.characterMotor.velocity.x = 0f;
                    body.characterMotor.velocity.z = 0f;
                }
                else if (item.GetComponent<Rigidbody>())
                {
                    if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)
                    {
                        num = body.rigidbody.mass;
                    }
                    else
                    {
                        num = 100;
                    }

                }

                forceVec = upForce * num;
                if (body.HasBuff(BayoBuffs.wtDebuff)) forceVec *= 0.8f;
                item.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
            }
        }
    }
}
