using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using BayoMod.Survivors.Bayo;
using RoR2;
using UnityEngine;
using RoR2.Projectile;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1_Alt
{
    public class PPKK : FlurryEnd
    {
        public override void OnEnter()
        {
            attackStart = 0.28f;
            dur = 1.62f;
            animDur = 0.88f;
            exit = 0.84f;
            weaveDamage = 10f;
            weaveForce = 1f;
            useSlowPrefabs = false;
            fastProj = BayoAssets.footFast;
            animGroundName = "ppkk";
            rootMult = 2f;
            camTransIn = 0.3f;
            camTransOut = 0.9f;
            camY = 1f;
            camZ = -16f;
            base.OnEnter();
        }
        protected override void FireProjectile()
        {
            dir = GetAimRay().direction;
            if (base.isAuthority)
            {
                dir.y = 0;
                //Quaternion rotation = Quaternion.AngleAxis(10f, Vector3.up);
                Vector3 pos = characterBody.transform.position + ((dir).normalized * 4f);
                pos.y = pos.y - 1.5f;
                //rotation = Quaternion.AngleAxis(30f, Vector3.up);
                //dir = rotation * dir;
                ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * weaveDamage, weaveForce, Util.CheckRoll(critStat, base.characterBody.master), DamageColorIndex.Default, null, -1, DamageTypeCombo.GenericPrimary);
                dir = GetAimRay().direction;
            }
        }

        protected override void SetStep()
        {
            outer.SetNextState(new PPKKK
            {
                inAltPath = false,
                altSwing = 0
            });
        }
    }
}
