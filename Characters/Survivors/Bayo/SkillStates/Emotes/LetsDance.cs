using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using RoR2.ConVar;
using BayoMod.Survivors.Bayo.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class LetsDance : BaseEmote
    {
        private uint sound = 0;
        private bool music;
        BaseConVar convar;
        private string oldMusic;
        private BayoWeaponComponent bwc;
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
                if (isAuthority)
                {
                    convar = RoR2.Console.instance.FindConVar("volume_music");
                    if (convar != null)
                    {
                        oldMusic = convar.GetString();
                        if (oldMusic != "0") convar.SetString("0");
                    }
                }
            }

            bwc = this.gameObject.GetComponent<BayoWeaponComponent>();
            bwc.currentWeapon = BayoWeaponComponent.WeaponState.Open;
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
                if (convar != null)
                {
                    if (oldMusic != "0") convar.SetString(oldMusic);
                }
                if (sound != 0)
                {
                    AkSoundEngine.StopPlayingID(sound);
                }
            }

            bwc.currentWeapon = BayoWeaponComponent.WeaponState.Guns;
            base.OnExit();
        }

    }
}
