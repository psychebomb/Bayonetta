using RoR2;
using UnityEngine;
using UnityEngine.UIElements;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class ABKRotator : MonoBehaviour
    {

        public Vector3 lookDir;

        private Quaternion origRotation;

        public bool rotate = false;

        private bool rotatedVFX = false;

        private Transform boneTrans;

        private Transform vfxTrans;
        private void Start()
        {
            ModelLocator component = this.gameObject.GetComponent<ModelLocator>();
            ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex("BoneR");
                boneTrans = component2.FindChild(childIndex);
                childIndex = component2.FindChildIndex("ABKC");
                vfxTrans = component2.FindChild(childIndex);
            }
        }
        private void LateUpdate()
        {
            if (lookDir != Vector3.zero && rotate)
            {
                boneTrans.rotation *= Quaternion.AngleAxis((lookDir.y * 90f), Vector3.forward);
                if (!rotatedVFX)
                {
                    rotatedVFX = true;
                    origRotation = vfxTrans.rotation;
                    vfxTrans.rotation *= Quaternion.AngleAxis((lookDir.y * -90f), Vector3.right);
                }
            }

            if (!rotate && rotatedVFX)
            {
                rotatedVFX = false;
                vfxTrans.rotation = origRotation;
            }
        }
    }
}
