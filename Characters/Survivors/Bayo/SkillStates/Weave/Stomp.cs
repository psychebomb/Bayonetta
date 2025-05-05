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
            BaseDelayDuration = 0.16f;
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
            Vector3 pos = this.target.transform.position;
            pos.y = pos.y - 2.5f;
            //if (this.target.healthComponent.body.characterMotor && this.target.healthComponent.body.HasBuff(BayoBuffs.wtDebuff))
           // {
              //  force /= 100f;
           //     force *= this.target.healthComponent.body.characterMotor.mass;
          //  }
            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
    }
}
