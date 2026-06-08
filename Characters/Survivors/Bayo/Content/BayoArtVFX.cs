using AK.Wwise;
using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Modules;
using BayoMod.Modules.Components;
using BayoMod.Survivors.Bayo;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.ContentManagement;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;

namespace BayoMod.Characters.Bayo
{
    public static class BayoArtVFX
    {
        private static AssetBundle _assetBundle;

        private static Material fireMat;
        private static Material fireMatrixMat;
        private static Material starbusrtMat;

        public static GameObject p1f;
        public static GameObject p1af;
        public static GameObject p2f;
        public static GameObject p2af;
        public static GameObject p3f;
        public static GameObject p3af;
        public static GameObject p4f;
        public static GameObject p4af;
        public static GameObject pflurf;
        public static GameObject fireFist;
        public static GameObject fireFistSpawn;

        public static GameObject spinf;
        public static GameObject abkf;
        public static GameObject heelsf;
        public static GameObject heelkf;
        public static GameObject backkf;
        public static GameObject backsf;
        public static GameObject fallkf;
        public static GameObject fallef;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            MakeMaterials();

            MakeFireSwings();

            MakeFireFist();

            Content.AddProjectilePrefab(fireFist);
            PrefabAPI.RegisterNetworkPrefab(fireFist);

        }

