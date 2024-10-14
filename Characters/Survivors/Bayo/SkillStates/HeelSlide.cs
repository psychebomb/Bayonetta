using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class HeelSlide : BaseMeleeAttack
    {
        protected float fallOffTime = 0.35f;
        protected float initialSpeedCoefficient = 0f;
        protected float midSpeedCoefficient = 8f;
        private Vector3 forwardDir;
        private bool hasEnded;
        private bool hasHit;
        protected AnimationCurve kickSpeed;
        public override void OnEnter()
        {
            duration = 1.72f;
            earlyExitPercentTime = 1.36f;
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;
            damageCoefficient = 2f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;
            hasHit = false;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            forwardDir = GetAimRay().direction;
            hitboxGroupName = "HeelGroup";

            characterDirection.forward = forwardDir;

            kickSpeed = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1f),
                new Keyframe(0.26f, 1.5f),
                new Keyframe(0.27f, 9f),
                new Keyframe(0.7f, 7f),
                new Keyframe(0.1f, 3f),
                new Keyframe(earlyExitPercentTime, 1f),
                new Keyframe(1.72f, 0f)
            });

            PlayAnimation("Body", "HeelSlide", playbackRateParam, 0.26f);
            exitToStance = true;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (characterDirection) characterDirection.forward = forwardDir;
            if (!inHitPause && characterMotor)
            {
                float num = kickSpeed.Evaluate(stopwatch);
                characterMotor.velocity = forwardDir * num * moveSpeedStat;
                if (hasHit) { characterMotor.velocity = Vector3.zero; }

            }
            if(isAuthority && (fixedAge >= earlyExitPercentTime || hasHit))
            {
                if (!hasEnded)
                {
                    hasEnded = true;
                    PlayAnimation("Body", "SlideExit", playbackRateParam, (duration - earlyExitPercentTime));
                }
                if (inputBank.skill2.down)
                {
                    characterMotor.velocity = Vector3.zero;
                    outer.SetNextState(new HeelKick());
                    return;
                }
                if (hasHit)
                {
                    outer.SetNextStateToMain();
                }
            }

        }
        protected override void RemoveHitstop()
        {
            base.RemoveHitstop();
            fixedAge = earlyExitPercentTime;
            stopwatch = earlyExitPercentTime;
            hasHit = true;
        }
    }
}
