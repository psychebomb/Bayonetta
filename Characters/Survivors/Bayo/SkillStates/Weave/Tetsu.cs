﻿using EntityStates;
using BayoMod.Modules.Components;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Weave
{
    public class Tetsu : GenericProjectileBaseState
    {
        public static float BaseDuration = 1.12f;

        public static float startDuration = 0.36f;
        public static float BaseDelayDuration = 0.15f;
        public string voiceString = "tetsu";

        public static float DamageCoefficient = 15f;
        public GameObject projpref = BayoAssets.fistFast;

        private bool ended = false;
        private bool cancel = false;
        private bool jumped = false;
        private bool noTarget = false;
        private RootMotionAccumulator rootMotionAccumulator;

        private BayoTracker tracker;
        protected HurtBox target;
        private bool targetIsValid;
        protected CameraTargetParams.AimRequest aimRequest;
        protected float fForce = 3500f;
        public override void OnEnter()
        {
            projectilePrefab = projpref;

            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "";

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = fForce;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            recoilAmplitude = 0.1f;
            bloom = 10;

            rootMotionAccumulator = GetModelRootMotionAccumulator();

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            characterDirection.forward = GetAimRay().direction;

            base.OnEnter();


            this.tracker = base.GetComponent<BayoTracker>();
            this.target = this.tracker.GetTrackingTarget();

            if (!this.tracker.GetTrackingTarget())
            {
                this.outer.SetNextStateToMain();
                noTarget = true;
                return;
            }

            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                this.targetIsValid = true;
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
        private void DetermineCancel()
        {
            if (inputBank)
            {
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }
                if (stopwatch >= startDuration)
                {
                    if (inputBank.skill1.down) cancel = true;
                    if (inputBank.skill2.down) cancel = true;
                    if (inputBank.skill3.down) cancel = true;
                    if (inputBank.moveVector != Vector3.zero) cancel = true;
                }
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            cancel = false;
            jumped = false;

            if (stopwatch >= startDuration && !ended)
            {
                ended = true;
                //PlayEndAnim();
            }

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (isAuthority && stopwatch >= baseDelayBeforeFiringProjectile)
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
            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextState(new Stance());
                return;
            }
            characterMotor.velocity.y = 0f;

            if (isAuthority && characterMotor)
            {
                inputBank.moveVector = Vector3.zero;
                characterMotor.moveDirection = Vector3.zero;
            }

            if (rootMotionAccumulator)
            {
                Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                {
                    base.characterMotor.rootMotion += vector;
                }
            }

        }

        public override void FireProjectile()
        {
            if (base.isAuthority && this.targetIsValid)
            {
                Util.PlaySound(voiceString, this.gameObject);
                Fire();
            }
        }

        public virtual void Fire()
        {
            Ray aimRay = GetAimRay();
            Vector3 dir = aimRay.direction;
            dir.y = 0.1f;
            Vector3 pos = this.target.transform.position;
            pos = pos - (dir.normalized * 1f);
            pos.y -= 3f;
            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
        public override void OnExit()
        {
            base.OnExit();
            if (this.tracker) Destroy(this.tracker);

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Body", "Tetsu", "Slash.playbackRate", startDuration * baseDuration);
            }
        }

        /*public virtual void PlayEndAnim()
        {
            if (GetModelAnimator())
            {
                PlayAnimation("Body", "TetsuEnd", "Slash.playbackRate", baseDuration - startDuration * baseDuration);
            }
        }
        */
    }
}