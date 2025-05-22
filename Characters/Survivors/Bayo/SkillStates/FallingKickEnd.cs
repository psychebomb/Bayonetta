using UnityEngine;
using EntityStates.Merc;
using EntityStates;
using RoR2;

namespace BayoMod.Survivors.Bayo.SkillStates
{
    public class FallingKickEnd : HeelKick
    {
        public override void OnEnter()
        {
            swing = "land";
            sEffect = BayoAssets.slam;
            if (isAuthority) EffectManager.SimpleMuzzleFlash(BayoAssets.falle, gameObject, muzzleString, true);
            base.OnEnter();
            duration = 0.85f;
            upForce = 8 * Vector3.up;
            attackStartPercentTime = 0f;
            earlyExit = 0.2f;
            hitboxGroupName = "FallHitbox";
            PlayAnimation("Body", "FallKickExit", "Slash.playbackRate", duration);

        }
    }
}
