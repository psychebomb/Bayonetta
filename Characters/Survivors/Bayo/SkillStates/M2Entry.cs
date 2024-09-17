using EntityStates;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class M2Entry : BaseSkillState
    {

        protected float direction = 0f;
        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 moveVect = base.inputBank.moveVector;
            Vector3 aimDir = base.inputBank.aimDirection;
            Vector3 normalized = new Vector3(aimDir.x, 0f, aimDir.z).normalized;
            direction = Vector3.Dot(moveVect, normalized);
            
            if (characterMotor && characterMotor.isGrounded)
            {
                if (direction < -0.5f)
                {
                    outer.SetNextState(new RisingKick());

                }
                else
                {
                    outer.SetNextState(new RisingKick());
                }
            }
            else
            {
                if (direction < -0.5f)
                {
                    outer.SetNextState(new FallingKickStart());

                }
                else
                {
                    outer.SetNextState(new ABK());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }

}
