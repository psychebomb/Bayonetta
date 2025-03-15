using UnityEngine;
using RoR2;

namespace BayoMod.Survivors.Bayo.Components
{
    internal class BayoWeaponComponent : MonoBehaviour
    {
        private ChildLocator component2;
        private GameObject transformm;
        private GameObject transformm2;
        private void Awake()
        {
            ModelLocator component = this.gameObject.GetComponent<ModelLocator>();
            component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                for(int i=0; i < 5; ++i)
                {
                    int childIndex = component2.FindChildIndex("gunrh" + i.ToString());
                    int childIndex2 = component2.FindChildIndex("gunlh" + i.ToString());
                    transformm = component2.FindChild(childIndex).gameObject;
                    transformm2 = component2.FindChild(childIndex2).gameObject;
                    transformm.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    transformm2.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
            }
        }

        /*
        private void OnDisable()
        {
            if ((bool)component2)
            {
                for (int i = 0; i < 5; ++i)
                {
                    transformm.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    transformm2.GetComponent<SkinnedMeshRenderer>().enabled = true;
                }
            }
        }
        */

        private void OnDestroy()
        {
            if ((bool)component2)
            {
                for (int i = 0; i < 5; ++i)
                {
                    int childIndex = component2.FindChildIndex("gunrh" + i.ToString());
                    int childIndex2 = component2.FindChildIndex("gunlh" + i.ToString());
                    transformm = component2.FindChild(childIndex).gameObject;
                    transformm2 = component2.FindChild(childIndex2).gameObject;
                    transformm.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    transformm2.GetComponent<SkinnedMeshRenderer>().enabled = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if ((bool)component2)
            {
                for (int i = 0; i < 5; ++i)
                {
                    int childIndex = component2.FindChildIndex("gunrh" + i.ToString());
                    int childIndex2 = component2.FindChildIndex("gunlh" + i.ToString());
                    transformm = component2.FindChild(childIndex).gameObject;
                    transformm2 = component2.FindChild(childIndex2).gameObject;

                    if (transformm.GetComponent<SkinnedMeshRenderer>().enabled == true)
                    {
                        transformm.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    if (transformm2.GetComponent<SkinnedMeshRenderer>().enabled == true)
                    {
                        transformm2.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                }
            }
        }
    }
}