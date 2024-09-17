using UnityEngine;
using EntityStates.Merc;
using EntityStates;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKickEnd : HeelKick
    {
        public override void OnEnter()
        {
            base.OnEnter();
            duration = 0.85f;
            earlyEnd = 0.85f;
            upForce = 0.4f * Vector3.up * Uppercut.upwardForceStrength;
            attackStartTime = 0f;
            hitboxGroupName = "FallHitbox";
            PlayAnimation("Body", "FallKickExit", "Emote.playbackRate", duration);

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            inputBank.moveVector = Vector3.zero;
            characterMotor.moveDirection = Vector3.zero;

            if (stopwatch >= earlyEnd && !inputBank.skill2.down)
            {
                outer.SetNextState(new Stance());
                return;
            }

        }
    }
}
