using BayoMod.Survivors.Bayo.Achievements;
using RoR2;
using UnityEngine;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                BayoMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(BayoMasteryAchievement.identifier),
                BayoSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
