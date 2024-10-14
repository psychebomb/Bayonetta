using EntityStates;
using BayoMod.Modules.Components;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates.ImpBossMonster;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Weave
{
    public class Tetsu : GenericProjectileBaseState
    {
        public static float BaseDuration = 1.12f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0

        public static float startDuration = 0.4f;
        public static float BaseDelayDuration = 0.15f;

        public static float DamageCoefficient = 9f;

        private bool ended = false;
        private bool cancel = false;
        private bool jumped = false;
        private bool noTarget = false;
        private RootMotionAccumulator rootMotionAccumulator;

        private BayoTracker tracker;
        protected HurtBox target;
        private bool targetIsValid;
        protected CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            projectilePrefab = BayoAssets.fistProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = SpawnState.spawnSoundString;

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 3000f;

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
                if (inputBank.skill1.down) cancel = true;
                if (inputBank.skill2.down) cancel = true;
                if (inputBank.skill3.down) cancel = true;
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }

                if (inputBank.moveVector != Vector3.zero) cancel = true;
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
                PlayEndAnim();
            }

            if (CanDodge())
            {
                outer.SetNextState(new Dodge());
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            if (isAuthority && stopwatch >= startDuration)
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
                Fire();
            }
        }

        public virtual void Fire()
        {
            Ray aimRay = GetAimRay();
            Vector3 dir = aimRay.direction;
            dir.y = 0f;
            Vector3 pos = this.target.transform.position;
            pos = pos - (dir.normalized * 3);
            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.tracker && !noTarget) Destroy(this.tracker);

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Body", "Tetsu", "Roll.playbackRate", startDuration);
            }
        }

        public virtual void PlayEndAnim()
        {
            if (GetModelAnimator())
            {
                PlayAnimation("Body", "TetsuEnd", "Roll.playbackRate", duration - startDuration);
            }
        }
    }
}
