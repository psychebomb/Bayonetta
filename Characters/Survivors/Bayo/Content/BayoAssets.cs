using RoR2;
using UnityEngine;
using BayoMod.Modules;
using System;
using RoR2.Projectile;
using On.EntityStates.Seeker;
using R2API;
using UnityEngine.AddressableAssets;
using System.Xml.Linq;
using System.Security.Cryptography;
using UnityEngine.Networking;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;

        public static GameObject fistProjectilePrefab;

        public static GameObject bulletMuz;

        private static AssetBundle _assetBundle;

        internal static GameObject trackerPrefab;

        internal static GameObject wardPrefab;

        internal static GameObject railPrefab;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("BayoSwordHit");

            CreateEffects();

            CreateProjectiles();

            CreateTracker();

        }




        #region effects
        private static void CreateEffects()
        {
            CreateBombExplosionEffect();

            swordSwingEffect = _assetBundle.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = _assetBundle.LoadEffect("ImpactHenrySlash");
            bulletMuz = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion().InstantiateClone("BayoMuzz", false);
            UnityEngine.Object.Destroy(bulletMuz.GetComponent<VFXAttributes>());
            UnityEngine.Object.Destroy(bulletMuz.GetComponent<ShakeEmitter>());
            ContentAddition.AddEffect(bulletMuz);
        }

        private static void CreateBombExplosionEffect()
        {
            bombExplosionEffect = _assetBundle.LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (!bombExplosionEffect)
                return;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 40f,
                cycleOffset = 0f
            };

        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateBombProjectile();
            CreateFistProjectile();
            CreateWard();
            Content.AddProjectilePrefab(bombProjectilePrefab);
            Content.AddProjectilePrefab(fistProjectilePrefab);
            ContentAddition.AddNetworkedObject(wardPrefab);
        }

        private static void CreateBombProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            bombProjectilePrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            //remove their ProjectileImpactExplosion component and start from default values
            UnityEngine.Object.Destroy(bombProjectilePrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileImpactExplosion bombImpactExplosion = bombProjectilePrefab.AddComponent<ProjectileImpactExplosion>();
            
            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.blastDamageCoefficient = 1f;
            bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = bombExplosionEffect;
            bombImpactExplosion.lifetimeExpiredSound = Content.CreateAndAddNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("HenryBombGhost") != null)
                bombController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("HenryBombGhost");
            
            bombController.startSound = "";
        }

        private static void CreateFistProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            //fistProjectilePrefab = Asset.CloneProjectilePrefab("UnseenHandMovingProjectile", "FistProjectile");
            //RoR2/Base/Bandit2/Bandit2SlashBlade.prefab
            //Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SlashBlade.prefab").WaitForCompletion()
            fistProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/UnseenHandMovingProjectile.prefab").WaitForCompletion().InstantiateClone("WeaveProjectile", false);
            //remove their ProjectileImpactExplosion component and start from default values
            UnityEngine.Object.Destroy(fistProjectilePrefab.GetComponent<ProjectileSimple>());
            UnityEngine.Object.Destroy(fistProjectilePrefab.GetComponent<RotateObject>());
            UnityEngine.Object.Destroy(fistProjectilePrefab.GetComponent<UnseenHandHealingProjectile>());
            ProjectileSimple fistSimple = fistProjectilePrefab.AddComponent<ProjectileSimple>();

            fistSimple.lifetime = 0.5f;
            fistSimple.desiredForwardSpeed = 1;
            fistSimple.updateAfterFiring = true;
            fistSimple.enableVelocityOverLifetime = true;
            fistSimple.velocityOverLifetime = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.075f, 60), new Keyframe(0.2f, 60), new Keyframe(0.205f, -60), new Keyframe(0.25f, -60), new Keyframe(0.255f, 0) });

            ProjectileController fistController = fistProjectilePrefab.GetComponent<ProjectileController>();

            ShakeEmitter shakeEmitter = fistProjectilePrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.4f;
            shakeEmitter.radius = 100f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 30f,
                cycleOffset = 0f
            };

            fistController.startSound = "";
        }
        #endregion projectiles

        private static void CreateTracker()
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
        }

        private static void CreateWard()
        {
            /*
            wardPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/AffixHauntedWard.prefab").WaitForCompletion().InstantiateClone("WtWardPrefab", false);
            railPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion();
            //UnityEngine.Object.Destroy(wardPrefab.GetComponent<BuffWard>());

            BuffWard buffWard = wardPrefab.GetComponent<BuffWard>();

            buffWard.radius = 25f;
            buffWard.buffDef = BayoBuffs.wtDebuff;
            buffWard.invertTeamFilter = true;

            wardPrefab.GetComponent<TeamFilter>().defaultTeam = TeamIndex.Player;

            NetworkIdentity ni = wardPrefab.GetComponent<NetworkIdentity>();

            //ni.localPlayerAuthority = true;

            SphereCollider sc = wardPrefab.AddComponent<SphereCollider>();

            sc.isTrigger = true;
            sc.radius = 25f;
            sc.center = Vector3.zero;


            wardPrefab.AddComponent<SlowDownProjectiles>();
            wardPrefab.GetComponent<SlowDownProjectiles>().teamFilter = wardPrefab.GetComponent<TeamFilter>();
            wardPrefab.GetComponent<SlowDownProjectiles>().slowDownCoefficient = 0.1f;

            //sdp.teamFilter = wardPrefab.GetComponent<TeamFilter>();

            //sdp.Equals(sdp1);

            //sdp.slowDownCoefficient = 0.1f;
            */
            wardPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion().InstantiateClone("WtWardPrefab", false);

            BuffWard buffWard = wardPrefab.GetComponent<BuffWard>();

            buffWard.radius = 25f;
            buffWard.buffDef = BayoBuffs.wtDebuff;
            buffWard.invertTeamFilter = true;
            buffWard.interval = 0.25f;

            SphereCollider sc = wardPrefab.GetComponent<SphereCollider>();

            sc.radius = 25f;

            wardPrefab.GetComponent<TeamFilter>().defaultTeam = TeamIndex.Player;

            wardPrefab.AddComponent<NetworkedBodyAttachment>();
            wardPrefab.GetComponent<NetworkedBodyAttachment>().shouldParentToAttachedBody = true;

            wardPrefab.GetComponent<SlowDownProjectiles>().slowDownCoefficient = 0.075f;
        }

    }
}
