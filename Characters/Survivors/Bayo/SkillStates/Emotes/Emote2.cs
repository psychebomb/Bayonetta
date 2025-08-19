using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Emote2 : BaseEmote
    {
        private bool voiced = false;
        private bool shined = false;
        private float shineTime = 0.6f;
        public GameObject glint = BayoAssets.glintR;

        public override void OnEnter()
        {
            animString = "urhalo";
            animDuration = 1.973f;
            zoomDur = 0.75f;
            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > 0.33f && !voiced)
            {
                Util.PlaySound("ktaunt", this.gameObject);
                voiced = true;
            }

            if (fixedAge > shineTime && !shined)
            {
                if(glint && isAuthority)
                {
                    EffectManager.SimpleMuzzleFlash(glint, gameObject, "glint", true);
                }
                shined = true;
            }
        }

    }
}
