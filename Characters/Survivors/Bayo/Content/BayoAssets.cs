﻿using RoR2;
using UnityEngine;
using BayoMod.Modules;
using RoR2.Projectile;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using BayoMod.Modules.Components;
using R2API.Utils;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoAssets
    {
        //projectiles

        public static GameObject fistProjectilePrefab;

        public static GameObject footProjectilePrefab;

        public static GameObject bulletMuz;

        private static GameObject tempMuz;

        public static GameObject wtOverlay;

        public static GameObject wtOverlay2;

        private static AssetBundle _assetBundle;

        internal static GameObject trackerPrefab;

        public static GameObject wardPrefab;

        public static GameObject tempWard;

        public static GameObject evilObject;

        public static PostProcessProfile profile;

        #region swing effects

        public static GameObject p1s;
        public static GameObject p1as;
        public static GameObject p2s;
        public static GameObject p2as;
        public static GameObject p3s;
        public static GameObject p3as;
        public static GameObject p4s;
        public static GameObject p4as;
        public static GameObject pflur;
        public static GameObject damage;

        public static GameObject heelk;
        public static GameObject backk;
        public static GameObject backs;
        public static GameObject spin;
        public static GameObject slam;
        public static GameObject fallk;
        public static GameObject abk;

        public static GameObject bwings;
        public static GameObject djump;
        #endregion

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            CreateEffects();

            CreateProjectiles();

            CreateTrackers();

        }




        #region effects
        private static void CreateEffects()
        {
            CreateSwings();
            damage = _assetBundle.LoadEffect("damage", true);
            CreateOverlay();
            CreateMuz();
        }

        private static void CreateSwings()
        {
            p1s = _assetBundle.LoadEffect("m1p1", true);
            p1as = _assetBundle.LoadEffect("m1p1a", true);
            p2s = _assetBundle.LoadEffect("m1p2", true);
            p2as = _assetBundle.LoadEffect("m1p2a", true);
            p3s = _assetBundle.LoadEffect("m1p3", true);
            p3as = _assetBundle.LoadEffect("m1p3a", true);
            p4s = _assetBundle.LoadEffect("m1p4", true);
            p4as = _assetBundle.LoadEffect("m1p4a", true);
            pflur = _assetBundle.LoadAsset<GameObject>("m1flur");

            heelk = _assetBundle.LoadEffect("heelkick", true);
            fallk = _assetBundle.LoadEffect("fallkick", true);
            spin = _assetBundle.LoadAsset<GameObject>("spin");
            backk = _assetBundle.LoadAsset<GameObject>("backkick");
            backs = _assetBundle.LoadAsset<GameObject>("backspin");
            abk = _assetBundle.LoadAsset<GameObject>("abk");
            slam = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/HermitCrab/HermitCrabBombExplosion.prefab").WaitForCompletion().InstantiateClone("BayoSlam", false);

            bwings = _assetBundle.LoadAsset<GameObject>("wings");
            djump = _assetBundle.LoadEffect("djump", true);

            p2s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            p2as.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            MoveOffset mo = p3s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;
            p3as.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo = p4s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;
            //UnityEngine.Object.Destroy(slam.GetComponent<ShakeEmitter>());
            UnityEngine.Object.Destroy(slam.transform.Find("Water, Billboard").gameObject);
            UnityEngine.Object.Destroy(slam.transform.Find("Flash").gameObject);
            slam.GetComponent<EffectComponent>().soundName = "";
            Vector3 tempVec = new Vector3(3f, 3f, 3f);
            slam.transform.Find("Water, Radial").gameObject.transform.set_localScale_Injected(ref tempVec);
            slam.transform.Find("Point Light").gameObject.GetComponent<Light>().range = 3f;
            ShakeEmitter se = slam.gameObject.GetComponent<ShakeEmitter>();
            se.radius = 50f;
            se.duration = 0.2f;
            se.wave.amplitude = 3f;
            se.wave.frequency = 60f;
            slam.transform.Find("Water, Directional").gameObject.transform.set_localScale_Injected(ref tempVec);
            tempVec = new Vector3(2f, 2f, 2f);
            slam.transform.Find("Debris, 3D").gameObject.transform.set_localScale_Injected(ref tempVec);
            Color temp = slam.transform.Find("Water, Directional").gameObject.GetComponent<ParticleSystem>().main.startColor.color;
            temp = new Color(0.231372f, 0.2f, 231372f, temp.a);
            bwings.AddComponent<WingComponent>();

            //312B25
            ContentAddition.AddEffect(slam);

            for (int i= 1; i < 7; ++i)
            {
                mo = pflur.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.slideDur = 0.15f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = heelk.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.5f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = spin.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.8f;
                mo.slideDur = 0.125f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = backk.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -1f;
                mo.slideDur = 0.35f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = backs.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = fallk.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.3f;
            }
        }
        private static void CreateOverlay()
        {
            if (Modules.Config.overlayOn.Value)
            {
                wtOverlay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion().InstantiateClone("WTOver", false);
                UnityEngine.Object.Destroy(wtOverlay.transform.Find("VisualEffect/BleedOverTime").gameObject);
                UnityEngine.Object.Destroy(wtOverlay.transform.Find("VisualEffect/Small Sparks").gameObject);
                UnityEngine.Object.Destroy(wtOverlay.transform.Find("VisualEffect/Smoke").gameObject);
                UnityEngine.Object.Destroy(wtOverlay.transform.Find("VisualEffect/Point Light").gameObject);
                UnityEngine.Object.Destroy(wtOverlay.transform.Find("CameraEffect/Shake").gameObject);

                profile = wtOverlay.transform.Find("CameraEffect/PP").gameObject.GetComponent<PostProcessVolume>().profile;
                profile.RemoveSettings<ColorGrading>();
                profile.RemoveSettings<ChromaticAberration>();
                profile.GetSetting<Vignette>().intensity.value = 0.3f;
                RampFog rf = profile.GetSetting<RampFog>();
                rf.fogColorStart.value = new Color(0.2f, 0.0980392156862745f, 0.4f, 0.09f);
                rf.fogIntensity.value = 0.9f;
                rf.fogPower.value = 0.75f;

                wtOverlay.transform.Find("CameraEffect/PP").gameObject.GetComponent<PostProcessVolume>().sharedProfile = profile;
            }

            wtOverlay2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion().InstantiateClone("WTOver2", false);
            UnityEngine.Object.Destroy(wtOverlay2.transform.Find("VisualEffect/BleedOverTime").gameObject);
            UnityEngine.Object.Destroy(wtOverlay2.transform.Find("VisualEffect/Small Sparks").gameObject);
            UnityEngine.Object.Destroy(wtOverlay2.transform.Find("VisualEffect/Smoke").gameObject);
            wtOverlay2.transform.Find("VisualEffect/Point Light").gameObject.GetComponent<Light>().color = Color.white;
            UnityEngine.Object.Destroy(wtOverlay2.transform.Find("CameraEffect/Shake").gameObject);
            //wtOverlay2.GetComponent<TemporaryVisualEffect>().visualTransform = null;

            //wtOverlay.transform.Find("CameraEffect/PP").gameObject.GetComponent<PostProcessVolume>();
        }

        private static void CreateMuz()
        {
            bulletMuz = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Muzzleflash1.prefab").WaitForCompletion().InstantiateClone("BayoMuz", true);
            tempMuz = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion().InstantiateClone("BayoTempMuz", true);

            ParticleSystem ps = bulletMuz.transform.Find("Starburst").gameObject.GetComponent<ParticleSystem>();
            ParticleSystem ps2 = tempMuz.transform.Find("TriangleSparks").gameObject.GetComponent<ParticleSystem>();
            ps.colorOverLifetime.color.gradient.SetKeys(ps2.colorOverLifetime.color.gradient.colorKeys, ps2.colorOverLifetime.color.gradient.alphaKeys);
            ps = bulletMuz.transform.Find("HitFlash").gameObject.GetComponent<ParticleSystem>();
            ps.colorOverLifetime.color.gradient.SetKeys(ps2.colorOverLifetime.color.gradient.colorKeys, ps2.colorOverLifetime.color.gradient.alphaKeys);

            Light light = bulletMuz.transform.Find("Point light").gameObject.GetComponent<Light>();
            Light light2 = tempMuz.transform.Find("Point light").gameObject.GetComponent<Light>();

            light.color = light2.color;

            ContentAddition.AddEffect(bulletMuz);

        }

        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateWeaveProjectiles();
            CreateWard();
            CreateEvilObject();
            Content.AddProjectilePrefab(fistProjectilePrefab);
            Content.AddProjectilePrefab(footProjectilePrefab);
            PrefabAPI.RegisterNetworkPrefab(fistProjectilePrefab);
            PrefabAPI.RegisterNetworkPrefab(footProjectilePrefab);
            ContentAddition.AddNetworkedObject(wardPrefab);
            ContentAddition.AddNetworkedObject(evilObject);

        }

        private static void CreateWeaveProjectiles()
        {
            if (_assetBundle.LoadAsset<GameObject>("footproj") != null && _assetBundle.LoadAsset<GameObject>("weavefoot") != null)
            {
                footProjectilePrefab = _assetBundle.LoadAsset<GameObject>("footproj");
                footProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavefoot");
                footProjectilePrefab.AddComponent<WickedWeave>();
                ShakeEmitter shakeEmitter = footProjectilePrefab.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.36f;
                shakeEmitter.radius = 100f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;
                shakeEmitter.wave = new Wave
                {
                    amplitude = 2f,
                    frequency = 7f,
                    cycleOffset = 0f
                };
            }
            if (_assetBundle.LoadAsset<GameObject>("fistproj") != null && _assetBundle.LoadAsset<GameObject>("weavehand") != null)
            {
                fistProjectilePrefab = _assetBundle.LoadAsset<GameObject>("fistproj");
                fistProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavehand");
                fistProjectilePrefab.AddComponent<WickedWeave>();
                ShakeEmitter shakeEmitter = fistProjectilePrefab.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.36f;
                shakeEmitter.radius = 100f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;
                shakeEmitter.wave = new Wave
                {
                    amplitude = 2f,
                    frequency = 7f,
                    cycleOffset = 0f
                };
            }
        }
        #endregion projectiles

        private static void CreateTrackers()
        {
            trackerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"), "BayoTrackerPrefab", false);
            trackerPrefab.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
            trackerPrefab.transform.Find("Core Pip").localScale = new Vector3(0.15f, 0.15f, 0.15f);

            trackerPrefab.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            trackerPrefab.transform.Find("Core, Dark").localScale = new Vector3(0.1f, 0.1f, 0.1f);

            foreach (SpriteRenderer i in trackerPrefab.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i)
                {
                    i.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    i.color = Color.red;
                }
            }

            /*
            punishTracker = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"), "PunishTrackerPrefab", false);
            punishTracker.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
            punishTracker.transform.Find("Core Pip").localScale = new Vector3(0.15f, 0.15f, 0.15f);

            punishTracker.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            punishTracker.transform.Find("Core, Dark").localScale = new Vector3(0.1f, 0.1f, 0.1f);

            foreach (SpriteRenderer i in punishTracker.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i)
                {
                    i.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    i.color = Color.yellow;
                }
            }
            */

        }

        private static void CreateWard()
        {
            wardPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion().InstantiateClone("WtWardPrefab", true);
            
            BuffWard buffWard = wardPrefab.GetComponent<BuffWard>();
            buffWard.buffDef = BayoBuffs.wtDebuff;
            buffWard.invertTeamFilter = true;
            buffWard.radius = 25f;
            buffWard.interval = 0.2f;
            buffWard.expires = false;

            SphereCollider sc = wardPrefab.GetComponent<SphereCollider>();
            sc.radius = 25f;

            SlowDownProjectiles sdp = wardPrefab.GetComponent<SlowDownProjectiles>();
            sdp.slowDownCoefficient = 0.075f;

            NetworkedBodyAttachment nba = wardPrefab.AddComponent<NetworkedBodyAttachment>();
            nba.shouldParentToAttachedBody = true;
            wardPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            Rigidbody rb = wardPrefab.GetComponent<Rigidbody>();
            rb.angularDrag = 0.05f;
            rb.interpolation = RigidbodyInterpolation.None;

            tempWard = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/AffixHauntedWard.prefab").WaitForCompletion().InstantiateClone("TempBayoWard", false);
            var sphere = wardPrefab.transform.Find("AreaIndicator/Sphere").gameObject.GetComponent<MeshRenderer>();
            Material mat = tempWard.transform.Find("Indicator/IndicatorSphere").gameObject.GetComponent<MeshRenderer>().material;
            //Material mat = Addressables.LoadAssetAsync<Material>("RoR2/Base/EliteHaunted/matHauntedEliteAreaIndicator.mat").WaitForCompletion();
            int num = mat.GetTexturePropertyNameIDs()[3];
            mat.SetTexture(num, _assetBundle.LoadAsset<Texture>("texRampWt"));
            Material[] mats = { mat };
            sphere.materials = mats;
            
            UnityEngine.Object.Destroy(wardPrefab.transform.Find("AreaIndicator/Point Light").gameObject);
            UnityEngine.Object.Destroy(wardPrefab.transform.Find("AreaIndicator/ChargeIn").gameObject);
            UnityEngine.Object.Destroy(wardPrefab.transform.Find("AreaIndicator/Core").gameObject);
            UnityEngine.Object.Destroy(wardPrefab.transform.Find("AreaIndicator/SoftGlow").gameObject);

        }

        //this is so stupid im sorry
        private static void CreateEvilObject()
        {
            evilObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPodBatteryPanel.prefab").WaitForCompletion().InstantiateClone("AwesomeBayoObject", true);
            UnityEngine.Object.Destroy(evilObject.GetComponent<EntityStateMachine>());
            UnityEngine.Object.Destroy(evilObject.GetComponent<NetworkStateMachine>());
            UnityEngine.Object.Destroy(evilObject.GetComponent<NetworkParent>());

            var inter = evilObject.transform.Find("Interactibility").gameObject;
            inter.transform.localPosition = new Vector3(0, 1.54f, 0);
            Vector3 newSize = new Vector3(2, 2, 3);
            inter.GetComponent<BoxCollider>().set_size_Injected(ref newSize);

            var mesh = evilObject.transform.Find("EscapePodMesh.002").gameObject.GetComponent<MeshRenderer>();

            Rigidbody rb = evilObject.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.angularDrag = 0;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;

            NetworkedBodyAttachment nba = evilObject.AddComponent<NetworkedBodyAttachment>();
            nba.shouldParentToAttachedBody = true;
            //nba.forceHostAuthority = true;

            GenericInteraction gi = evilObject.GetComponent<GenericInteraction>();
            gi.interactability = Interactability.Disabled;
            gi.contextToken = BayoSurvivor.BAYO_PREFIX + "PUNISH_PROMPT";
            gi.shouldShowOnScanner = false;
            gi.shouldIgnoreSpherecastForInteractibility = false;
            gi.onActivation.RemoveAllListeners();

            NetworkIdentity nw = evilObject.GetComponent<NetworkIdentity>();
            nw.localPlayerAuthority = true;

            //uncomment this stuff for debugging
            /*
            Light l = evilObject.AddComponent<Light>();
            l.color = Color.red;
            l.intensity = 20f;
            l.range = 10f;
            */
            //and then comment this stuff
            ///*
            UnityEngine.Object.Destroy(evilObject.GetComponent<Highlight>());
            mesh.enabled = false;
            gi.shouldProximityHighlight = false;
            //*/

        }

    }
}
