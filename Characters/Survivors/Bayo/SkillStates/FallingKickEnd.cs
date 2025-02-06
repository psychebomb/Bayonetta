using UnityEngine;
using EntityStates.Merc;
using EntityStates;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKickEnd : HeelKick
    {
        public override void OnEnter()
        {
            swing = "land";
            base.OnEnter();
            duration = 0.85f;
            upForce = 5 * Vector3.up;
            attackStartPercentTime = 0f;
            earlyExit = 0.2f;
            hitboxGroupName = "FallHitbox";
            PlayAnimation("Body", "FallKickExit", "Slash.playbackRate", duration);

        }
    }
}
