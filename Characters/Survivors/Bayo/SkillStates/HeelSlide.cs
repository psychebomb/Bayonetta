using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;

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
        private bool hasStarted;
        protected AnimationCurve kickSpeed;
        public override void OnEnter()
        {
            duration = 1.72f;
            earlyExitPercentTime = 1.36f;
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;
            damageCoefficient = 1f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;
            hasHit = false;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            forwardDir = GetAimRay().direction;
            forwardDir.y = 0f;
            hitboxGroupName = "HeelGroup";
            hitboxName = "HeelHitbox";
            loopEffectPrefab = BayoAssets.heels;

            characterDirection.forward = forwardDir;

            kickSpeed = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.39f, 2f),
                new Keyframe(0.4f, 9f),
                new Keyframe(0.8f, 6.5f),
                new Keyframe(0.1f, 3f),
                new Keyframe(earlyExitPercentTime, 1f),
                new Keyframe(1.72f, 0f)
            });

            PlayAnimation("Body", "HeelSlide", playbackRateParam, 0.52f);
            exitToStance = true;

            Vector3 vec = forwardDir;
            vec.y = 0f;
            Ray ray = new Ray(GetAimRay().origin, vec);
            shootRay = ray;
            gunName = "muzrf";
            gunDamage = 0.1f;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Ray ray = new Ray(GetAimRay().origin, forwardDir);
            shootRay = ray;

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
                    fireTime = 100f;
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
            if(isAuthority && !hasStarted && stopwatch>= 0.4f)
            {
                fireTime = 0.15f / attackSpeedStat;
                Util.PlaySound("fallslide", this.gameObject);
                Util.PlaySound("heelslide", this.gameObject);
                hasStarted = true;
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
