using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class ABKEnd : BaseMeleeAttack
    {
        public Vector3 forwardDirr;
        protected AnimationCurve hoverSpeed;
        public override void OnEnter()
        {
            duration = 0.75f;
            attackStartPercentTime = 1.1f;
            attackEndPercentTime = 1.11f;
            damageCoefficient = 0f;
            procCoefficient = 0f;

            characterDirection.forward = forwardDirr;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            hoverSpeed = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.25f, 0f),
            });

            characterMotor.Motor.ForceUnground();
            exitToStance = false;
            shootRay = GetAimRay();
            gunName = "muzrf";
            gunDamage = 0.1f;
            fireTime = 0.15f;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            shootRay = GetAimRay();

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (characterDirection) characterDirection.forward = forwardDirr;
            if (!inHitPause && characterMotor)
            {
                float num = hoverSpeed.Evaluate(stopwatch / duration);
                characterMotor.velocity = forwardDirr * num * moveSpeedStat;

            }

            if (!base.inputBank.skill2.down)
            {
                outer.SetNextStateToMain();
                return;
            }

        }
        public override void OnExit()
        {
            PlayAnimation("Body", "AbkExit");
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }
    }
}