using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class ABKEnd : BaseMeleeAttack
    {
        public Vector3 forwardDirr;
        protected AnimationCurve hoverSpeed;
        private Transform boneTrans;
        private ABKRotator abkr;
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

            ModelLocator component = this.gameObject.GetComponent<ModelLocator>();
            ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex("BoneR");
                boneTrans = component2.FindChild(childIndex);
            }

            abkr = GetComponent<ABKRotator>();
            characterMotor.Motor.ForceUnground();
            exitToStance = false;
            shootRay = GetAimRay();
            gunName = "muzrf";
            gunDamage = 1f;
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
            abkr.lookDir = Vector3.zero;
            abkr.rotate = false;

            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }
    }
}