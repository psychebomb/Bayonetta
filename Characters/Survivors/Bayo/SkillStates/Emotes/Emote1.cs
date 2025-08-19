using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class Emote1 : BaseEmote
    {
        private uint sound;
        private bool voiced = false;

        public Modules.Config.LongTaunt voiceOption;
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
                PlayTauntSound();
                voiced = true;
            }
        }

        private void PlayTauntSound()
        {
            voiceOption = Modules.Config.longTaunt.Value;

            SkinDef curSkin = SkinCatalog.FindCurrentSkinDefForBodyInstance(this.characterBody.gameObject);

            switch (voiceOption)
            {
                case Modules.Config.LongTaunt.Bayo_1_Only:
                    sound = AkSoundEngine.PostEvent(821863073, this.gameObject);
                    break;
                case Modules.Config.LongTaunt.Bayo_2_Only:
                    sound = AkSoundEngine.PostEvent(2830927257, this.gameObject);
                    break;
                case Modules.Config.LongTaunt.Random:
                    sound = AkSoundEngine.PostEvent(2054889326, this.gameObject);
                    break;
                case Modules.Config.LongTaunt.Based_On_Skin_Choice:
                    if(curSkin && curSkin == BayoSurvivor.masterySkin)
                    {
                        sound = AkSoundEngine.PostEvent(2830927257, this.gameObject);
                    }
                    else
                    {
                        sound = AkSoundEngine.PostEvent(821863073, this.gameObject);

                    }
                    break;
            }
        }
        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(sound);
            base.OnExit();
        }

    }
}
