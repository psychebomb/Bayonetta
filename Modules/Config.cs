using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;

namespace BayoMod.Modules
{
    public static class Config
    {
        public static ConfigFile MyConfig = BayoPlugin.instance.Config;

        /// <summary>
        /// automatically makes config entries for disabling survivors
        /// </summary>
        /// <param name="section"></param>
        /// <param name="characterName"></param>
        /// <param name="description"></param>
        /// <param name="enabledByDefault"></param>
        public static ConfigEntry<KeyCode> emote1Keybind;

        public static void ReadConfig()
        {
            emote1Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Emote1"), KeyCode.Alpha1, new ConfigDescription("I've got a fever, and the only cure is more dead angels"));
        }

    }
}
