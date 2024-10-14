using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using System;


namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class Dodge : BaseSkillState
    {
        public static float duration = 1.5f;
        public static float initialSpeedCoefficient = 2.5f;
        public static float finalSpeedCoefficient = 2f;
        protected bool cancel;

        public static string dodgeSoundString = "Roll";
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;

        private float rollSpeed;
        private float mSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;
        public int currentSwing = -1;
        protected float earlyExit = 0.525f;
        protected bool jumped;

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

            float num = Vector3.Dot(forwardDirection, rhs);
            float num2 = Vector3.Dot(forwardDirection, rhs2);

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
                jump.y = Math.Min(Math.Max(characterMotor.velocity.y, -10f),20f);
                characterMotor.velocity = jump;
            }

            Vector3 b = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = transform.position - b;

            PlayAnimation("Body", "Roll", "Roll.playbackRate", duration);
            Util.PlaySound(dodgeSoundString, gameObject);

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(BayoBuffs.armorBuff, earlyExit);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * earlyExit);
            }
        }

        private void RecalculateRollSpeed()
        {
            rollSpeed = mSpeed * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / earlyExit);
            if (isAuthority && fixedAge >= earlyExit)
            {
                rollSpeed = mSpeed * Mathf.Lerp(finalSpeedCoefficient, 0f, fixedAge / duration);
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
            if (cameraTargetParams) cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, fixedAge / earlyExit);

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

            if (isAuthority && fixedAge >= earlyExit)
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
            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextState(new Stance());
                return;
            }
        }

        public override void OnExit()
        {
            if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
            base.OnExit();
            if(cancel) PlayAnimation("FullBody, Override", "BufferEmpty");

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
            if (fixedAge >= earlyExit)
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

    }
}