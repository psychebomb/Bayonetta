﻿using UnityEngine;
using RoR2;
using UnityEngine.UIElements;

namespace BayoMod.Survivors.Bayo.Components
{
    internal class BayoWeaponComponent : MonoBehaviour
    {
        public enum WeaponState
        {
            Guns,
            Open,
        }

        public WeaponState currentWeapon = WeaponState.Guns;

        private int RHOpen;
        private int RHGun;
        private int LHOpen;
        private int LHGun;
        private int GunsTop;

        private ChildLocator component2;
        private void Awake()
        {
            ModelLocator component = this.gameObject.GetComponent<ModelLocator>();
            component2 = component.modelTransform.GetComponent<ChildLocator>();

            if ((bool)component2)
            {
                RHOpen = component2.FindChildIndex("RHO");
                RHGun = component2.FindChildIndex("RHG");
                LHOpen = component2.FindChildIndex("LHO");
                LHGun = component2.FindChildIndex("LHG");
                GunsTop = component2.FindChildIndex("GHands");

            }
        }
        private void OnDisable()
        {
            if ((bool)component2)
            {
                EnableMesh(RHOpen);
                DisableMesh(RHGun);
                EnableMesh(LHOpen);
                DisableMesh(LHGun);
                EnableMesh(GunsTop);
            }
        }
        private void OnDestroy()
        {
            if ((bool)component2)
            {
                EnableMesh(RHOpen);
                DisableMesh(RHGun);
                EnableMesh(LHOpen);
                DisableMesh(LHGun);
                EnableMesh(GunsTop);
            }
        }
        private void EnableMesh(int childIndex)
        {
            GameObject obj = component2.FindChild(childIndex).gameObject;
            if (obj.GetComponent<SkinnedMeshRenderer>().enabled == false)
            {
                obj.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
        }
        private void DisableMesh(int childIndex)
        {
            GameObject obj = component2.FindChild(childIndex).gameObject;
            if (obj.GetComponent<SkinnedMeshRenderer>().enabled == true)
            {
                obj.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        private void LateUpdate()
        {
            if ((bool)component2)
            {
                switch (currentWeapon)
                {
                    case WeaponState.Guns:
                        DisableMesh(RHOpen);
                        EnableMesh(RHGun);
                        DisableMesh(LHOpen);
                        EnableMesh(LHGun);
                        EnableMesh(GunsTop);
                        break;
                    case WeaponState.Open:
                        EnableMesh(RHOpen);
                        DisableMesh(RHGun);
                        EnableMesh(LHOpen);
                        DisableMesh(LHGun);
                        DisableMesh(GunsTop);
                        break;
                }
            }
        }
    }
}