        private static void MakeMaterials()
        {
            fireMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Wisp.matWispEmber_mat)).WaitForCompletion());
            fireMat.name = "matBayoFire";
            fireMat.mainTexture = _assetBundle.LoadAsset<Texture>("texBasicMask");
            fireMat.SetTexture("_Cloud1Tex", _assetBundle.LoadAsset<Texture>("testmask"));
            Vector4 scrollVec = new Vector4(0, 25, 4, 4);
            fireMat.SetVector("_CutoffScroll", scrollVec);

            fireMat.mainTextureOffset = new Vector2(0, 0.1f);
            fireMat.mainTextureScale = new Vector2(1, 0.8f);

            fireMatrixMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Mage.matMageMatrixTriFire_mat)).WaitForCompletion());
            fireMatrixMat.name = "matBayoFireMatrix";
            fireMatrixMat.mainTexture = _assetBundle.LoadAsset<Texture>("texmyawesomegradient");
            fireMatrixMat.SetTexture("_RemapTex", _assetBundle.LoadAsset<Texture>("texRampBayoFire"));
            fireMatrixMat.mainTextureScale = new Vector2(0, 0.6f);

            starbusrtMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.mageMageFireStarburst_mat)).WaitForCompletion());
            starbusrtMat.name = "matBayoStarburst";
            starbusrtMat.SetTextureOffset("_Cloud1Tex", new Vector2(0, -0.2f));
        }

        private static void MakeFireSwings()
        {
            p1f = _assetBundle.LoadAsset<GameObject>("m1p1f");
            p1f.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;

            p2f = _assetBundle.LoadAsset<GameObject>("m1p2f");
            p2f.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            MoveOffset mo = p2f.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;

            p3f = _assetBundle.LoadAsset<GameObject>("m1p3f");
            p3f.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            mo = p3f.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;
            mo.slideDur = 0.15f;

            p4f = _assetBundle.LoadAsset<GameObject>("m1p4f");
            p4f.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            mo = p4f.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = 0.3f;
            mo.idealOffset = -0.65f;
            mo.slideDur = 0.15f;

            p1af = _assetBundle.LoadEffect("m1p1af", true);
            p1af.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;

            p2af = _assetBundle.LoadEffect("m1p2af", true);
            p2af.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            p2af.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();

            p3af = _assetBundle.LoadEffect("m1p3af", true);
            p3af.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            p3af.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();

            p4af = _assetBundle.LoadEffect("m1p4af", true);
            p4af.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;

            pflurf = _assetBundle.LoadAsset<GameObject>("m1flurf");
            for (int i = 1; i < 4; ++i)
            {
                GameObject swing = pflurf.transform.Find("swing" + i.ToString()).gameObject;
                swing.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = swing.AddComponent<MoveOffset>();
                mo.slideDur = 0.15f;

                RotateSkulls skulls = swing.AddComponent<RotateSkulls>();
                if(i == 1)
                {
                    skulls.xMult = -0.02f;
                    skulls.yMult = -0.02f;
                    skulls.negXMult = -0.02f;
                    skulls.negYMult = -0.035f;
                }
                if(i == 3)
                {
                    skulls.xMult = -0.04f;
                    skulls.yMult = 0.01f;
                    skulls.negXMult = -0.04f;
                    skulls.negYMult = -0.0005f;
                }

                swing.transform.Find("skull").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Wisp.matWisp1_mat)).WaitForCompletion();
                swing.transform.Find("skull/impact").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.matGenericFire_mat)).WaitForCompletion();
            }

            spinf = _assetBundle.LoadAsset<GameObject>("spinf");
            for (int i = 1; i < 3; ++i)
            {
                spinf.transform.Find("swing" + i.ToString()).gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = spinf.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.8f;
                mo.slideDur = 0.125f;
            }

            p1f.gameObject.AddComponent<VFXrm>();
            p2f.gameObject.AddComponent<VFXrm>();
            p3f.gameObject.AddComponent<VFXrm>();
            p4f.gameObject.AddComponent<VFXrm>();
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.p1as, p1af);
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.p2as, p2af);
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.p3as, p3af);
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.p4as, p4af);

            abkf = _assetBundle.LoadAsset<GameObject>("abkf");
            abkf.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            abkf.transform.Find("wind").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.matOmniExplosion1_mat)).WaitForCompletion();
            ParticleSystemRenderer fireWave = abkf.transform.Find("wave").gameObject.GetComponent<ParticleSystemRenderer>();
            fireWave.material = starbusrtMat;
            GameObject temp = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_VoidJailer.voidjailer_cone_vfx_fbx)).WaitForCompletion();
            fireWave.mesh = temp.transform.Find("VoidJailerCone").gameObject.GetComponent<MeshFilter>().mesh;

            heelsf = _assetBundle.LoadAsset<GameObject>("heelsf");
            fireWave = heelsf.transform.Find("impact2").gameObject.GetComponent<ParticleSystemRenderer>();
            fireWave.material = starbusrtMat;
            temp = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_VoidJailer.voidjailer_cone_vfx_fbx)).WaitForCompletion();
            fireWave.mesh = temp.transform.Find("VoidJailerCone").gameObject.GetComponent<MeshFilter>().mesh;

            fallef = _assetBundle.LoadEffect("fallendf", true);
            fallef.transform.Find("swing1").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
            mo = fallef.transform.Find("swing1").gameObject.AddComponent<MoveOffset>();
            mo.startOffset = -3f;
            mo.idealOffset = 0;
            mo.atEnd = true;
            mo.atStart = false;
            mo.duration = 0.4f;
            mo.slideDur = 0.2f;

            heelkf = _assetBundle.LoadEffect("heelkickf", true);
            fallkf = _assetBundle.LoadAsset<GameObject>("fallkickf");
            backkf = _assetBundle.LoadAsset<GameObject>("backkick");
            backsf = _assetBundle.LoadAsset<GameObject>("backspin");
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.heelk, heelkf);
            SkinVFX.AddSkinVFX(BayoSurvivor.artSkin, BayoAssets.falle, fallef);

            for (int i = 1; i < 3; ++i)
            {
                heelkf.transform.Find("swing" + i.ToString()).gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = heelkf.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -0.5f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                backkf.transform.Find("swing" + i.ToString()).gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = backkf.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -1f;
                mo.slideDur = 0.35f;
            }

            for (int i = 1; i < 3; ++i)
            {
                backsf.transform.Find("swing" + i.ToString()).gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = backsf.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.2f;
            }

            for (int i = 1; i < 3; ++i)
            {
                fallkf.transform.Find("swing" + i.ToString()).gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                mo = fallkf.transform.Find("swing" + i.ToString()).gameObject.AddComponent<MoveOffset>();
                mo.startOffset = 0.3f;
                mo.idealOffset = -.8f;
                mo.slideDur = 0.3f;
            }

        }

        private static void MakeFireFist()
        {
            fireFist = _assetBundle.LoadAsset<GameObject>("fistproj").InstantiateClone("bayoFireFistProj");
            fireFist.GetComponent<ProjectileController>().ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("fireFist");
            WickedWeave ww = fireFist.GetComponent<WickedWeave>();
            ww.startTime = 0f;
            ww.hitboxEnd = 0.8f;
            ShakeEmitter shakeEmitter = fireFist.GetComponent<ShakeEmitter>();
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
            LoopSoundDef loop = ScriptableObject.CreateInstance<LoopSoundDef>();
            loop.startSoundName = "weave";
            fireFist.gameObject.GetComponent<ProjectileController>().flightSoundLoop = loop;
            GameObject fireGhost = fireFist.GetComponent<ProjectileController>().ghostPrefab;
            fireGhost.transform.Find("animRoot/Fist").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMatrixMat;
            fireGhost.transform.Find("animRoot/Fist/Tendrils/Right").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMatrixMat;
            fireGhost.transform.Find("animRoot/Fist/Tendrils/Left").gameObject.GetComponent<ParticleSystemRenderer>().material = fireMatrixMat;
            fireGhost.transform.Find("animRoot/Fist/Fist, Frensel").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Seeker.matSpiritPunchFistFresnel_mat)).WaitForCompletion();
            fireGhost.transform.Find("animRoot/Fist/Splashes").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.matGenericFire_mat)).WaitForCompletion();
            fireGhost.transform.Find("animRoot/Fist/Petals").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Seeker.matSeekerLotus_mat)).WaitForCompletion();

            fireFistSpawn = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Gravekeeper/MuzzleflashTrackingFireball.prefab").WaitForCompletion().InstantiateClone("bayoFireFistMuzzle", false);
            fireFistSpawn.transform.localScale *= 1.25f;
            GameObject glow = fireFistSpawn.transform.Find("Glow").gameObject;
            glow.transform.localScale *= 1.25f;
            glow.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.matArcaneCircleProvi_mat)).WaitForCompletion();
            var why = glow.GetComponent<ParticleSystem>().shape;
            why.alignToDirection = false;
            glow.GetComponent<ParticleSystemRenderer>().alignment = ParticleSystemRenderSpace.View;
            var whyyy = glow.GetComponent<ParticleSystem>().main;
            whyyy.startColor = new Color(1, 0.2f, 0.04339612f, 1);

            GameObject.Destroy(fireFistSpawn.transform.Find("Point Light").gameObject);
            var ring = fireFistSpawn.transform.Find("AreaIndicatorRing, Billboard").gameObject.GetComponent<ParticleSystem>().main;
            ring.startColor = new Color(1, 0.4078431f, 0.2470588f, 1);
            ring.startSize = 5;

            var soft = fireFistSpawn.transform.Find("Flash, Soft Glow").gameObject.GetComponent<ParticleSystem>().main;
            soft.startColor = new Color(1, 0.4158444f, 0.04339612f, 1);

            fireFistSpawn.transform.Find("Unscaled Flames").gameObject.GetComponent<ParticleSystemRenderer>().material = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.matGenericFire_mat)).WaitForCompletion();
            var flames = fireFistSpawn.transform.Find("Unscaled Flames").gameObject.GetComponent<ParticleSystem>().main;
            flames.startColor = new Color(1, 0.4158444f, 0.04339612f, 1);
            fireFistSpawn.transform.Find("Point Light").gameObject.GetComponent<Light>().color = new Color(1, 0.4158444f, 0.04339612f, 1);

            ContentAddition.AddEffect(fireFistSpawn);
        } 
    }
}
