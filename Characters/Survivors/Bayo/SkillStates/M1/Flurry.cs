using BayoMod.Survivors.Bayo.SkillStates;
using RoR2;
using UnityEngine;
using EntityStates.Loader;
using UnityEngine.UIElements;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Flurry : BaseMeleeAttack
    {
        protected float fireAge;
        protected float fireFreq;
        protected float animTime;
        protected float myDuration;
        private string animName;
        public static float verticalAcceleration = GroundSlam.verticalAcceleration * 0.2f;
        protected float hopVelocity = 1.25f;
        protected bool flip = false;
        public override void OnEnter()
        {
            attackStartPercentTime = 0f;
            attackEndPercentTime = 1f;
            earlyExitPercentTime = 0.3f;

            damageCoefficient = 1.35f;
            procCoefficient = 0.75f;
            damageType = DamageTypeCombo.GenericPrimary;
            pushForce = 300f;
            hitStopDuration = 0.012f;
            attackRecoil = 1f;
            hitHopVelocity = 4f;
            exitToStance = false;
            fireAge = 0f;
            shootRay = GetAimRay();
            gunName = "muzrh";
            gunDamage = 0.1f;
            launch = false;
            fireTime = 0.15f;

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }

            Util.PlaySound("flurry", this.gameObject);
            Util.PlaySound("falling", this.gameObject);

            base.OnEnter();

            if (characterMotor.isGrounded)
            {
                animName = "Flurry";
            }
            else
            {
                animName = "FlurryA";
                characterMotor.airControl = characterMotor.airControl;
                launch = true;
                juggleHop = 2.5f / this.attackSpeedStat;
            }

            myDuration = 2.666f / this.attackSpeedStat;
            duration = 3f / this.attackSpeedStat;
            durOverride = true;
            fireFreq = 0.2f / this.attackSpeedStat;
            animTime = 0.9333f / this.attackSpeedStat;

            characterDirection.forward = GetAimRay().direction;
            PlayAnimation("Body", animName, playbackRateParam, animTime);

        }

        protected override void FireAttack()
        {
            if (fireAge >= fireFreq)
            {
                fireAge = 0f;
                attack.ResetIgnoredHealthComponents();
                //attack.Fire();
                hasFired = false;
                results.Clear();
                if (characterMotor && !characterMotor.isGrounded && hopVelocity > 0f)
                {
                    SmallHop(characterMotor, hopVelocity);
                }

                if (!flip)
                {
                    flip = true;
                    gunName = "muzlh";
                }
                else
                {
                    flip = false;
                    gunName = "muzrh";
                }
            }
            base.FireAttack();
        }

        public override void FixedUpdate()
        {

            if (CanDodge())
            {
                //outer.SetNextState(new Dodge { currentSwing = 4 });
                //inputBank.skill3.hasPressBeenClaimed = true;
                //return;
            }

            characterDirection.forward = GetAimRay().direction;
            shootRay = GetAimRay();

            if (characterMotor.isGrounded)
            {
                if (isAuthority && characterMotor)
                {
                    inputBank.moveVector = Vector3.zero;
                    characterMotor.moveDirection = Vector3.zero;
                }
                if (characterMotor && characterDirection)
                {
                    characterMotor.velocity = characterMotor.velocity * 0f;
                }
            }
            else
            {
                base.characterMotor.rootMotion = Vector3.zero;
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;
            }

            fireAge += Time.fixedDeltaTime;

            if (isAuthority && stopwatch >= earlyExitPercentTime)
            {
                if (!inputBank.skill1.down || stopwatch >= myDuration)
                {
                    outer.SetNextState(new FlurryEnd());
                    return;
                }
            }

            base.FixedUpdate();

        }

        private void LastHit()
        {
            int num = results.Count;
            TeamIndex team = GetTeam();

            for (int i = 0; i < num; ++i)
            {
                HealthComponent item = results[i];
                if (FriendlyFireManager.ShouldDirectHitProceed(item, team) && (!item.body.isChampion || (item.gameObject.name.Contains("Brother") && item.gameObject.name.Contains("Body"))) && item && item.transform)
                {
                    CharacterBody body = item.body;
                    if (body.characterMotor && !body.characterMotor.isGrounded)
                    {
                        juggleHop = 9f / this.attackSpeedStat;
                        if (base.characterBody.HasBuff(BayoBuffs.wtBuff)) juggleHop /= 3f;
                        SmallHop(body.characterMotor, juggleHop);
                        body.characterMotor.velocity.x = 0f;
                        body.characterMotor.velocity.z = 0f;
                        item.GetComponent<SetStateOnHurt>()?.SetStun(1f);
                    }
                }
            }
        }
        public override void OnExit()
        {
            if (launch) LastHit();
            base.OnExit();
        }
    }
}