using RoR2;
using UnityEngine;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;

        public static BuffDef dodgeBuff;

        public static BuffDef evadeSuccess;

        public static BuffDef wtBuff;

        public static BuffDef wtCoolDown;

        public static BuffDef wtDebuff;

        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("BayoArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            dodgeBuff = Modules.Content.CreateAndAddBuff("BayoDodgeBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.magenta,
                false,
                false);

            evadeSuccess = Modules.Content.CreateAndAddBuff("BayoSuccessBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.yellow,
                false,
                false);

            wtBuff = Modules.Content.CreateAndAddBuff("BayoWTBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Overheat").iconSprite,
                Color.magenta,
                false,
                false);

            wtCoolDown = Modules.Content.CreateAndAddBuff("BayoWTCDBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Overheat").iconSprite,
                Color.gray,
                true,
                false);

            wtDebuff = Modules.Content.CreateAndAddBuff("BayoWTDebuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Overheat").iconSprite,
                Color.black,
                false,
                true);
        }
    }
}
