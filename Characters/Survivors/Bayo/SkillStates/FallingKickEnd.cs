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
            upForce = 0.2f * Vector3.up * Uppercut.upwardForceStrength;
            attackStartPercentTime = 0f;
            hitboxGroupName = "FallHitbox";
            PlayAnimation("Body", "FallKickExit", "Emote.playbackRate", duration);

        }
    }
}
