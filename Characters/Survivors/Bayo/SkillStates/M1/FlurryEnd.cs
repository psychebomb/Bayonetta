using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class FlurryEnd : BaseMeleeAttack
    {

        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private float earlyExit;

        public override void OnEnter()
        {

            duration = 1.92f;
            attackStartPercentTime = 0.25f;
            attackEndPercentTime = 0.625f;
            earlyExit = 1.5f;

            damageCoefficient = 15f;
            procCoefficient = 1.5f;
            damageType = DamageType.Generic;
            pushForce = 1500f;
            bonusForce = 0.6f * GetAimRay().direction * Uppercut.upwardForceStrength;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", "FlurryE", "Slash.playbackRate", duration);

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            base.OnEnter();

        }

        private void DetermineCancel()
        {
            if (characterMotor)
            {
                if (!characterMotor.isGrounded) cancel = true;
            }
            if (inputBank)
            {

                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }

                if (inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }

        public override void FixedUpdate()
        {
            cancel = false;
            jumped = false;
            if (stopwatch >= duration * attackEndPercentTime)
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
            if (inputBank.skill3.justPressed)
            {
                cancel = true;
                inputBank.skill3.PushState(false);
                outer.SetNextStateToMain();
                return;
            }

            characterDirection.forward = GetAimRay().direction;

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

            if (isAuthority && (stopwatch >= earlyExit))
            {
                if (inputBank.skill1.down)
                {
                    outer.SetNextState(new Punch1
                    {
                        swingIndex = 0
                    });
                    return;
                }

            }

            base.FixedUpdate();


        }
    }
}
