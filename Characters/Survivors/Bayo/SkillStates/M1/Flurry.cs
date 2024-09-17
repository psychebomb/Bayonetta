using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
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
        public override void OnEnter()
        {
            myDuration = 2.08f;
            duration = 3f;
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;
            earlyExitPercentTime = 0.3f;

            damageCoefficient = 2f;
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

            characterDirection.forward = GetAimRay().direction;
            PlayCrossfade("Body", "Flurry", "Slash.playbackRate", loopTime, 0.05f);

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

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            fireAge += Time.fixedDeltaTime;

            if (stopwatch >= loopTime)
            {
                if (!hasLooped)
                {
                    hasLooped = true;
                    PlayCrossfade("Body", "Flurry", "Slash.playbackRate", loopTime, 0.05f);
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
