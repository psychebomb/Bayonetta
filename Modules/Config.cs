using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;

namespace BayoMod.Modules
{
    public static class Config
    {
        public static ConfigFile MyConfig = BayoPlugin.instance.Config;

        public static ConfigEntry<KeyCode> emote1Keybind;
        public static ConfigEntry<KeyCode> emote2Keybind;
        public static ConfigEntry<bool> musicOn;
        public static ConfigEntry<bool> overlayOn;
        public static ConfigEntry<bool> wtInvul;

        public static void ReadConfig()
        {
            emote1Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha1, new ConfigDescription("I've got a fever, and the only cure is more dead angels"));
            emote2Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Strut"), KeyCode.Alpha2, new ConfigDescription("Dreadful!"));
            musicOn = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Strut music", true, "Toggle's whether Bayonetta's strut emote plays music.");
            overlayOn = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Witch time screen overlay", true, "Causes a screen overlay effect to occur during witch time when enabled.");
            wtInvul = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Witch time invulnerability", false, "Gives Bayonetta invincibilty instead of an armor boost during with time when enabled.");
        }

    }
}
