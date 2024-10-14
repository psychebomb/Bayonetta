using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using UnityEngine.UIElements;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class RisingFinisher : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq;
        public override void OnEnter()
        {
            duration = 1f;
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 1f;
            earlyExitPercentTime = 0.3f;

            damageCoefficient = 1.25f;
            procCoefficient = 0.5f;
            damageType = DamageType.Stun1s;
            pushForce = 0f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = false;
            fireAge = 0f;
            fireFreq = 0.15f;
            StartAimMode(0.5f + duration, false);
            launch = true;
            juggleHop = 2f;
            PlayAnimation("Body", "BackKickExit", playbackRateParam, attackStartPercentTime);

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
                PlayAnimation("Body", "BackKickExit", playbackRateParam, fireFreq);
                attack.ResetIgnoredHealthComponents();
                attack.Fire();
                hasFired = false;
                launchList.Clear();
            }
            base.FireAttack();
        }

        public override void FixedUpdate()
        {

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (isAuthority && characterMotor)
            {
                inputBank.moveVector = Vector3.zero;
                characterMotor.moveDirection = Vector3.zero;
            }
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }


            fireAge += Time.fixedDeltaTime;

            base.FixedUpdate();

        }
    }
}
