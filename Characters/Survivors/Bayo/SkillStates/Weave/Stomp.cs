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
            Vector3 dir = Vector3.down;
            Vector3 pos = this.target.transform.position;
            pos.y = pos.y + 5;
            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
    }
}
