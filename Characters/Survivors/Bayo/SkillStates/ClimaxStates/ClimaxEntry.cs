using EntityStates;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine;
using BayoMod.Modules.Components;
using RoR2;
using UnityEngine.Networking;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates
{
    public class ClimaxEntry : BaseSkillState
    {

        private PunishTracker tracker = null;
        protected HurtBox target;
        protected CharacterBody body;
        public override void OnEnter()
        {
            base.OnEnter();

            if (base.GetComponent<ClimaxTracker>() != null)
            {
                this.tracker = base.GetComponent<ClimaxTracker>();
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }
            this.target = this.tracker.GetTrackingTarget();


            if (!this.tracker.GetTrackingTarget())
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                if (this.target.healthComponent.body)
                {
                    body = this.target.healthComponent.body;
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.body.gameObject.GetComponent<SphereCollider>())
            {
                this.outer.SetNextStateToMain();
                return;
            }

            //Destroy(tracker);

            if (base.characterMotor.isGrounded)
            {
                if ((this.target.healthComponent.body.characterMotor && !this.target.healthComponent.body.characterMotor.isGrounded))
                {
                    // Chat.AddMessage("flying/airborne");
                    this.tracker.punishing = true;
                    outer.SetNextState(new SummonGom());
                }
                else if (this.target.healthComponent.body.characterMotor)
                {
                    if (this.target.healthComponent.body.characterMotor.mass >= 300 || (this.modelLocator && this.modelLocator.gameObject.name == "VultureBody(Clone)"))
                    {
                        //outer.SetNextState(new SmackStart());
                        // Chat.AddMessage("enemy large");
                        this.tracker.punishing = true;
                        outer.SetNextState(new SummonGom());
                    }
                    else
                    {
                        //Chat.AddMessage("petite");
                        this.tracker.punishing = true;
                        outer.SetNextState(new SummonGom());
                    }
                }
                else if (this.target.healthComponent.GetComponent<Rigidbody>())
                {
                    //Chat.AddMessage("flying/airborne");
                    this.tracker.punishing = true;
                    outer.SetNextState(new SummonGom());
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }

}
