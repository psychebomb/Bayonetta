using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using RoR2.Projectile;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Bayo;

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

        public GameObject projectilePrefab = BayoAssets.fistProjectilePrefab;
        public float weaveDamage = 12.5f;
        public float weaveForce = 3000f;
        private bool firedProjectile = false;
        private float recoilAmplitude = 0.1f;
        private float bloom = 10;
        private bool hasEnded = false;
        protected Vector3 dir;
        private float fireProj;
        private bool actuallyFired = false;
        private bool fast = true;

        protected float attackStart = 0.25f;
        protected float attackEnd = 0.5f;
        protected string voiceStr = "flurryend";
        protected float dur = 1.92f;
        protected float animDur = 1.92f;
        protected float exit = 1f;
        protected float rootMult = 1f;
        protected bool useSlowPrefabs = true;
        protected GameObject regProj = BayoAssets.fistProjectilePrefab;
        protected GameObject fastProj = BayoAssets.fistFast;
        protected string animGroundName = "FlurryE";
        protected string animAirName = "FlurryAE";

        public int curSwing = -1;
        public int altSwing = -1;
        public bool inAltPath = false;

        private bool usingArtSkin = false;
        public GameObject spawnEffect = BayoArtVFX.fireFistSpawn;

        CharacterCameraParams camParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        private float transDur = 0.9f;
        public float camX = 0f;
        public float camY = 0.5f;
        public float camZ = -14f;
        public float camTransIn = 0.3f;
        public float camTransOut = 0.3f;

        public override void OnEnter()
        {
            attackStartPercentTime = attackStart;
            attackEndPercentTime = attackEnd;

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
            voiceString = voiceStr;

            characterDirection.forward = GetAimRay().direction;
            rootMotionAccumulator = GetModelRootMotionAccumulator();

            GameObject dam = BayoAssets.sum;
            SkinDef curSkin = SkinCatalog.FindCurrentSkinDefForBodyInstance(this.characterBody.gameObject);
            if (curSkin == BayoSurvivor.artSkin)
            {
                usingArtSkin = true;
            }
            else
            {
                if (isAuthority) EffectManager.SimpleMuzzleFlash(dam, this.gameObject, "DamageCenter", true);
                Util.PlaySound("portalsum", this.gameObject);
            }

            base.OnEnter();

            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
            
            duration = dur / this.attackSpeedStat;
            earlyExit = exit / this.attackSpeedStat;
            camTransIn /= this.attackSpeedStat;
            camTransOut /= this.attackSpeedStat;
            SetCamera();

            projectilePrefab = fastProj;
            fireProj = (duration * attackStartPercentTime) - 0.24f;
            if(fireProj >= 0f && useSlowPrefabs)
            {
                projectilePrefab = regProj;
                fast = true;
            }
            else
            {
                fireProj = duration * attackStartPercentTime;
            }

            if (characterMotor.isGrounded)
            {
                animName = animGroundName;
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
            else
            {
                animName = animAirName;
                characterMotor.airControl = characterMotor.airControl;
                exitToStance = false;
            }

            PlayAnimation("Body", animName, "Slash.playbackRate", animDur);

            if (usingArtSkin)
            {
                projectilePrefab = BayoArtVFX.fireFist;
                fast = true;
                fireProj = duration * attackStartPercentTime;
            }

             if (characterBody && characterBody.isSprinting) characterBody.isSprinting = false;

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
                outer.SetNextState(new Dodge
                {
                    currentSwing = curSwing,
                    inAltPath = inAltPath,
                    altSwing = altSwing
                });
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
                        base.characterMotor.rootMotion += (vector * rootMult);
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
                        base.characterMotor.rootMotion += (vector * rootMult);
                    }
                }
                
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
                characterMotor.velocity.y = 0;
            }

            if (stopwatch >= fireProj)
            {
                if (!firedProjectile)
                {
                    firedProjectile = true;
                    FireProjectile();
                }

                characterDirection.forward = dir;
                characterDirection.moveVector = dir;
            }
            else
            {
                characterDirection.forward = GetAimRay().direction;
            }

            if (((stopwatch >= fireProj + 0.24f)|| (fast && stopwatch >= fireProj)) && !actuallyFired)
            {
                actuallyFired = true;
                DoFireEffects();
            }

            if (isAuthority && (stopwatch >= earlyExit))
            {
                if (inputBank.skill1.down)
                {
                    SetStep();
                    return;
                }
                if (!hasEnded)
                {
                    hasEnded = true;
                }

            }

            base.FixedUpdate();

        }

        protected virtual void SetStep()
        {
            outer.SetNextState(new Punch1
            {
                swingIndex = 0
            });
        }
        protected void DoFireEffects()
        {
            AddRecoil(-2f * recoilAmplitude, -3f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            base.characterBody.AddSpreadBloom(bloom);
        }

        protected virtual void FireProjectile()
        {
            dir = GetAimRay().direction;

            Vector3 pos = this.gameObject.transform.position;
            pos.y -= 0.5f;
            dir.y = 0f;
            pos = pos + (dir.normalized * 2.5f);

            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * weaveDamage, weaveForce, Util.CheckRoll(critStat, base.characterBody.master), DamageColorIndex.Default, null, -1,DamageTypeCombo.GenericPrimary);
            }
            if (usingArtSkin)
            {
                pos = pos - (dir.normalized * 4f);
                pos.y += 3.75f;
                EffectManager.SimpleEffect(spawnEffect, pos, Util.QuaternionSafeLookRotation(dir), true);
            }
        }
        private void SetCamera()
        {
            camParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            camParams.name = "FinisherPunch";
            camParams.data.wallCushion = 0.1f;
            camParams.data.idealLocalCameraPos = new Vector3(camX, camY, camZ);

            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = camParams.data,
                    priority = 1f
                }, camTransIn);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, camTransOut);
            }
        }

        
    }
}
