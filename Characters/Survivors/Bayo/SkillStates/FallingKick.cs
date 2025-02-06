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
        public static float initialVerticalVelocity = GroundSlam.initialVerticalVelocity;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration *0.8f;
        private float previousAirControl;


        protected string hitboxGroupName = "FallGroup";
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

        private readonly List<HealthComponent> launchList = new List<HealthComponent>();
        private bool launch = true;
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
                launchList.Clear();
                hasFired = false;
            }
            if (isAuthority && fallAttack.Fire())
            {
                OnHitEnemyAuthority();
            }
            if (NetworkServer.active && launch)
            {
                int num = fallAttack.ignoredHealthComponentList.Count;
                TeamIndex team = GetTeam();

                for (int i = 0; i < num; ++i)
                {
                    HealthComponent item = fallAttack.ignoredHealthComponentList[i];
                    if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))) && item && item.transform)
                    {
                        ApplyForce(item);
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
            launchList.Clear();
            base.OnExit();
        }

        private void ApplyForce(HealthComponent item)
        {
            CharacterBody body = item.body;
            if (!launchList.Contains(item))
            {
                launchList.Add(item);
                float num = 1f;
                Vector3 forceVec;

                if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
                }
                if (body.characterMotor)
                {
                    num = body.characterMotor.mass;
                    body.characterMotor.velocity.x = 0f;
                    body.characterMotor.velocity.z = 0f;
                }
                else if (item.GetComponent<Rigidbody>())
                {
                    num = body.rigidbody.mass;
                }
                num = num * 24f;
                forceVec = downForce * num;
                item.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
            }
        }
    }
}
