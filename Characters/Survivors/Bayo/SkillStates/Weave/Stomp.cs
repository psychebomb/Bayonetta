using EntityStates;
using BayoMod.Modules.Components;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Weave
{
    public class Stomp : Tetsu
    {
        public override void OnEnter()
        {
            baseDuration = 1.68f;
            startDuration = 0.5f;
            BaseDelayDuration = 0.4f;
            fForce = 1f;
            voiceString = "stompabk";
            projpref = BayoAssets.footProjectilePrefab;

            base.OnEnter();

        }

        //public override void PlayEndAnim()
        //{
        //}
        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Body", "Stomp", "Slash.playbackRate", baseDuration);
            }
        }

        public override void Fire()
        {
            Vector3 dir = GetAimRay().direction;
            dir.y = 0;
            Vector3 pos = characterBody.transform.position + (dir.normalized * 2f);
            pos.y = pos.y - 1.5f;
            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
    }
}
