using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using static R2API.SoundAPI;
using RoR2.ConVar;
using BayoMod.Survivors.Bayo.Components;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class LetsDance : BaseEmote
    {
        private uint sound;
        private bool music;
        BaseConVar convar;
        private string oldMusic;
        BayoWeaponComponent bwc;
        public override void OnEnter()
        {
            animString = "Letsdance";
            animDuration = 216.16f;
            half = true;
            zoomDur = 6f;
            music = Modules.Config.musicOn2.Value;
            bool client = Modules.Config.musicClient.Value;
            if (music)
            {
                if (client && isAuthority)
                {
                    sound = AkSoundEngine.PostEvent(1924791374, this.gameObject);
                }
                else if(!client)
                {
                    sound = AkSoundEngine.PostEvent(1924791374, this.gameObject);
                }
                convar = RoR2.Console.instance.FindConVar("volume_music");
                if (convar != null)
                {
                    oldMusic = convar.GetString();
                    convar.SetString("0");
                }
            }

            this.gameObject.AddComponent<BayoWeaponComponent>();
            bwc = this.gameObject.GetComponent<BayoWeaponComponent>();
            y = -2.25f;
            z = -6f;
            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && stopwatch >= 213.28f)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            if (music)
            {
                //AkSoundEngine.SetRTPCValue("Volume_MSX", currentMaster * actualMSX);
                convar.SetString(oldMusic);
                AkSoundEngine.StopPlayingID(sound);
            }

            Destroy(bwc);
            base.OnExit();
        }

    }
}
