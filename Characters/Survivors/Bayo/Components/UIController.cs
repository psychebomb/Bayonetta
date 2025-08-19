using UnityEngine;
using RoR2;
using RoR2.UI;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class UIController : MonoBehaviour
    {
        private GameObject RoRHUDObject;

        public void Start()
        {
            On.RoR2.UI.HUD.Update += HUD_Update;
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
