using EntityStates;
using BayoMod.Survivors.Bayo;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.UIElements;
using EntityStates.ImpBossMonster;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class TetsuProto : GenericProjectileBaseState
    {
        public static float BaseDuration = 1.12f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0

        public static float startDuration = 0.4f;
        public static float BaseDelayDuration = 0.2f;

        public static float DamageCoefficient = 10f;

        private bool ended = false;
        private bool cancel = false;
        private bool jumped = false;
        private RootMotionAccumulator rootMotionAccumulator;

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
            force = 5000f;

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
                PlayAnimation("Body", "TetsuEnd", "Roll.playbackRate", duration - startDuration);
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
    }
}