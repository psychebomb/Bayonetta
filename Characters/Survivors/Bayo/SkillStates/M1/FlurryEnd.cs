using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using EntityStates.Loader;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class FlurryEnd : BaseMeleeAttack
    {

        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private float earlyExit;
        private string animName;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;

        public override void OnEnter()
        {

            duration = 1.92f;
            attackStartPercentTime = 0.25f;
            attackEndPercentTime = 0.5f;
            earlyExit = 1.5f;

            damageCoefficient = 15f;
            procCoefficient = 1.5f;
            damageType = DamageType.Generic;
            pushForce = 1500f;
            bonusForce = 0.6f * GetAimRay().direction * Uppercut.upwardForceStrength;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            characterMotor.velocity.y = 0f;
            exitToStance = true;
            if (characterMotor.isGrounded)
            {
                animName = "FlurryE";
            }
            else
            {
                animName = "FlurryAE";
                characterMotor.airControl = characterMotor.airControl;
            }

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", animName, "Slash.playbackRate", duration);

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
                if (stopwatch >= duration * attackEndPercentTime + 0.012)
                {
                    if (inputBank.skill2.down) cancel = true;
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
            else
            {
                base.characterMotor.rootMotion = Vector3.zero;
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterMotor.velocity.y = Mathf.Lerp(0f, -20f, fixedAge / duration);
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
