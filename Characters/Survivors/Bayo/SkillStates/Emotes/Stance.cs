using EntityStates;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Stance : BaseState
    {
        public static float idleDuration = 1.5f;
        public static float cancelDuration = 0.5f;
        private RootMotionAccumulator outmove;
        private Animator animator;
        protected bool cancel;
        protected float stopwatch;
        protected bool hasExit;
        protected bool jumped;
        private bool flag1;
        private bool flag2;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            hasExit = false;
            flag1 = false;
            flag2 = false;
            outmove = GetModelRootMotionAccumulator();
            if (characterBody && characterBody.isSprinting) characterBody.isSprinting = false;

            PlayCrossfade("Body", "StanceIdle", "Emote.playbackRate", idleDuration, 0.05f);

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

                if (inputBank.interact.down) cancel = true;
                if (inputBank.moveVector != Vector3.zero) cancel = true;
                if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value)) cancel = true;
                if (Input.GetKeyDown(Modules.Config.emote2Keybind.Value)) cancel = true;
                if (Input.GetKeyDown(Modules.Config.emote3Keybind.Value)) cancel = true;
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

            if (stopwatch >= idleDuration)
            {
                if (!hasExit)
                {
                    hasExit = true;
                    PlayAnimation("Body", "StanceOut", "Emote.playbackRate", cancelDuration);
                }
                if (isAuthority && outmove)
                {
                    if (!flag1)
                    {
                        flag1 = true;
                    }
                    Vector3 vector = outmove.ExtractRootMotion();
                    if (isAuthority && characterMotor)
                    {
                        if (!flag2)
                        {
                            flag2 = true;
                        }
                        characterMotor.rootMotion = vector;
                    }
                }
            }
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = Vector3.zero;
            }
            if (isAuthority && stopwatch >= cancelDuration + idleDuration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("FullBody, Override", "BufferEmpty");

        }
    }
}
