using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using System;
using R2API;


namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class Dodge : BaseSkillState
    {
        public static float duration = 1.5f;
        public static float initialSpeedCoefficient = 2.5f;
        public static float finalSpeedCoefficient = 2f;
        protected bool cancel;

        public static string dodgeSoundString = "Roll";

        private float rollSpeed;
        private float mSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;
        public int currentSwing = -1;
        protected float earlyExit = 0.525f;
        protected bool jumped;
        protected float stopwatch;

        protected float evadeWatch;
        protected bool inEvade = false;
        protected bool evadeDone = false;
        protected bool slowed = false;
        protected float evadeTime = 0.4f;
        private HitStopCachedState hitStopCachedState;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        private CharacterCameraParams dodgeCam;
        public static CharacterCameraParams cameraParams;
        protected bool rlyGoodTiming = false;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();

            if (isAuthority && inputBank && characterDirection)
            {
                forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
            }
            if (isAuthority && inputBank.skill1.down)
            {
                skillLocator.primary.Reset();
            }

            Vector3 rhs = characterDirection ? characterDirection.forward : forwardDirection;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);

            mSpeed = moveSpeedStat;
            if (inputBank.skill1.down)
            {
                mSpeed = 6.5f;
            }
            else
            {
                mSpeed = Math.Max(mSpeed, 6.5f);
            }

            RecalculateRollSpeed();

            if (characterMotor && characterDirection)
            {
                Vector3 jump = forwardDirection * rollSpeed;
                jump.y = Math.Min(Math.Max(characterMotor.velocity.y, -10f), 20f);
                characterMotor.velocity = jump;
            }

            Vector3 b = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = transform.position - b;

            PlayAnimation("Body", "Roll", "Roll.playbackRate", duration);
            Util.PlaySound(dodgeSoundString, gameObject);

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(BayoBuffs.armorBuff, earlyExit + 0.13333333f);
                if(characterBody.HasBuff(BayoBuffs.wtBuff)|| characterBody.HasBuff(BayoBuffs.wtCoolDown))
                {
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.25f);
                }
                else
                {
                    characterBody.AddTimedBuff(BayoBuffs.dodgeBuff, 0.25f);
                }
            }
        }

        private void RecalculateRollSpeed()
        {
            rollSpeed = mSpeed * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, stopwatch / earlyExit);
            if (isAuthority && stopwatch >= earlyExit)
            {
                rollSpeed = mSpeed * Mathf.Lerp(finalSpeedCoefficient, 0f, stopwatch / duration);
            }
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
            base.FixedUpdate();
            RecalculateRollSpeed();
            cancel = false;
            jumped = false;

            if (characterDirection) characterDirection.forward = forwardDirection;
 
            if (!inEvade)
            {
                stopwatch += Time.fixedDeltaTime;
                HandleBuffs();
            }
            else
            {
                evadeWatch -= Time.fixedDeltaTime;
                if (!slowed)
                {
                    slowed = true;
                    hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, "Roll.playbackRate");
                    mSpeed *= (1f / 3f);
                    CreateCamera();
                }
                if (animator) animator.SetFloat("Roll.playbackRate", 0.33333f);

                if (evadeWatch < 0f && !evadeDone)
                {
                    inEvade = false;
                    evadeDone = true;
                    mSpeed *= 3f;
                    earlyExit -= 0.13333f;
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.25f);
                    ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
                }
            }

            Vector3 normalized = (transform.position - previousPosition).normalized;
            if (characterMotor && characterDirection && normalized != Vector3.zero)
            {
                Vector3 vector = normalized * rollSpeed;
                float d = Mathf.Max(Vector3.Dot(vector, forwardDirection), 0f);
                vector = forwardDirection * d;
                vector.y = characterMotor.velocity.y;

                characterMotor.velocity = vector;
            }
            previousPosition = transform.position;

            if (isAuthority && stopwatch >= earlyExit)
            {
                DetermineCancel();
                if (inputBank.skill1.down)
                {
                    SetStep();
                    return;
                }
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
            if (isAuthority && stopwatch >= duration)
            {
                outer.SetNextState(new Stance());
                return;
            }
        }

        public override void OnExit()
        {
            if (cancel) PlayAnimation("FullBody, Override", "BufferEmpty");
            if (evadeDone && NetworkServer.active)
            {
                if (rlyGoodTiming)
                {
                    characterBody.AddTimedBuff(BayoBuffs.wtBuff, 6f);
                    //for (int k = 1; k <= 10f; k++)
                    //{
                    //    characterBody.AddTimedBuff(BayoBuffs.wtCoolDown, k + 6);
                    //}
                    //Chat.AddMessage("6 seconds");
                }
                else
                {
                    //for (int k = 1; k <= 12f; k++)
                    //{
                    //    characterBody.AddTimedBuff(BayoBuffs.wtCoolDown, k + 4);
                    //}
                    characterBody.AddTimedBuff(BayoBuffs.wtBuff, 4f);
                    //Chat.AddMessage("4 seconds");
                }
            }
            base.OnExit();

            characterMotor.disableAirControlUntilCollision = false;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= earlyExit)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public void SetStep()
        {
            switch (currentSwing + 1)
            {
                case 0:
                    outer.SetNextState(new Punch1
                    {
                        swingIndex = 0
                    });
                    break;
                case 1:
                    outer.SetNextState(new Punch2
                    {
                        swingIndex = 1
                    });
                    break;
                case 2:
                    outer.SetNextState(new Punch3
                    {
                        swingIndex = 2
                    });
                    break;
                case 3:
                    outer.SetNextState(new Punch4
                    {
                        swingIndex = 3
                    });
                    break;
                case 4:
                    outer.SetNextState(new FlurryStart());
                    break;
                case 5:
                    outer.SetNextState(new FlurryEnd());
                    break;

            }
        }

        private void HandleBuffs()
        {
            if (characterBody.HasBuff(BayoBuffs.evadeSuccess))
            {
                characterBody.RemoveBuff(BayoBuffs.evadeSuccess);
                evadeWatch = evadeTime;
                characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, (earlyExit - stopwatch + evadeTime));
                inEvade = true;
                if (stopwatch < 0.1f) rlyGoodTiming = true;
            }
        }

        private void CreateCamera()
        {
            dodgeCam = ScriptableObject.CreateInstance<CharacterCameraParams>();
            dodgeCam.name = "dodgeCam";
            dodgeCam.data.wallCushion = 0.1f;
            dodgeCam.data.idealLocalCameraPos = new Vector3(0f, -1f, -5f);
            cameraParams = dodgeCam;
            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 0.35f);
            }
        }

    }
}