using UnityEngine;
using RoR2;
using RoR2.UI;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class UIController : MonoBehaviour
    {
        private GameObject RoRHUDObject;
        private CharacterBody body;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            On.RoR2.UI.HUD.Update += HUD_Update;

            switch (body.skinIndex)
            {
                case 1:
                    body.portraitIcon = BayoAssets.bayo2icon.texture;
                    break;
                default:
                    body.portraitIcon = BayoAssets.bayo1icon.texture;
                    break;
            }
        }

        public void OnDestroy()
        {
            Unhook();
        }
        public void SetRORUIActiveState(bool state)
        {
            if (RoRHUDObject)
            {
                RoRHUDObject.SetActive(state);
            }
        }
        private void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, HUD self)
        {
            orig(self);
            if (!RoRHUDObject)
            {
                RoRHUDObject = self.gameObject;
            }
        }

        public void Unhook()
        {
            On.RoR2.UI.HUD.Update -= HUD_Update;
        }
    }
}
