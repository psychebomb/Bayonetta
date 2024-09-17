using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class RisingKick : BaseMeleeAttack
    {
        public enum SpeedState
        {
            starting,
            middle,
            slower,
        }

        protected float fallOffTime = 0.7f;
        protected float initialSpeedCoefficient = 0f;
        protected float midSpeedCoefficient = 8f;
        private Vector3 forwardDir;
        private Vector3 previousPosition;
        private float rollSpeed;
        public SpeedState speedState;
        private bool hasEnded;
        public override void OnEnter()
        {
            duration = 1f;
            earlyExitPercentTime = 0.75f;
            attackStartPercentTime = 0.5f;
            attackEndPercentTime = 0.95f;
            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Stun1s;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            bonusForce = 0.8f * Vector3.up * Uppercut.upwardForceStrength;
            
            forwardDir = Vector3.up;
            rollSpeed = 0f;
            characterDirection.forward = GetAimRay().direction;

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = forwardDir * rollSpeed;
            }

            Vector3 b = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = transform.position - b;
            PlayAnimation("Body", "BackKickStart", playbackRateParam, attackStartPercentTime);
            characterMotor.Motor.ForceUnground();
            exitToStance = false;

            base.OnEnter();
        }

        private void GetNextSpeed()
        {
            if (stopwatch <= attackStartPercentTime * duration)
            {
                speedState = SpeedState.starting;
            }
            if (stopwatch > attackStartPercentTime * duration && stopwatch <= fallOffTime)
            {
                speedState = SpeedState.middle;
            }
            if (stopwatch > fallOffTime)
            {
                speedState = SpeedState.slower;
            }
        }
        private void RecalculateRollSpeed()
        {
            switch (speedState)
            {
                case SpeedState.starting:
                    rollSpeed = 0f;
                    break;
                case SpeedState.middle:
                    rollSpeed = moveSpeedStat * midSpeedCoefficient;
                    break;
                case SpeedState.slower:
                    rollSpeed = moveSpeedStat * Mathf.Lerp(midSpeedCoefficient, 1f, stopwatch / duration);
                    break;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            GetNextSpeed();
            RecalculateRollSpeed();

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (characterDirection) characterDirection.forward = GetAimRay().direction;
            if (!inHitPause)
            {
                Vector3 normalized = (transform.position - previousPosition).normalized;
                if (characterMotor && characterDirection)
                {
                    Vector3 vector = normalized * rollSpeed;
                    float d = Mathf.Max(Vector3.Dot(vector, forwardDir), 0f);
                    vector = forwardDir * d;
                    vector.x = 0f;
                    vector.z = 0f;

                    characterMotor.velocity = vector;
                }
                previousPosition = transform.position;
            }

            if (isAuthority && (stopwatch >= earlyExitPercentTime))
            {
                if (!hasEnded)
                {
                    hasEnded = true;
                    PlayAnimation("Body", "BackKickExit", playbackRateParam, duration - earlyExitPercentTime);
                }
                if (base.inputBank.skill2.down)
                {
                    outer.SetNextState(new RisingFinisher());
                    return;
                }

            }
        }
        protected override void RemoveHitstop()
        {
            base.RemoveHitstop();
            characterMotor.Motor.ForceUnground();
        }

        protected override void EnterAttack()
        {
            characterMotor.Motor.ForceUnground();
            PlayCrossfade("Body", "BackKick", playbackRateParam, earlyExitPercentTime - attackStartPercentTime, 0.05f);
            base.EnterAttack();
        }

        public override void OnExit()
        {
            PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }
    }
}
