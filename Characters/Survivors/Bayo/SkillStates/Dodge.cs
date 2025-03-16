using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using System;
using R2API;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using System.ComponentModel;
using BayoMod.Modules.Components;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class Dodge : BaseSkillState
    {
        public static float duration = 1.5f;
        public static float initialSpeedCoefficient = 2.5f;
        public static float finalSpeedCoefficient = 2f;
        protected bool cancel;

        public static string dodgeSoundString = "dodge";

        private float rollSpeed;
        private float mSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;
        public int currentSwing = -1;
        protected float earlyExit = 0.525f;
        protected bool jumped;
        protected float stopwatch;
        private float baseSpeed;
        private float speedMult;

        protected float evadeWatch;
        private bool inEvade = false;
        private bool evadeDone = false;
        private bool slowed = false;
        protected float evadeTime = 0.4f;
        private HitStopCachedState hitStopCachedState;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        private CharacterCameraParams dodgeCam;
        public static CharacterCameraParams cameraParams;
        protected bool rlyGoodTiming = false;
        private float dodgeTime;
        private float armorTime;
        private bool buffDone = false;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            CreateCamera();

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
            Util.PlaySound("evade", gameObject);

            baseSpeed = 7f;
            if (this.characterBody.isSprinting) { baseSpeed *= this.characterBody.sprintingSpeedMultiplier; }

            armorTime = (earlyExit + 0.13333333f);
            dodgeTime = 0.25f;

            if (this.moveSpeedStat - baseSpeed > 0)
            {
                speedMult = 1 + ((this.moveSpeedStat - baseSpeed) / baseSpeed);
                //Chat.AddMessage("mult: " + speedMult.ToString());
                armorTime *= speedMult;
                armorTime = Math.Min(armorTime, 1f);
                dodgeTime *= speedMult;
                dodgeTime = Math.Min(dodgeTime, earlyExit + 0.13333333f);

            }

            //Chat.AddMessage("armorTime: " + armorTime.ToString());
            //Chat.AddMessage("dodgeTime: " + dodgeTime.ToString());

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(BayoBuffs.armorBuff, armorTime);
                if(characterBody.HasBuff(BayoBuffs.wtBuff)|| characterBody.HasBuff(BayoBuffs.wtCoolDown))
                {
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, dodgeTime);
                }
                else
                {
                    characterBody.AddTimedBuff(BayoBuffs.dodgeBuff, dodgeTime);
                }
            }

            //ResizeHurtbox();
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
                    hitStopCachedState = CreateHitStopCachedState(this.characterMotor, this.animator, "Roll.playbackRate");
                    mSpeed *= (1f / 3f);
                    if (base.cameraTargetParams)
                    {
                        cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                        {
                            cameraParamsData = cameraParams.data,
                            priority = 1f
                        }, 0.35f);
                    }
                }
                if (this.animator) this.animator.SetFloat("Roll.playbackRate", 0.33333f);

                if (evadeWatch < 0f && !evadeDone)
                {
                    inEvade = false;
                    evadeDone = true;
                    mSpeed *= 3f;
                    earlyExit -= 0.13333f;
                    cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.25f);
                    ConsumeHitStopCachedState(hitStopCachedState, this.characterMotor, this.animator);
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

            if(isAuthority && stopwatch > dodgeTime && !buffDone)
            {
                //ResizeHurtbox();
                buffDone = true;
            }

            if (isAuthority && stopwatch >= earlyExit && !inEvade)
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
            int wtDur = 6;
            int hardlights = this.characterBody.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine);
            wtDur += (hardlights * 3);

            if (cancel) PlayAnimation("FullBody, Override", "BufferEmpty");
            if (evadeDone && NetworkServer.active)
            {
                Util.PlaySound("wtv", this.gameObject);
                if (rlyGoodTiming)
                {
                    for (int k = 1; k <= wtDur; k++)
                    {
                        characterBody.AddTimedBuff(BayoBuffs.wtBuff, k);
                    }
                }
                else
                {
                    for (int k = 1; k <= (wtDur - 2); k++)
                    {
                        characterBody.AddTimedBuff(BayoBuffs.wtBuff, k);
                    }
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
                Util.PlaySound("ds", this.gameObject);
                inEvade = true;
                evadeWatch = evadeTime;
                float goodTime = 0.1f;
                if (this.moveSpeedStat - baseSpeed > 0)
                {
                    goodTime *= speedMult;
                    goodTime = Math.Min(goodTime, earlyExit + 0.13333333f);
                }

                //Chat.AddMessage("goodTime: " + goodTime.ToString());

                if (stopwatch <= goodTime)
                {
                    rlyGoodTiming = true;
                }

                if (NetworkServer.active)
                {
                    characterBody.RemoveBuff(BayoBuffs.evadeSuccess);
                    characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, (earlyExit - stopwatch + evadeTime));
                }
            }
        }

        private void CreateCamera()
        {
            dodgeCam = ScriptableObject.CreateInstance<CharacterCameraParams>();
            dodgeCam.name = "dodgeCam";
            dodgeCam.data.wallCushion = 0.1f;
            dodgeCam.data.idealLocalCameraPos = new Vector3(0f, -1f, -5f);
            cameraParams = dodgeCam;
        }
    }
}