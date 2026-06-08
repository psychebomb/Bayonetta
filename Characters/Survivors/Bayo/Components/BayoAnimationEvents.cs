using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using RoR2;
using UnityEngine;
using UnityEngine.UIElements;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class BayoAnimationEvents : MonoBehaviour
    {

        private GameObject bodyObject;

        private CharacterModel characterModel;

        private BayoCameraController camController;


        private void Start()
        {
            characterModel = GetComponent<CharacterModel>();
            if ((bool)characterModel && (bool)characterModel.body)
            {
                bodyObject = characterModel.body.gameObject;
            }
            camController = bodyObject.gameObject.GetComponent<BayoCameraController>();
        }
        public void PlayFastWoosh()
        {
            Util.PlaySound("wooshf", bodyObject);
        }
        public void PlaySlowWoosh()
        {
            Util.PlaySound("wooshs", bodyObject);
        }
        public void PlayDoubleWoosh()
        {
            Util.PlaySound("wooshd", bodyObject);
        }
        public void ZoomInFOV()
        {
            //camController.ZoomIn();
        }

        public void ZoomOutFOV()
        {
            //camController.ZoomOut();
        }
    }
}
