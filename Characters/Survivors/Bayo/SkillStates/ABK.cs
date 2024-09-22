using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class ABK : BaseMeleeAttack
    {
        protected float fallOffTime = 0.35f;
        protected float initialSpeedCoefficient = 0f;
        protected float midSpeedCoefficient = 8f;
        private Vector3 forwardDir;
        private bool hasEnded;
        protected AnimationCurve kickSpeed;
        public override void OnEnter()
        {
            duration = 0.65f;
            attackStartPercentTime = 0.05f;
            attackEndPercentTime = 1f;
            damageCoefficient = 4.5f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            forwardDir = GetAimRay().direction;
            bonusForce = 0.8f * forwardDir * Uppercut.upwardForceStrength;

            characterDirection.forward = forwardDir;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            kickSpeed = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.2f, 7f),
                new Keyframe(0.5f, 7f),
                new Keyframe(0.65f, 1.5f)
            });

            PlayAnimation("Body", "Abk", playbackRateParam, duration);
            characterMotor.Motor.ForceUnground();
            exitToStance = false;

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
                float num = kickSpeed.Evaluate(stopwatch / duration);
                characterMotor.velocity = forwardDir * num * moveSpeedStat;
                
            }

        }
        protected override void RemoveHitstop()
        {
            base.RemoveHitstop();
            characterMotor.Motor.ForceUnground();
        }
        public override void OnExit()
        {
            PlayAnimation("Body", "AbkExit");
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }
    }
}
