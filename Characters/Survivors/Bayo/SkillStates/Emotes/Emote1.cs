using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Emote1 : BaseEmote
    {
        private uint sound;
        private bool voiced = false;
        public override void OnEnter()
        {
            animString = "Taunt";
            animDuration = 6.56f;
            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > 0.4f && !voiced)
            {
                sound = AkSoundEngine.PostEvent(821863073, this.gameObject);
                voiced = true;
            }
        }
        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(sound);
            base.OnExit();
        }

    }
}
