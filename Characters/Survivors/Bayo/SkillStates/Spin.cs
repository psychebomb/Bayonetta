using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class Spin : RisingFinisher
    {

        public override void OnEnter()
        {
            dur = 1.6f;
            damage = 2f;
            fireFreq = 0.24f;
            clear = false;
            attackEnd = 0.625f;
            muzName = "muzrf";
            gDam = 0.25f;
            frTime = 0.1f;
            damageType = DamageType.Stun1s;
            Util.PlaySound("spin", this.gameObject);
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(stopwatch >= fireFreq)
            {
                if (inputBank.jump.down)
                {
                    inputBank.jump.PushState(false);
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        protected override void PlayAnim()
        {
            PlayAnimation("Body", "Spin", playbackRateParam, fireFreq);
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
        }

        protected override void FinisherSpecific()
        {
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex(muzName);
                Transform transformm = component2.FindChild(childIndex);
                int childIndex2 = component2.FindChildIndex(muzName + "f");
                Transform transformm2 = component2.FindChild(childIndex2);
                gunRay = new Ray(transform.position, transformm2.position - transformm.position);
            }
            Vector3 vec = gunRay.direction;
            vec.y = 0;
            gunRay= new Ray(gunRay.origin, vec);

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
                fireTime = 100f;
                PlayAnimation("Body", "SpinExit", playbackRateParam, duration - earlyExitPercentTime);
            }
        }
        protected override void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            if (!launchList.Contains(item))
            {
                launchList.Add(item);
                float num = 1f;
                Vector3 forceVec;
                bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.5f;

                if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
                }
                if (body.characterMotor)
                {
                     if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)
                    {
                        num = body.characterMotor.mass;
                    }
                    else
                    {
                        num = 100;
                    }
                }
                else if (item.GetComponent<Rigidbody>())
                {
                    if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)
                    {
                        num = body.rigidbody.mass;
                    }
                    else
                    {
                        num = 50;
                    }
                }
                num = num * 16f;
                forceVec = GetAimRay().direction;
                forceVec.y = 1.5f;
                forceVec *= num;
                item.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
            }
        }
    }
}
