using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using UnityEngine.UIElements;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Flurry : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq;
        protected float loopTime;
        protected bool hasLooped;
        protected float myDuration;
        private string animName;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;
        public override void OnEnter()
        {
            myDuration = 2.08f;
            duration = 3f;
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;
            earlyExitPercentTime = 0.3f;

            damageCoefficient = 1.5f;
            procCoefficient = 0.75f;
            damageType = DamageType.Generic;
            pushForce = 300f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = false;
            fireAge = 0f;
            fireFreq = 0.2f;
            loopTime = 1.12f;
            hasLooped = false;
            shootRay = GetAimRay();
            gunName = "gunrh4";
            gunDamage = 0.5f;
            fireTime = 0.15f;

            if (characterMotor.isGrounded)
            {
                animName = "Flurry";
            }
            else
            {
                animName = "FlurryA";
                characterMotor.airControl = characterMotor.airControl;
            }

            characterDirection.forward = GetAimRay().direction;
            PlayCrossfade("Body", animName, "Slash.playbackRate", loopTime, 0.05f);

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            base.OnEnter();

        }

        protected override void FireAttack()
        {
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                attack.ResetIgnoredHealthComponents();
                attack.Fire();
                hasFired = false;
            }
            base.FireAttack();
        }

        public override void FixedUpdate()
        {
        
            if (CanDodge())
            {
                outer.SetNextState(new Dodge { currentSwing = 4 });
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            characterDirection.forward = GetAimRay().direction;
            shootRay = GetAimRay();

            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    inputBank.moveVector = Vector3.zero;
                    characterMotor.moveDirection = Vector3.zero;
                }
                if (characterMotor && characterDirection)
                {
                    characterMotor.velocity = characterMotor.velocity * 0f;
                }
            }
            else
            {
                base.characterMotor.rootMotion = Vector3.zero;
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterMotor.velocity.y = Mathf.Lerp(0f, -20f, fixedAge / duration);
            }

            fireAge += Time.fixedDeltaTime;

            if (stopwatch >= loopTime)
            {
                if (!hasLooped)
                {
                    hasLooped = true;
                    PlayCrossfade("Body", animName, "Slash.playbackRate", loopTime, 0.05f);
                }   
            }

            if (isAuthority && stopwatch >= earlyExitPercentTime)
            {
                if (!inputBank.skill1.down || stopwatch >= myDuration)
                {
                    outer.SetNextState(new FlurryEnd());
                    return;
                }
            }

            base.FixedUpdate();

        }
    }
}
