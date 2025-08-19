using RoR2;
using System.Linq;
using UnityEngine;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine.Networking;

namespace BayoMod.Modules.Components
{
    public class ClimaxTracker : PunishTracker
    {
        public bool hpDebug = true;
        private bool hpCheck = false;
        protected override void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = sort;
            this.search.maxDistanceFilter = 50f;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.filterByLoS = false;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();

            if(!hpDebug && this.trackingTarget)
            {
                if (this.trackingTarget.healthComponent.combinedHealth <= this.trackingTarget.healthComponent.body.maxHealth * 0.15f)
                {
                    hpCheck = true;
                }
                else
                {
                    hpCheck = false;
                }
            }
            else
            {
                hpCheck = true;
            }

            while (this.trackingTarget && (!this.trackingTarget.healthComponent.body.isChampion || !hpCheck || (this.trackingTarget.healthComponent.body.HasBuff(BayoBuffs.climaxed))))
            {
                this.search.FilterOutGameObject(this.trackingTarget.healthComponent.gameObject);
                this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
            }
        }
    }
}
