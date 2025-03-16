using RoR2;
using UnityEngine;
using BayoMod.Modules;
using RoR2.Projectile;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using BayoMod.Modules.Components;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoAssets
    {
        //projectiles

        public static GameObject fistProjectilePrefab;

        public static GameObject footProjectilePrefab;

        public static GameObject bulletMuz;

        public static GameObject wtOverlay;

        public static GameObject wtOverlay2;

        private static AssetBundle _assetBundle;

        internal static GameObject trackerPrefab;

        public static GameObject wardPrefab;

        public static GameObject tempWard;

        public static GameObject evilObject;

        public static PostProcessProfile profile;

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
            CreateOverlay();
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
            PrefabAPI.RegisterNetworkPrefab(evilObject);
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
                    amplitude = 0.5f,
                    frequency = 10f,
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
                    amplitude = 0.5f,
                    frequency = 10f,
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
