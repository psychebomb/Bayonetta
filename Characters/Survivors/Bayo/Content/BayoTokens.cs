using System;
using BayoMod.Modules;
using BayoMod.Survivors.Bayo.Achievements;
using R2API;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoTokens
    {
        public static void Init()
        {
            AddBayoTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Bayo.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddBayoTokens()
        {
            string prefix = BayoSurvivor.BAYO_PREFIX;

            string desc = "Bayonetta is a versatile melee-ranged hybrid, able to weave in and out of danger in order to dish out high damage.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > If bayonetta dodges while in using Bullet Arts, she'll continue the combo sequence where she left off as long as the button for Bullet Arts is held." + Environment.NewLine + Environment.NewLine
             + "< ! > The added armor and slow effect from witch time gives Bayonetta a much needed layer of protection from up-close threats. Use witch time as an opportunity to close the gap and use Bayonetta's high-damaging short range options." + Environment.NewLine + Environment.NewLine
             + "< ! > Many of Bayonetta's moves can be cancelled early with a jump. All of Bayonetta's moves are cancellable with Dodge." + Environment.NewLine + Environment.NewLine
             + "< ! > Similar to the effects of witch time, heavier enemies below half health will also receive more knockback from Bayonetta's moves." + Environment.NewLine + Environment.NewLine
             + "< ! > Repeatedly pressing the primary skill button during a punish attack will speed up the attack." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, still in search for answers to her mysterious destiny.";
            string outroFailure = "..and so she vanished, but as long as there is light, THE SHADOW REMAINS CAST!";

            Language.Add(prefix + "NAME", "Bayonetta");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Left Eye of Darkness");
            Language.Add(prefix + "LORE", "girl idfk its just bayonetta");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);
            Language.Add(prefix + "PUNISH_PROMPT", "Punish!");

            #region Skins
            Language.Add(prefix + "BAYO2_SKIN_NAME", "Famed");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Bayonetta passive");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_COMBO_NAME", "Bullet Arts");
            Language.Add(prefix + "PRIMARY_COMBO_DESCRIPTION", $"Perform a 5 hit punch sequence, dealing <style=cIsDamage>{200f}%</style> damage for the first four hits." +
                $" The last hit is a flurry attack that deals <style=cIsDamage>{165f}%</style> damage each hit and summons a wicked weave when released, dealing <style=cIsDamage>{1500f}%</style> damage." +
                $" Bayonetta will also continuously fire her guns during the sequence, each shot dealing <style=cIsDamage>{50f}%</style> damage.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_ABK_NAME", "Umbran Techniques");
            Language.Add(prefix + "SECONDARY_ABK_DESCRIPTION",  $"Perform a variety of different moves depending on movement inputs and whether or not Bayonetta is grounded");

            LanguageAPI.Add("KEYWORD_BAYO_ABK", $"<style=cKeywordName>After Burner Kick</style> <style=cIsUtility>Input: aerial, hold forward + m2.</style> <style=cSub>Launch into the air with a blazing kick, knocking back lighter enemies and dealing <style=cIsDamage>{375f}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_SPIN", $"<style=cKeywordName>Heel Tornado</style> <style=cIsUtility>Input: aerial, hold neutral + m2.</style> <style=cSub>Perform a spinning kick, continuously dealing <style=cIsDamage>{150f}%</style> damage. Knocks away lighter enemies.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_DOWN", $"<style=cKeywordName>Death Drop</style> <style=cIsUtility>Input: aerial, hold back + m2.</style> <style=cSub>Quickly descend with a spiking kick, continuously dealing <style=cIsDamage>{250f}%</style> damage and <style=cIsUtility>knocking down</style> enemies.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_HEEL", $"<style=cKeywordName>Heel Slide</style> <style=cIsUtility>Input: grounded, hold forward + m2.</style> <style=cSub>Slide forwards at high speed, dealing <style=cIsDamage>{100f}%</style> damage." +
                $" Continue holding the move down after stopping to perform a rising kick, launching lighter enemies and dealing <style=cIsDamage>{300f}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_BREAK", $"<style=cKeywordName>Breakdance</style> <style=cIsUtility>Input: grounded, hold neutral + m2.</style> <style=cSub>Perform a breakdance, continuously dealing <style=cIsDamage>{120f}%</style> damage per hit.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_RISE", $"<style=cKeywordName>Full Moon Shoot</style> <style=cIsUtility>Input: grounded, hold back + m2.</style> <style=cSub>Perform a backflip that launches lighter enemies, dealing <style=cIsDamage>{395f}%</style> damage." +
                $" Continue holding the move down to keep spinning, continuously dealing <style=cIsDamage>{100f}%</style> damage.</style>");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_DODGE_NAME", "Dodge");
            Language.Add(prefix + "UTILITY_DODGE_DESCRIPTION", "Dodge a short distance, gaining brief invincibility and <style=cIsUtility>100 armor.</style> If you are hit during the invincibility, activate <style=cIsUtility>Witch Time.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_WT", $"<style=cKeywordName>Witch Time</style> <style=cSub>Gain <style=cIsUtility>350 armor.</style> Enemies and projectiles near Bayonetta are greatly slowed, and moves that normally only launch lighter enemies will now launch all non-boss enemies.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_WEAVEIN_NAME", "Tetsuzanko/Heel Stomp");
            Language.Add(prefix + "SPECIAL_WEAVEIN_DESCRIPTION", $"Lock onto enemies and use primary or secondary buttons to summon a wicked weave at their location.");
            Language.Add(prefix + "SPECIAL_TETSU_NAME", "Tetsuzanko");
            Language.Add(prefix + "SPECIAL_TETSU_DESCRIPTION", $"juninhiyandiayooo");
            Language.Add(prefix + "SPECIAL_WEEAVEOUT_NAME", "Cancel");
            Language.Add(prefix + "SPECIAL_WEAVEOUT_DESCRIPTION", $"Cancel");
            Language.Add(prefix + "SPECIAL_STOMP_NAME", "Heel Stomp");
            Language.Add(prefix + "SPECIAL_STOMP_DESCRIPTION", $"TEYIAHHH");

            LanguageAPI.Add("KEYWORD_BAYO_TETS", $"<style=cKeywordName>Tetsuzanko</style> <style=cIsUtility>Input: Primary (M1)</style> <style=cSub>Summon a demon fist that knocks enemies away, dealing <style=cIsDamage>{1500f}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_HSTOMP", $"<style=cKeywordName>Heel Stomp</style> <style=cIsUtility>Input: Secondary (M2)</style> <style=cSub>Summon a demon foot that <style=cIsUtility>knocks down</style> enemies, sending them downwards and dealing <style=cIsDamage>{1500f}%</style> damage.</style>");
            LanguageAPI.Add("KEYWORD_BAYO_KD", $"<style=cKeywordName>Knocked Down</style> <style=cSub>Enemies who are knocked down are stunned for a extended duration. If Bayonetta is grounded and close enough to a knocked down enemy, she can use the Interact button to perform a punish attack</style>");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(BayoMasteryAchievement.identifier), "Bayonetta: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(BayoMasteryAchievement.identifier), "She doesn't have a mastery skin yet sorry");
            #endregion
        }
    }
}
