using EntityStates;
using RoR2;
using UnityEngine;


namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class BaseEmote : BaseState
    {
        public float animDuration;
        public float cancelDuration;
        public string animString;
        private RootMotionAccumulator rootmotion;
        protected bool cancel;
        protected float stopwatch;
        protected bool hasExit;
        protected bool jumped;
        private bool flag1;

        public override void OnEnter()
        {
            rootmotion = base.GetModelRootMotionAccumulator();
            hasExit = false;
            flag1 = false;
            base.characterBody.hideCrosshair = true;
            base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);
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
                if (inputBank.skill1.down) cancel = true;
                if (inputBank.skill2.down) cancel = true;
                if (inputBank.skill3.down) cancel = true;
                if (inputBank.skill4.down) cancel = true;
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
            stopwatch += Time.fixedDeltaTime;
            cancel = false;
            jumped = false;
            DetermineCancel();
            base.FixedUpdate();         

            if (jumped)
            {
                inputBank.jump.PushState(false);
            }

            if (cancel)
            {
                outer.SetNextStateToMain();
                return;
            }

            if (rootmotion)
            {
                if (!flag1)
                {
                    flag1 = true;
                }
                Vector3 vector = rootmotion.ExtractRootMotion();
                if (isAuthority && characterMotor)
                {
                    characterMotor.rootMotion = vector;
                }
            }

            if (isAuthority && stopwatch >= (animDuration))
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.hideCrosshair = false;
            PlayAnimation("FullBody, Override", "BufferEmpty");

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}