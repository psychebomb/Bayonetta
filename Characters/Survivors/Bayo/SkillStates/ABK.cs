using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using BayoMod.Characters.Survivors.Bayo.Components;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class ABK : BaseMeleeAttack
    {
        private Vector3 forwardDir;
        private bool hasExtended;
        private string animName;
        protected AnimationCurve kickSpeed;
        protected Vector3 speedVec;
        protected Ray saveRay;
        private ABKRotator abkr;
        private float rotTime = 0.18f;
        private bool rotated = false;
        private bool down = false;
        public override void OnEnter()
        {
            duration = 0.65f;
            attackStartPercentTime = 0.05f;
            attackEndPercentTime = 1f;
            damageCoefficient = 3.75f;
            procCoefficient = 1f;
            damageType = DamageType.Stun1s;

            hitStopDuration = 0.1f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            forwardDir = GetAimRay().direction;
            saveRay = GetAimRay();
            voice = true;
            voiceString = "stompabk";
            Util.PlaySound("abk", gameObject);
            ReplacePrefabs(BayoAssets.abk, BayoAssets.abk2);
            muzzleString = "ABKC";
            playSwing = 0.15f;
            shootRay = GetAimRay();
            gunName = "muzrf";
            gunDamage = 0.75f;
            m2Refund = true;
            //fireTime = 0.15f;
            //bonusForce = 0.8f * forwardDir * Uppercut.upwardForceStrength;

            characterDirection.forward = forwardDir;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            if (forwardDir.y <= -0.5)
            {
                kickSpeed = new AnimationCurve(new Keyframe[]
                {
                new Keyframe(0f, 1.5f),
                new Keyframe(0.2f, 7f),
                new Keyframe(0.5f, 7f),
                });
                launch = false;
                down = true;
                animName = "AbkDown";
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
                animName = "Abk";
            }

            abkr = GetComponent<ABKRotator>();
            PlayAnimation("Body", animName, playbackRateParam, duration);
            characterMotor.Motor.ForceUnground();
            exitToStance = false;
            hasExtended = false;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            shootRay = GetAimRay();

            if (base.inputBank.skill2.down && isAuthority)
            {
                fireTime = 0.15f / this.attackSpeedStat;
            }
            else
            {
                fireTime = 9999f;
            }

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (stopwatch >= 0.1f && down)
            {
                if (inputBank.jump.justPressed && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
                {
                    inputBank.jump.PushState(false);
                    outer.SetNextStateToMain();
                    return;
                }
            }

            if (characterDirection) characterDirection.forward = forwardDir;
            if (!inHitPause && characterMotor)
            {
                float num = kickSpeed.Evaluate(stopwatch / duration);
                speedVec = forwardDir * num * moveSpeedStat;
                characterMotor.velocity = speedVec;

            }

            if(stopwatch >= rotTime && !rotated)
            {
                abkr.rotate = true;
                abkr.rVFX = true;
                abkr.lookDir = forwardDir;
                rotated = true;
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
            abkr.rVFX = false;
            if (!hasExtended)
            {
                PlayAnimation("Body", "AbkExit");
                abkr.lookDir = Vector3.zero;
                abkr.rotate = false;
            }
            if (!down) characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            LastHit();
            base.OnExit();
        }

        protected override void FireAttack()
        {
            base.FireAttack();

            if (launch)
            {
                TeamIndex team = GetTeam();

                for (int i = 0; i < this.results.Count; i++)
                {
                    item = this.results[i];
                    if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body")) && item && item.transform)
                    {
                        ApplyForce2();
                    }
                }
            }
        }

        protected override void ApplyForce()
        {
            CharacterBody body = item.body;
            bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.5f;
            if (body)
            {
                if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
                }
                if (body.characterMotor && ((body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)))
                {
                    //Chat.AddMessage(body.name);
                    float dist = Vector3.Distance(base.characterBody.transform.position, body.transform.position);
                    if (dist > 2.5f)
                    {
                        body.characterMotor.rootMotion += speedVec.normalized * -1 * (dist - 2.5f);
                    }
                }
                else if (body.rigidbody && ((body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.rigidbody.mass < 300)))
                {
                    float dist = Vector3.Distance(base.characterBody.transform.position, body.transform.position);
                    if (dist > 2.5f)
                    {
                        body.rigidbody.position += speedVec.normalized * -1 * (dist - 2.5f);
                    }
                }
            }
        }

        private void ApplyForce2()
        {
            CharacterBody body = item.body;
            bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.5f;
            if (body)
            {
                if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
                }
                if (body.characterMotor && ((body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)))
                {
                    Vector3 realSpeed = speedVec * 0.9f;
                    body.characterMotor.velocity = realSpeed;
                }
                else if (body.rigidbody && ((body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.rigidbody.mass < 300)))
                {
                    Vector3 realSpeed = speedVec * 0.9f;
                    body.rigidbody.velocity = realSpeed;
                    if (item.GetComponent<RigidbodyMotor>())
                    {
                        item.GetComponent<RigidbodyMotor>().canTakeImpactDamage = false;

                    }
                }
            }
        }

        private void LastHit()
        {
            int num = results.Count;
            TeamIndex team = GetTeam();

            for (int i = 0; i < num; ++i)
            {
                HealthComponent item = results[i];
                if (item && item.transform && FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))))
                {
                    CharacterBody body = item.body;
                    if (body && body.characterMotor)
                    {
                        body.characterMotor.velocity = speedVec.normalized * (1.5f * 0.9f);
                    }
                    else if (body && body.rigidbody)
                    {
                        body.rigidbody.velocity = speedVec.normalized * (1.5f * 0.9f);
                        //body.rigidbody.detectCollisions = true;
                        if (item.GetComponent<RigidbodyMotor>())
                        {
                            item.GetComponent<RigidbodyMotor>().canTakeImpactDamage = true;

                        }
                    }
                }
            }
        }
    }
}
