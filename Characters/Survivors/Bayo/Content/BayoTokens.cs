using System;
using BayoMod.Modules;
using BayoMod.Survivors.Bayo.Achievements;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoTokens
    {
        public static void Init()
        {
            AddBayoTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddBayoTokens()
        {
            string prefix = BayoSurvivor.BAYO_PREFIX;

            string desc = "BAYO is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Bayonetta");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Chosen One");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Bayonetta passive");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_COMBO_NAME", "Combo");
            Language.Add(prefix + "PRIMARY_COMBO_DESCRIPTION", Tokens.agilePrefix + $"punch them idk");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_ABK_NAME", "Umbran Techniques");
            Language.Add(prefix + "SECONDARY_ABK_DESCRIPTION", Tokens.agilePrefix + $"Perform a variety of different moves depending on airstate and if back is held");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_DODGE_NAME", "Dodge");
            Language.Add(prefix + "UTILITY_DODGE_DESCRIPTION", "DODGE a shorter distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * BayoStaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(BayoMasteryAchievement.identifier), "Bayoetta: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(BayoMasteryAchievement.identifier), "As Bayonetta, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
