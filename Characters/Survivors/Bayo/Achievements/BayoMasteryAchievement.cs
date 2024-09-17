using RoR2;
using BayoMod.Modules.Achievements;

namespace BayoMod.Survivors.Bayo.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5, null)]
    public class BayoMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = BayoSurvivor.BAYO_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = BayoSurvivor.BAYO_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => BayoSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}