using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using BayoMod.Survivors.Bayo;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using BayoMod.Characters.Survivors.Bayo.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1_Alt
{
    public class PPKKK : FlurryEnd
    {
        CharacterCameraParams camParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        //BayoCameraRotater camRotater;
        public override void OnEnter()
        {
            attackStart = 0.55f;
            attackEnd = 0.75f;
            dur = 1.862f;
            animDur = 1.36f;
            exit = 1.31f;
            weaveDamage = 15f;
            weaveForce = 6000f;
            useSlowPrefabs = false;
            fastProj = BayoAssets.footForward;
            animGroundName = "ppkkk";
            rootMult = 2f;
            camTransIn = 0.9f;
            base.OnEnter();
        }
        protected override void FireProjectile()
        {
            dir = GetAimRay().direction;
            if (base.isAuthority)
            {
                Vector3 pos = this.gameObject.transform.position;
                pos.y -= 0.5f;
                dir.y = 0f;
                pos = pos + (dir.normalized * 3f);
                ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * weaveDamage, weaveForce, Util.CheckRoll(critStat, base.characterBody.master), DamageColorIndex.Default, null, -1, DamageTypeCombo.GenericPrimary);
            }
        }
    }
}