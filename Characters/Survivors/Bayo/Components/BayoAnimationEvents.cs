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

        private CameraController camController;


        private void Start()
        {
            characterModel = GetComponent<CharacterModel>();
            if ((bool)characterModel && (bool)characterModel.body)
            {
                bodyObject = characterModel.body.gameObject;
            }
            camController = bodyObject.gameObject.GetComponent<CameraController>();
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
