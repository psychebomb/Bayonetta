using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;
using RoR2;

namespace BayoMod.Modules
{
    public static class Config
    {
        public static ConfigFile MyConfig = BayoPlugin.instance.Config;

        public static ConfigEntry<KeyCode> emote1Keybind;
        public static ConfigEntry<KeyCode> emote2Keybind;
        public static ConfigEntry<KeyCode> emote3Keybind;
        public static ConfigEntry<KeyCode> emote4Keybind;
        public static ConfigEntry<KeyCode> emote5Keybind;
        public static ConfigEntry<bool> musicClient;
        public static ConfigEntry<bool> eZoom;
        public static ConfigEntry<bool> musicOn;
        public static ConfigEntry<bool> musicOn2;
        public static ConfigEntry<bool> overlayOn;
        public static ConfigEntry<bool> tpFreeze;

        public enum LongTaunt
        {
            Random,
            Based_On_Skin_Choice,
            Bayo_1_Only,
            Bayo_2_Only
        }

        public enum StrutMusic
        {
            Random,
            Walking,
            Crazy
        }

        public static ConfigEntry<LongTaunt> longTaunt;
        public static ConfigEntry<StrutMusic> strutMus;

        //stats
        public static ConfigEntry<float> hpStat;
        public static ConfigEntry<float> regenStat;
        public static ConfigEntry<float> armorStat;
        public static ConfigEntry<float> wtx;
        public static ConfigEntry<float> wty;
        public static ConfigEntry<float> wtz;
        public static ConfigEntry<int> wtdur;
        public static ConfigEntry<int> wtpdur;
        public static ConfigEntry<int> wtcoold;

        public static void ReadConfig()
        {
            emote1Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Long Taunt"), KeyCode.Alpha1, new ConfigDescription("I've got a fever, and the only cure is more dead angels"));
            emote2Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Short Taunt"), KeyCode.Alpha2, new ConfigDescription("You want to touch me?"));
            emote3Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Lets Dance Boys (cam)"), KeyCode.Alpha3, new ConfigDescription("Entire LDB dance from end of bayo 1 with camera animations"));
            emote4Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Lets Dance Boys (no cam)"), KeyCode.Alpha4, new ConfigDescription("Entire LDB dance from end of bayo 1 without camera animations"));
            emote5Keybind = BayoPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Strut Emote"), KeyCode.Alpha5, new ConfigDescription("This one makes bayo walk slow"));
            musicOn = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Strut music", true, "Toggle's whether Bayonetta's strut emote plays music.");
            strutMus = BayoPlugin.instance.Config.Bind<StrutMusic>("01 - Misc Settings", "Strut music choice", StrutMusic.Walking, "Chooses which music plays during the strut emote (none are dmca save lol)");
            musicOn2 = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Lets Dance Boys music", true, "Toggle's whether Bayonetta's LDB emote plays music.");
            longTaunt = BayoPlugin.instance.Config.Bind<LongTaunt>("01 - Misc Settings", "Emote 1 voice options", LongTaunt.Based_On_Skin_Choice, "Chooses which voiceline will play when performing Emote 1.");
            musicClient = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Client side music", true, "Toggle's whether or not emotes with music will play for the whole server (false) or just the local client (true)");
            eZoom = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Emote zoom", true, "Toggle's whether Bayonetta's emotes zoom in the camera.");
            overlayOn = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Witch time screen overlay", true, "Causes a screen overlay effect to occur during witch time when enabled.");
            tpFreeze = BayoPlugin.instance.Config.Bind<bool>("01 - Misc Settings", "Teleporter Freeze-Frame Event", true, "Activates a special freeze frame + emote after entering a teleporter when enabled.");

            hpStat = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "Base HP"), 110f, new ConfigDescription("Bayonetta's base starting HP"));
            regenStat = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "Base HP Regen"), 1f, new ConfigDescription("Bayonetta's base starting health regen"));
            armorStat = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "Base Armor"), 0f, new ConfigDescription("Bayonetta's base starting armor stat"));
            wtx = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "WT activation X"), 4f, new ConfigDescription("The x value for the bounds of witch time's activation hitbox"));
            wty = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "WT activation Y"), 2f, new ConfigDescription("The y value for the bounds of witch time's activation hitbox"));
            wtz = BayoPlugin.instance.Config.Bind<float>(new ConfigDefinition("02 - Base Stats", "WT activation Z"), 4f, new ConfigDescription("The z value for the bounds of witch time's activation hitbox"));
            wtdur = BayoPlugin.instance.Config.Bind<int>(new ConfigDefinition("02 - Base Stats", "Witch Time Duration"), 4, new ConfigDescription("The base duration for witch time"));
            wtpdur = BayoPlugin.instance.Config.Bind<int>(new ConfigDefinition("02 - Base Stats", "Witch Time Perfect Duration"), 6, new ConfigDescription("The base duration for witch time when dodging with perfect timing"));
            wtcoold = BayoPlugin.instance.Config.Bind<int>(new ConfigDefinition("02 - Base Stats", "Witch Time Cooldown"), 10, new ConfigDescription("The base duration for witch time's cooldown"));
        }
    }
}
