using BayoMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using BayoMod.Survivors.Bayo;


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
            damageCoefficient = 3.95f;
            procCoefficient = 1f;
            damageType = DamageType.Stun1s;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            forwardDir = GetAimRay().direction;
            saveRay = GetAimRay();
            //bonusForce = 0.8f * forwardDir * Uppercut.upwardForceStrength;

            characterDirection.forward = forwardDir;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            if (forwardDir.y < -0.33)
            {
                kickSpeed = new AnimationCurve(new Keyframe[]
                {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.2f, 7f),
                new Keyframe(0.5f, 7f),
                });
            }
            else
            {
                kickSpeed = new AnimationCurve(new Keyframe[]
                {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.2f, 7f),
                new Keyframe(0.5f, 7f),
                new Keyframe(0.75f, 1.5f)
                });
                launch = true;
            }

            PlayAnimation("Body", "Abk", playbackRateParam, duration);
            characterMotor.Motor.ForceUnground();
            exitToStance = false;
            hasExtended = false;

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
            if (forwardDir.y > -0.33) characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            LastHit();
            base.OnExit();
        }

        protected override void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            float num = 1f;
            //Vector3 forceVec;

            if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }
            if (body.characterMotor &&((base.characterBody.HasBuff(BayoBuffs.wtBuff) || body.characterMotor.mass < 300)))
            {
                if (!launchList.Contains(item))
                {
                    launchList.Add(item);
                    float dist = Vector3.Distance(base.characterBody.transform.position, body.transform.position);
                    if (dist > 2.5f)
                    {
                        body.characterMotor.rootMotion += speedVec.normalized * -1 * (dist - 2.5f);
                    }
                }
                else
                {
                    Vector3 realSpeed = speedVec * 0.9f;
                    //if (base.characterBody.HasBuff(BayoBuffs.wtBuff)) realSpeed /= 2f;
                    body.characterMotor.velocity = realSpeed;
                }
            }
            //num = num / 150f;
            //forceVec = speedVec.normalized;
            //forceVec.y += 30f;
            //forceVec = forceVec * num; //Mathf.Lerp(1f, 0f, fixedAge / duration) * moveSpeedStat;
            //item.healthComponent.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
        }

        private void LastHit()
        {
            int num = launchList.Count;
            TeamIndex team = GetTeam();

            for (int i = 0; i < num; ++i)
            {
                HealthComponent item = launchList[i];
                if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))) && item && item.transform)
                {
                    CharacterBody body = item.body;
                    if (body.characterMotor)
                    {
                        body.characterMotor.velocity = speedVec.normalized * (1.5f * 0.9f);
                    }
                }
            }
        }
    }
}
