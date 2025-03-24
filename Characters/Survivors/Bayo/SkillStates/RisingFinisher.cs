using RoR2;
using UnityEngine;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class RisingFinisher : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq = 0.15f;
        protected bool hasEnded;
        protected float damage = 1.25f;
        protected float dur = 1.25f;
        protected float attackStart = 0.1f;
        protected float attackEnd = 0.8f;
        protected float earlyEnd = 1f;
        protected string muzName = "muzlf";
        protected float gDam = 0.1f;
        protected float frTime = 0.1f;
        protected ModelLocator component;
        protected ChildLocator component2;
        protected Ray gunRay;

        protected bool cancel = false;
        protected bool jumped = false;
        protected bool clear = true;
        public override void OnEnter()
        {
            duration = dur;
            attackStartPercentTime = attackStart;
            attackEndPercentTime = attackEnd;
            earlyExitPercentTime = earlyEnd;
            hasEnded = false;

            damageCoefficient = damage;
            procCoefficient = 0.5f;
            pushForce = 0f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = false;
            fireAge = 0f;
            StartAimMode(0.5f + duration, false);
            launch = true;
            juggleHop = 2f;
            hitboxGroupName = "CoverGroup";
            hitboxName = "Envelop";
            PlayAnim();

            component = gameObject.GetComponent<ModelLocator>();
            component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex(muzName);
                Transform transformm = component2.FindChild(childIndex);
                int childIndex2 = component2.FindChildIndex(muzName + "f");
                Transform transformm2 = component2.FindChild(childIndex2);
                gunRay = new Ray(transform.position, transformm2.position - transformm.position);
            }
            
            shootRay = gunRay;
            gunName = muzName;
            gunDamage = gDam;
            fireTime = frTime;

            base.OnEnter();

            fireFreq /=this.attackSpeedStat;

        }

        protected virtual void PlayAnim()
        {
            PlayAnimation("Body", "BackSpin");
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
        }

        protected override void FireAttack()
        {
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                attack.ResetIgnoredHealthComponents();
                //attack.Fire();
                hasFired = false;
                if (clear) results.Clear();
            }
            base.FireAttack();
        }

        protected virtual void DetermineCancel()
        {

            if (inputBank)
            {
                if (hasEnded)
                {
                    if (inputBank.skill1.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.skill4.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }
                //if (stopwatch >= exitTime && inputBank.moveVector != Vector3.zero) cancel = true;
            }
        }

        protected virtual void FinisherSpecific()
        {
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex(muzName);
                Transform transformm = component2.FindChild(childIndex);
                int childIndex2 = component2.FindChildIndex(muzName + "f");
                Transform transformm2 = component2.FindChild(childIndex2);
                gunRay = new Ray(transform.position, transformm2.position - transformm.position);
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

            if (stopwatch >= earlyExitPercentTime && !hasEnded)
            {
                hasEnded = true;
                fireTime = 9999f;
                PlayAnimation("Body", "BackKickExit", playbackRateParam, duration - earlyExitPercentTime);
            }
        }

        public override void FixedUpdate()
        {
            cancel = false;
            jumped = false;
            if (stopwatch >= earlyExitPercentTime)
            {
                DetermineCancel();
                if (jumped)
                {
                    inputBank.jump.PushState(false);
                }

                if (cancel)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            FinisherSpecific();

            shootRay = gunRay;

            fireAge += Time.fixedDeltaTime;

            base.FixedUpdate();

        }
    }
}
