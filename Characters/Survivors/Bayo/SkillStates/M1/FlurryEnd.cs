using BayoMod.Modules.BaseStates;
using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Merc;
using EntityStates.Loader;
using EntityStates;
using RoR2.Projectile;
using BayoMod.Survivors.Bayo;
using static UnityEngine.ParticleSystem.PlaybackState;
using EntityStates.ImpBossMonster;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class FlurryEnd : BaseMeleeAttack
    {

        private RootMotionAccumulator rootMotionAccumulator;
        private bool cancel;
        private bool jumped;
        private float earlyExit;
        private string animName;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;

        private GameObject projectilePrefab = BayoAssets.fistProjectilePrefab;
        private float weaveDamage = 12f;
        private float weaveForce = 3000f;
        private bool firedProjectile = false;
        private float recoilAmplitude = 0.1f;
        private float bloom = 10;

        public override void OnEnter()
        {

            duration = 1.92f;
            attackStartPercentTime = 0.25f;
            attackEndPercentTime = 0.5f;
            earlyExit = 1f;

            damageCoefficient = 3f;
            procCoefficient = 1f;
            damageType = DamageType.Generic;
            pushForce = 0f;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            characterMotor.velocity.y = 0f;
            exitToStance = true;
            if (characterMotor.isGrounded)
            {
                animName = "FlurryE";
            }
            else
            {
                animName = "FlurryAE";
                characterMotor.airControl = characterMotor.airControl;
            }

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();
            PlayAnimation("Body", animName, "Slash.playbackRate", duration);

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            base.OnEnter();

            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
                if (stopwatch >= duration * attackEndPercentTime + 0.012)
                {
                    if (inputBank.skill2.down) cancel = true;
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

        public override void FixedUpdate()
        {
            cancel = false;
            jumped = false;
            if (stopwatch >= duration * attackEndPercentTime)
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
                cancel = true;
                outer.SetNextState(new Dodge { currentSwing = -1 });
                inputBank.skill3.hasPressBeenClaimed = true;
                return;
            }

            characterDirection.forward = GetAimRay().direction;

            if (characterMotor.isGrounded)
            {
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
            else
            {
                rootMotionAccumulator.accumulatedRootMotion = Vector3.zero;
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterMotor.velocity.y = Mathf.Lerp(0f, -20f, fixedAge / duration);
            }

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            if (stopwatch >= duration*attackStartPercentTime && !firedProjectile)
            {
                firedProjectile = true;
                FireProjectile();
                DoFireEffects();
            }

            if (isAuthority && (stopwatch >= earlyExit))
            {
                if (inputBank.skill1.down)
                {
                    outer.SetNextState(new Punch1
                    {
                        swingIndex = 0
                    });
                    return;
                }

            }

            base.FixedUpdate();


        }
        protected virtual void DoFireEffects()
        {
            Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
            AddRecoil(-2f * recoilAmplitude, -3f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            base.characterBody.AddSpreadBloom(bloom);
        }

        protected virtual void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = GetAimRay();
                Vector3 dir = aimRay.direction;
                dir.y = 0f;
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * weaveDamage, weaveForce, Util.CheckRoll(critStat, base.characterBody.master));
            }
        }
    }
}
