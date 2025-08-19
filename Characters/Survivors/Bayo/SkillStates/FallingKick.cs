using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Audio;
using EntityStates.Loader;
using EntityStates.Merc;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKick : BaseSkillState
    {
        public static float airControl = GroundSlam.airControl;
        public static float minimumDuration = GroundSlam.minimumDuration;
        public static string enterSoundString = GroundSlam.enterSoundString;
        public static float initialVerticalVelocity = GroundSlam.initialVerticalVelocity * 1.2f;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration *0.8f;
        private float previousAirControl;


        protected string hitboxGroupName = "FallGroup";
        protected string hitboxName = "FallHitbox";
        private OverlapAttack fallAttack;
        protected DamageType damageType = DamageType.Generic;
        protected float fallDamage = 2.5f;
        protected float fireAge = 0f;
        protected float procCoefficient = 1f;
        protected Vector3 downForce =  Vector3.down * 0.8f;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        protected string kickSoundString = "falling";
        protected string hitSoundString = "hit";
        protected float attackRecoil = 1f;
        protected float hitStopDuration = 0;
        protected float fireFreq = 0.25f;

        private bool hasFired;
        private float hitPauseTimer;
        protected bool inHitPause;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        protected string playbackRateParam = "Slash.playbackRate";

        protected readonly List<HealthComponent> results = new List<HealthComponent>();
        private bool launch = true;

        private GameObject swingEffectPrefab = BayoAssets.fall;
        private GameObject loopEffectInstance;
        private bool refunded = false;
        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            hasFired = false;
            fireFreq /= this.attackSpeedStat;
            SetupFallAttack();
            if (isAuthority)
            {
                characterMotor.velocity.y = initialVerticalVelocity;
            }
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = airControl;
            characterDirection.forward = GetAimRay().direction;
            PlayAnimation("Body", "FallKick");
            //Util.PlaySound(kickSoundString, gameObject);

            /*
            ChildLocator childLocator = GetModelChildLocator();
            if (childLocator)
            {
                Transform transform = childLocator.FindChild("SwingCenter") ?? base.characterBody.coreTransform;
                Quaternion rot = transform.rotation;
                if (transform)
                {
                    loopEffectInstance = Object.Instantiate(swingEffectPrefab, transform.position, rot);
                    //EffectManager.SpawnEffect(loopEffectPrefab, new EffectData
                    //{
                    //    origin = transform.position,
                    //    rotation = transform.rotation
                    //}, true);
                    loopEffectInstance.transform.parent = transform;
                }
            }
            */

        }

        private void SetupFallAttack()
        {
            //fallAttack = InitMeleeOverlap(fallDamage, GroundLight.comboHitEffectPrefab, GetModelTransform(), hitboxGroupName);
            //fallAttack.damageType = damageType;
            //fallAttack.procCoefficient = procCoefficient;
            //fallAttack.isCrit = RollCrit();
            //fallAttack.impactSound = impactSound;

            fallAttack = new OverlapAttack();
            fallAttack.damageType = damageType;
            fallAttack.attacker = gameObject;
            fallAttack.inflictor = gameObject;
            fallAttack.teamIndex = GetTeam();
            fallAttack.damage = fallDamage * damageStat;
            fallAttack.procCoefficient = procCoefficient;
            fallAttack.hitEffectPrefab = hitEffectPrefab;
            fallAttack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
            fallAttack.isCrit = RollCrit();
            fallAttack.impactSound = impactSound;
        }
        private void FireFallAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
                if (isAuthority)
                {
                    AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                }
            }
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                fallAttack.ResetIgnoredHealthComponents();
                hasFired = false;
                results.Clear();
            }
            if (isAuthority && fallAttack.Fire())
            {
                OnHitEnemyAuthority();

                /*
                if (NetworkServer.active && launch)
                {
                    TeamIndex team = GetTeam();

                    for (int i = 0; i < this.results.Count; i++)
                    {
                        HealthComponent item = this.results[i].healthComponent;
                        if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))) && item && item.transform)
                        {
                            ApplyForce(item);
                        }
                    }
                }
                */
            }
            if (NetworkServer.active)
            {
                Transform t = base.FindModelChild(this.hitboxName);
                Vector3 position = t.position;
                Vector3 vector = t.localScale * 0.5f;
                Quaternion rot = t.rotation;
                Collider[] hits = Physics.OverlapBox(position, vector, rot, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    HurtBox hurtBox = hits[i].GetComponent<HurtBox>();
                    if (hurtBox)
                    {
                        HealthComponent healthComponent = hurtBox.healthComponent;
                        if (healthComponent)
                        {
                            TeamIndex team = GetTeam();

                            if (!this.results.Contains(healthComponent))
                            {
                                results.Add(healthComponent);
                                HealthComponent item = healthComponent;
                                if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body")) && item && item.transform)
                                {
                                    ApplyForce(item);
                                }

                            }
                        }
                    }

                }

            }

        }

        protected bool CanDodge()
        {
            if (inputBank.skill3.down && skillLocator.utility && (!skillLocator.utility.mustKeyPress || !inputBank.skill3.hasPressBeenClaimed) && skillLocator.utility.ExecuteIfReady())
            {
                return true;
            }
            return false;
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

            if (isAuthority && characterMotor)
            {
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterDirection.forward = GetAimRay().direction;
                characterMotor.velocity.y += verticalAcceleration * Time.fixedDeltaTime;
                if (fixedAge >= minimumDuration && characterMotor.Motor.GroundingStatus.IsStableOnGround)
                {
                    outer.SetNextState(new FallingKickEnd());
                }
            }
            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                fireAge += Time.fixedDeltaTime;
            }
            else
            {
                hitPauseTimer -= Time.fixedDeltaTime;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            FireFallAttack();

        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            if (!refunded)
            {
                if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                {
                    base.skillLocator.secondary.rechargeStopwatch += 2.5f;
                }

                refunded = true;
            }

            ApplyHitstop();
        }

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
        }

        public override void OnExit()
        {
            characterMotor.airControl = previousAirControl;
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            results.Clear();
            /*
            if (loopEffectInstance)
            {
                Destroy(loopEffectInstance);
            }
            */
            base.OnExit();
        }

        private void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            float num = 100f;
            Vector3 forceVec;

            if (body.characterMotor)
            {
                if (!body.characterMotor.isGrounded)
                {
                    num = body.characterMotor.mass;
                    body.characterMotor.velocity.x = 0f;
                    body.characterMotor.velocity.z = 0f;
                    body.AddTimedBuff(BayoBuffs.punishable, 2f);
                    item.GetComponent<SetStateOnHurt>()?.SetStun(2f);
                }
            }
            else if (item.GetComponent<Rigidbody>())
            {
                num = body.rigidbody.mass;
                body.AddTimedBuff(BayoBuffs.punishable, 2f);
                item.GetComponent<SetStateOnHurt>()?.SetStun(2f);
            }
            num = num * 24f;
            forceVec = downForce * num;
            item.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
        }
    }
}
