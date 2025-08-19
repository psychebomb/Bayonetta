using RoR2;
using UnityEngine;
using BayoMod.Modules;
using RoR2.Projectile;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using BayoMod.Modules.Components;
using RoR2.Audio;
using TMPro;
using BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates;
using BayoMod.Characters.Survivors.Bayo.Components.Demon;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoAssets
    {
        //projectiles

        public static GameObject fistProjectilePrefab;

        public static GameObject footProjectilePrefab;

        public static GameObject fistFast;

        public static GameObject footFast;

        public static GameObject fistDown;

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

        public static GameObject gomorrah;

        public static GameObject summonHair;

        public static GameObject camObj;

        #region vfx hehe

        public static GameObject p1s;
        public static GameObject p1as;
        public static GameObject p2s;
        public static GameObject p2as;
        public static GameObject p3s;
        public static GameObject p3as;
        public static GameObject p4s;
        public static GameObject p4as;
        public static GameObject pflur;

        public static GameObject heelk;
        public static GameObject heels;
        public static GameObject backk;
        public static GameObject backs;
        public static GameObject spin;
        public static GameObject slam;
        public static GameObject fallk;
        public static GameObject fall;
        public static GameObject falle;
        public static GameObject abk;

        public static GameObject p1s2;
        public static GameObject p1as2;
        public static GameObject p2s2;
        public static GameObject p2as2;
        public static GameObject p3s2;
        public static GameObject p3as2;
        public static GameObject p4s2;
        public static GameObject p4as2;
        public static GameObject pflur2;

        public static GameObject heelk2;
        public static GameObject heels2;
        public static GameObject backk2;
        public static GameObject backs2;
        public static GameObject spin2;
        public static GameObject slam2;
        public static GameObject fallk2;
        public static GameObject fall2;
        public static GameObject falle2;
        public static GameObject abk2;

        private static Material fireMat;
        public static GameObject p1art;
        //public static GameObject p1aart;
        public static GameObject p2art;
        //public static GameObject p2aart;
        public static GameObject p3art;
        //public static GameObject p3aart;
        public static GameObject p4art;
        //public static GameObject p4aart;

        public static GameObject bwings;
        public static GameObject bwings2;
        public static GameObject land;
        public static GameObject bats;
        public static GameObject djump;
        public static GameObject hearts;
        public static GameObject sum;
        public static GameObject damage;
        public static GameObject glintL;
        public static GameObject glintR;
        public static GameObject spotlight;
        public static GameObject spotlight2;
        #endregion

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            CreateEffects();

            CreateProjectiles();

            CreateTrackers();

            CreateDemons();

        }




        #region effects
        private static void CreateEffects()
        {
            CreateCommonEffects();
            CreateSwings();
            CreateSwings2();
            MakeArtstyleVFX();
            CreateOverlay();
            CreateMuz();
        }

        private static void CreateCommonEffects()
        {
            damage = _assetBundle.LoadEffect("damage", true);
            slam = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/HermitCrab/HermitCrabBombExplosion.prefab").WaitForCompletion().InstantiateClone("BayoSlam", false);
            bwings = _assetBundle.LoadAsset<GameObject>("wings");
            bwings2 = _assetBundle.LoadAsset<GameObject>("wings2");
            djump = _assetBundle.LoadEffect("djump", true);
            hearts = _assetBundle.LoadEffect("kiss", true);
            sum = _assetBundle.LoadEffect("summon", true);
            bats = _assetBundle.LoadAsset<GameObject>("batswithin");
            land = _assetBundle.LoadEffect("land", true);
            camObj = _assetBundle.LoadAsset<GameObject>("CamObject");
            glintL = _assetBundle.LoadEffect("shinel", true);
            glintR = _assetBundle.LoadEffect("shiner", true);

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
            bwings2.AddComponent<WingComponent2>();
            ContentAddition.AddEffect(slam);
        }
        private static void CreateSwings()
        {
            p1s = _assetBundle.LoadAsset<GameObject>("m1p1");
            p1as = _assetBundle.LoadEffect("m1p1a", true);
            p2s = _assetBundle.LoadAsset<GameObject>("m1p2");
            p2as = _assetBundle.LoadEffect("m1p2a", true);
            p3s = _assetBundle.LoadAsset<GameObject>("m1p3");
            p3as = _assetBundle.LoadEffect("m1p3a", true);
            p4s = _assetBundle.LoadAsset<GameObject>("m1p4");
            p4as = _assetBundle.LoadEffect("m1p4a", true);
            pflur = _assetBundle.LoadAsset<GameObject>("m1flur");

            heelk = _assetBundle.LoadEffect("heelkick", true);
            fallk = _assetBundle.LoadAsset<GameObject>("fallkick");
            fall = _assetBundle.LoadAsset<GameObject>("fall");
            falle = _assetBundle.LoadEffect("fallend", true);
            heels = _assetBundle.LoadAsset<GameObject>("heels");
            spin = _assetBundle.LoadAsset<GameObject>("spin");
            backk = _assetBundle.LoadAsset<GameObject>("backkick");
            backs = _assetBundle.LoadAsset<GameObject>("backspin");
            abk = _assetBundle.LoadAsset<GameObject>("abk");

            p1s.gameObject.AddComponent<VFXrm>();
            p2s.gameObject.AddComponent<VFXrm>();
            p3s.gameObject.AddComponent<VFXrm>();
            p4s.gameObject.AddComponent<VFXrm>();
            p2s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            p2as.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            MoveOffset mo = p3s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;
            p3as.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo = p4s.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;

            mo = fall.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = -3f;
            mo.idealOffset = 0;
            mo.smooth = false;
            mo.slideDur = 0.25f;

            mo = falle.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = -3f;
            mo.idealOffset = 0;
            mo.atEnd = true;
            mo.atStart = false;
            mo.duration = 0.4f;
            mo.slideDur = 0.2f;

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

        private static void CreateSwings2()
        {
            p1s2 = _assetBundle.LoadAsset<GameObject>("m1p12");
            p1as2 = _assetBundle.LoadEffect("m1p1a2", true);
            p2s2 = _assetBundle.LoadAsset<GameObject>("m1p22");
            p2as2 = _assetBundle.LoadEffect("m1p2a2", true);
            p3s2 = _assetBundle.LoadAsset<GameObject>("m1p32");
            p3as2 = _assetBundle.LoadEffect("m1p3a2", true);
            p4s2 = _assetBundle.LoadAsset<GameObject>("m1p42");
            p4as2 = _assetBundle.LoadEffect("m1p4a2", true);
            pflur2 = _assetBundle.LoadAsset<GameObject>("m1flur2");

            heelk2 = _assetBundle.LoadEffect("heelkick2", true);
            fallk2 = _assetBundle.LoadAsset<GameObject>("fallkick2");
            fall2 = _assetBundle.LoadAsset<GameObject>("fall2");
            falle2 = _assetBundle.LoadEffect("fallend2", true);
            heels2 = _assetBundle.LoadAsset<GameObject>("heels2");
            spin2 = _assetBundle.LoadAsset<GameObject>("spin2");
            backk2 = _assetBundle.LoadAsset<GameObject>("backkick2");
            backs2 = _assetBundle.LoadAsset<GameObject>("backspin2");
            abk2 = _assetBundle.LoadAsset<GameObject>("abk2");

            p1s2.gameObject.AddComponent<VFXrm>();
            p2s2.gameObject.AddComponent<VFXrm>();
            p3s2.gameObject.AddComponent<VFXrm>();
            p4s2.gameObject.AddComponent<VFXrm>();
            p2s2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            p2as2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            MoveOffset mo = p3s2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;
            p3as2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo = p4s2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.slideDur = 0.15f;

            mo = fall2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = -3f;
            mo.idealOffset = 0;
            mo.smooth = false;
            mo.slideDur = 0.25f;

            mo = falle2.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = -3f;
            mo.idealOffset = 0;
            mo.atEnd = true;
            mo.atStart = false;
            mo.duration = 0.4f;
            mo.slideDur = 0.2f;

            for (int i = 1; i < 7; ++i)
            {
                mo = pflur2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.slideDur = 0.15f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = heelk2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.5f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = spin2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.8f;
                mo.slideDur = 0.125f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = backk2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -1f;
                mo.slideDur = 0.35f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = backs2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                mo = fallk2.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.3f;
            }

            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, p1as, p1as2);
            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, p2as, p2as2);
            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, p3as, p3as2);
            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, p4as, p4as2);
            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, heelk, heelk2);
            SkinVFX.AddSkinVFX(BayoSurvivor.masterySkin, falle, falle2);

        }

    
        private static void MakeArtstyleVFX()
        {
            MakeMaterial();

            MakeArtSwings();
        }

        private static void MakeMaterial()
        {
            GameObject tempThingy = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Wisp/TracerEmbers.prefab").WaitForCompletion().InstantiateClone("TempBayoThingy", false);
            fireMat = tempThingy.gameObject.GetComponent<LineRenderer>().material;
            fireMat.mainTexture = _assetBundle.LoadAsset<Texture>("texBasicMask");
            fireMat.SetTexture("_Cloud1Tex", _assetBundle.LoadAsset<Texture>("testmask"));
            Vector4 scrollVec = new Vector4(0, 25, 4, 4);
            fireMat.SetVector("_CutoffScroll", scrollVec);

            fireMat.mainTextureOffset = new Vector2(0, 0.1f);
            fireMat.mainTextureScale = new Vector2(1, 0.8f);
        }

        private static void MakeArtSwings()
        {
            p1art = _assetBundle.LoadAsset<GameObject>("artm1p1");
            p1art.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;

            p2art = _assetBundle.LoadAsset<GameObject>("artm1p2");
            p2art.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            MoveOffset mo = p2art.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;

            p3art = _assetBundle.LoadAsset<GameObject>("artm1p3");
            p3art.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            mo = p3art.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;
            mo.slideDur = 0.15f;

            p4art = _assetBundle.LoadAsset<GameObject>("artm1p4");
            p4art.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            mo = p4art.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;
            mo.slideDur = 0.15f;

            p1art.gameObject.AddComponent<VFXrm>();
            p2art.gameObject.AddComponent<VFXrm>();
            p3art.gameObject.AddComponent<VFXrm>();
            p4art.gameObject.AddComponent<VFXrm>();

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
            wtOverlay2.transform.Find("VisualEffect/Point Light").gameObject.GetComponent<Light>().intensity = 10f;
            UnityEngine.Object.Destroy(wtOverlay2.transform.Find("CameraEffect/Shake").gameObject);

            spotlight = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion().InstantiateClone("BayoSpotlight", false);
            UnityEngine.Object.Destroy(spotlight.transform.Find("VisualEffect/BleedOverTime").gameObject);
            UnityEngine.Object.Destroy(spotlight.transform.Find("VisualEffect/Small Sparks").gameObject);
            UnityEngine.Object.Destroy(spotlight.transform.Find("VisualEffect/Smoke").gameObject);
            UnityEngine.Object.Destroy(spotlight.transform.Find("VisualEffect/Point Light").gameObject);
            UnityEngine.Object.Destroy(spotlight.transform.Find("CameraEffect/Shake").gameObject);

            spotlight2 = _assetBundle.LoadEffect("spotlight", true);
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
            Content.AddProjectilePrefab(fistFast);
            Content.AddProjectilePrefab(footFast);
            PrefabAPI.RegisterNetworkPrefab(fistFast);
            PrefabAPI.RegisterNetworkPrefab(footFast);
            Content.AddProjectilePrefab(fistDown);
            PrefabAPI.RegisterNetworkPrefab(fistDown);
            ContentAddition.AddNetworkedObject(wardPrefab);
            ContentAddition.AddNetworkedObject(evilObject);

        }

        private static void CreateWeaveProjectiles()
        {
            Material mat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Nullifier/matNullifierGemPortal2.mat").WaitForCompletion();

            if (_assetBundle.LoadAsset<GameObject>("footproj") != null && _assetBundle.LoadAsset<GameObject>("weavefoot") != null)
            {
                footProjectilePrefab = _assetBundle.LoadAsset<GameObject>("footproj");
                footProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavefoot");
                WickedWeave ww =footProjectilePrefab.AddComponent<WickedWeave>();
                ww.startTime = 0.24f;
                ww.hitboxEnd = 1.04f;
                ShakeEmitter shakeEmitter = footProjectilePrefab.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.36f;
                shakeEmitter.radius = 100f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;
                shakeEmitter.shakeOnStart = false;
                shakeEmitter.shakeOnEnable = true;
                shakeEmitter.wave = new Wave
                {
                    amplitude = 2f,
                    frequency = 7f,
                    cycleOffset = 0f
                };
                LoopSoundDef loop = ScriptableObject.CreateInstance<LoopSoundDef>();
                loop.startSoundName = "weaved";
                footProjectilePrefab.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
                footProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/ring").GetComponent<ParticleSystemRenderer>().material = mat;
                footProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/hair").GetComponent<ParticleSystemRenderer>().material = mat;

                footFast = _assetBundle.LoadAsset<GameObject>("footproj").InstantiateClone("footfast");
                footFast.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavefootfast");
                ww = footFast.GetComponent<WickedWeave>();
                ww.startTime = 0f;
                ww.hitboxEnd = 0.8f;
                shakeEmitter = footFast.GetComponent<ShakeEmitter>();
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
                loop = ScriptableObject.CreateInstance<LoopSoundDef>();
                loop.startSoundName = "weave";
                footFast.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
                footFast.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/ring").GetComponent<ParticleSystemRenderer>().material = mat;
                footFast.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/hair").GetComponent<ParticleSystemRenderer>().material = mat;

                fistDown = _assetBundle.LoadAsset<GameObject>("footproj").InstantiateClone("fastweaveHand");
                fistDown.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavehandDown");
                shakeEmitter = fistDown.GetComponent<ShakeEmitter>();
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
                loop = ScriptableObject.CreateInstance<LoopSoundDef>();
                loop.startSoundName = "weaved";
                fistDown.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
                fistDown.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/ring").GetComponent<ParticleSystemRenderer>().material = mat;
                fistDown.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/hair").GetComponent<ParticleSystemRenderer>().material = mat;
                ww = fistDown.GetComponent<WickedWeave>();
                ww.startTime = 0.32f;
                ww.hitboxEnd = 1.12f;
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
                shakeEmitter.shakeOnStart = false;
                shakeEmitter.shakeOnEnable = true;
                shakeEmitter.wave = new Wave
                {
                    amplitude = 2f,
                    frequency = 7f,
                    cycleOffset = 0f
                };
                LoopSoundDef loop = ScriptableObject.CreateInstance<LoopSoundDef>();
                loop.startSoundName = "weaved";
                fistProjectilePrefab.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
                fistProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/ring").GetComponent<ParticleSystemRenderer>().material = mat;
                fistProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/hair").GetComponent<ParticleSystemRenderer>().material = mat;

                fistFast = _assetBundle.LoadAsset<GameObject>("fistproj").InstantiateClone("fistfast");
                fistFast.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("weavehandfast");
                WickedWeave ww = fistFast.GetComponent<WickedWeave>();
                ww.startTime = 0f;
                ww.hitboxEnd = 0.8f;
                shakeEmitter = fistFast.GetComponent<ShakeEmitter>();
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
                loop = ScriptableObject.CreateInstance<LoopSoundDef>();
                loop.startSoundName = "weave";
                fistFast.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
                fistFast.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/ring").GetComponent<ParticleSystemRenderer>().material = mat;
                fistFast.GetComponent<ProjectileController>().ghostPrefab.transform.Find("portal/hair").GetComponent<ParticleSystemRenderer>().material = mat;
            }
        }
        #endregion 

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

        private static void CreateDemons()
        {
            summonHair = _assetBundle.LoadAsset<GameObject>("sumHair");
            summonHair.transform.Find("summonhair").gameObject.AddComponent<SummonHair>();

            gomorrah = _assetBundle.LoadAsset<GameObject>("gomorrah");
            gomorrah.gameObject.AddComponent<DemonVisualController>();
            gomorrah.gameObject.AddComponent<DemonController>();
            gomorrah.gameObject.AddComponent<NetworkIdentity>();
            DestroyOnTimer timer = gomorrah.gameObject.AddComponent<DestroyOnTimer>();
            timer.duration = 17.98f;

            Material mat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matNullifierGemPortal3.mat").WaitForCompletion();
            ChildLocator component = gomorrah.gameObject.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                int childIndex = component.FindChildIndex("portal");
                Transform transformm = component.FindChild(childIndex);
                transformm.Find("hair").GetComponent<ParticleSystemRenderer>().material = mat;
                transformm.Find("ring").GetComponent<ParticleSystemRenderer>().material = mat;
            }

            //profile = gomorrah.transform.Find("PP").gameObject.GetComponent<PostProcessVolume>().profile;
            profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            RampFog rf = profile.AddSettings<RampFog>();
            profile.name = "DemonPPEffect";
            rf.enabled.value = true;
            rf.fogIntensity.overrideState = true;
            rf.fogIntensity.value = 1f;
            rf.fogPower.overrideState = true;
            rf.fogPower.value = 1f;
            rf.fogZero.overrideState = true;
            rf.fogZero.value = 0f;
            rf.fogOne.overrideState = true;
            rf.fogOne.value = 0.163f;
            rf.fogHeightEnd.overrideState = false;
            rf.fogHeightIntensity.overrideState = false;
            rf.fogHeightStart.overrideState = false;
            rf.fogColorStart.overrideState = true;
            rf.fogColorStart.value = new Color(0f, 0f, 0f, 0f);
            rf.fogColorMid.overrideState = true;
            rf.fogColorMid.value = new Color(0f, 0f, 0f, 0f);
            rf.fogColorEnd.overrideState = true;
            rf.fogColorEnd.value = new Color(0.3886792f, 0.1400712f, 0.1400712f, 1f);
            rf.skyboxStrength.overrideState = true;
            rf.skyboxStrength.value = 0f;

            gomorrah.transform.Find("PP").gameObject.GetComponent<PostProcessVolume>().sharedProfile = profile;

            ContentAddition.AddNetworkedObject(gomorrah);
        }
    }
}
