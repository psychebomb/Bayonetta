using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class ABK : BaseMeleeAttack
    {
        private Vector3 forwardDir;
        private bool hasExtended;
        protected AnimationCurve kickSpeed;
        protected Vector3 speedVec;
        protected Ray saveRay;
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
            saveRay = GetAimRay();
            //bonusForce = 0.8f * forwardDir * Uppercut.upwardForceStrength;

            characterDirection.forward = forwardDir;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            kickSpeed = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.2f, 7f),
                new Keyframe(0.5f, 7f),
                new Keyframe(0.75f, 1.5f)
            });

            PlayAnimation("Body", "Abk", playbackRateParam, duration);
            characterMotor.Motor.ForceUnground();
            exitToStance = false;
            hasExtended = false;
            launch = true;

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
                speedVec = forwardDir * num * moveSpeedStat;
                characterMotor.velocity = speedVec;

            }

            if (stopwatch >= duration && isAuthority)
            {
                if (base.inputBank.skill2.down)
                {
                    outer.SetNextState(new ABKEnd
                    {
                        forwardDirr = forwardDir,
                    });
                    hasExtended = true;
                }
                else { outer.SetNextStateToMain(); }
                return;
            }

        }
        protected override void RemoveHitstop()
        {
            base.RemoveHitstop();
            characterMotor.Motor.ForceUnground();
        }
        public override void OnExit()
        {
            if (!hasExtended) { PlayAnimation("Body", "AbkExit"); }
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }

        protected override void ApplyForce(HurtBox item)
        {
            CharacterBody body = item.healthComponent.body;
            float num = 1f;
            Vector3 forceVec;

            if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }
            if (body.characterMotor)
            {
                num = body.characterMotor.mass;
            }
            else if (item.healthComponent.GetComponent<Rigidbody>())
            {
                num = base.rigidbody.mass;
            }
            //num = num / 150f;
            body.characterMotor.velocity = speedVec * 0.9f;
            //forceVec = speedVec.normalized;
            //forceVec.y += 30f;
            //forceVec = forceVec * num; //Mathf.Lerp(1f, 0f, fixedAge / duration) * moveSpeedStat;
            //item.healthComponent.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
        }
    }
}
