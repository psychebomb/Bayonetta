using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Modules.Components;
using BayoMod.Survivors.Bayo;
using RoR2;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1_Alt
{
    public class PPK : BasePunch
    {
        protected Vector3 upForce = 12f * Vector3.up;
        private BayoTracker tracker;
        private HurtBox target;
        private Transform muzPos;
        private bool setTarget = false;
        public override void OnEnter()
        {
            if (characterMotor && characterMotor.isGrounded)
            {
                animStart = "ppk";
                animEnd = "ppke";
                earlyExitPercentTime = 0.65f;
                endDuration = 0.367f;
                //ReplacePrefab2(BayoAssets.p2s, BayoAssets.p2s2, BayoAssets.p2art);
                playSwing = 0.3f;
                attackEnd = 0.8f;
                animDur = 0.92f;
                aimMove = true;
                holdTime = 1f - earlyExitPercentTime;
            }
            else
            {
                animStart = "ppkA";
                animEnd = "ppkAE";
                animDur = 0.44f;
                earlyExitPercentTime = 0.44f;
                endDuration = 0.28f;
                //swingEffectPrefab = BayoAssets.p2as;
                playSwing = 0.24f;
                holdTime = 0.22f;
            }
            gunStr = "muzlf";
            voiceString = "pv4";
            swingSoundString = "p4";
            voice = true;
            hitStopDuration = 0.1f;

            tracker = this.gameObject.AddComponent<BayoTracker>();
            tracker.update = false;

            ModelLocator component = gameObject.GetComponent<ModelLocator>();
            ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex("muzlf");
                muzPos = component2.FindChild(childIndex);
            }

            hitboxGroupName = "OverheadGroup";
            hitboxName = "OverheadHitbox";

            base.OnEnter();

            //launch = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(target != null)
            {
                if (target.healthComponent.alive)
                {
                    Vector3 pos = target.transform.position;
                    Vector3 dir = muzPos.position - pos;
                    shootRay = new Ray(dir, muzPos.position);
                }
            }
        }
        protected override void ApplyForce()
        {
            CharacterBody body = item.body;
            float num = 1f;
            Vector3 forceVec;
            bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.5f;

            if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }
            if (body.characterMotor)
            {
                if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.characterMotor.mass < 300)
                {
                    num = body.characterMotor.mass;
                }
                else
                {
                    num = 100;
                }
                body.characterMotor.velocity.x = 0f;
                body.characterMotor.velocity.z = 0f;
            }
            else if (item.GetComponent<Rigidbody>())
            {
                if (body.HasBuff(BayoBuffs.wtDebuff) || healthCheck || body.rigidbody.mass < 300)
                {
                    num = body.rigidbody.mass / 2;
                }
                else
                {
                    num = 50;
                }

            }

            if(body.characterMotor && body.characterMotor.isGrounded)
            {
                forceVec = upForce * num;
            }
            else
            {
                forceVec = upForce * num * -1;
            }
            //if (body.HasBuff(BayoBuffs.wtDebuff)) forceVec *= 0.8f;
            item.GetComponent<SetStateOnHurt>()?.SetStun(1f);
            item.TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);

            if (!setTarget)
            {
                setTarget = true;
                tracker.SetTrackingTarget(itemHurt);
                target = tracker.GetTrackingTarget();
            }
        }

        //get rid of this once other moves are made
        public override void OnExit()
        {
            base.OnExit();

            if (this.tracker) Destroy(this.tracker);
        }
    }
}

