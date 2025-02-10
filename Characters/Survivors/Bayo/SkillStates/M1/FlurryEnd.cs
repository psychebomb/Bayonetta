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
using System.Collections.Generic;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

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
        private float weaveDamage = 14f;
        private float weaveForce = 3000f;
        private bool firedProjectile = false;
        private float recoilAmplitude = 0.1f;
        private float bloom = 10;
        private bool hasEnded = false;
        private Vector3 dir;

        public override void OnEnter()
        {
            attackStartPercentTime = 0.25f;
            attackEndPercentTime = 0.5f;

            damageCoefficient = 2f;
            procCoefficient = 1f;
            damageType = DamageTypeCombo.GenericPrimary;
            pushForce = 0f;
            hitStopDuration = 0.05f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            characterMotor.velocity.y = 0f;
            exitToStance = true;
            voice = true;
            voiceString = "flurryend";

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();

            base.OnEnter();

            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            duration = 1.92f / this.attackSpeedStat;
            earlyExit = 1f / this.attackSpeedStat;

            if (characterMotor.isGrounded)
            {
                animName = "FlurryE";
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
            else
            {
                animName = "FlurryAE";
                characterMotor.airControl = characterMotor.airControl;
                exitToStance = false;
            }

            PlayAnimation("Body", animName, "Slash.playbackRate", duration);


        }

        private void DetermineCancel()
        {

            if (inputBank)
            {
                if (hasEnded)
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

            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    inputBank.moveVector = Vector3.zero;
                    characterMotor.moveDirection = Vector3.zero;
                    characterMotor.velocity = characterMotor.velocity * 0f;
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
                
                if (rootMotionAccumulator)
                {
                    Vector3 vector = rootMotionAccumulator.ExtractRootMotion();
                    if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
                    {
                        base.characterMotor.rootMotion += vector;
                    }
                }
                
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterMotor.velocity.y = 0;
            }

            if (stopwatch >= duration*attackStartPercentTime)
            {
                if (!firedProjectile)
                {
                    firedProjectile = true;
                    FireProjectile();
                    DoFireEffects();
                }

                characterDirection.forward = dir;
                characterDirection.moveVector = dir;
            }
            else
            {
                characterDirection.forward = GetAimRay().direction;
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
                if (!hasEnded)
                {
                    hasEnded = true;
                }

            }

            base.FixedUpdate();


        }
        protected void DoFireEffects()
        {
            Util.PlaySound("weave", base.gameObject);
            AddRecoil(-2f * recoilAmplitude, -3f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            base.characterBody.AddSpreadBloom(bloom);
        }

        protected void FireProjectile()
        {
            if (base.isAuthority)
            {
                dir = GetAimRay().direction;
                Vector3 pos = this.gameObject.transform.position;
                pos.y -= 1f;
                dir.y = 0f;
                pos = pos + (dir.normalized * 2.5f);
                dir.y = 0.1f;
                ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * weaveDamage, weaveForce, Util.CheckRoll(critStat, base.characterBody.master), DamageColorIndex.Default, null, -1,DamageTypeCombo.GenericPrimary);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        
    }
}
