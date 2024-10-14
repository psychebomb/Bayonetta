using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class HeelKick : BaseMeleeAttack
    {

        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private float earlyExit;
        protected Vector3 upForce = 0.6f * Vector3.up * Uppercut.upwardForceStrength;

        public override void OnEnter()
        {

            duration = 0.75f;
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.75f;
            earlyExit = 0.25f;

            hitboxGroupName = "CoverGroup";
            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Stun1s;
            pushForce = 0f;
            bonusForce = upForce;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = true;

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
    }
}
