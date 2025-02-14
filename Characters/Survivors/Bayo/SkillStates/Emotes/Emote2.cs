using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using RoR2;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Emote2 : BaseEmote
    {
        private bool voiced = false;
        public override void OnEnter()
        {
            animString = "urhalo";
            animDuration = 1.96f;
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
        }

    }
}
