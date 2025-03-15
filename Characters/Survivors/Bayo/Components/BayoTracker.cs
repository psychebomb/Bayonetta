using RoR2;
using System.Linq;
using UnityEngine;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Modules.Components
{
    public class BayoTracker : MonoBehaviour
    {
        public float maxTrackingDistance = 50f;
        public float maxTrackingAngle = 45f;
        public float trackerUpdateFrequency = 10f;
        public BullseyeSearch.SortMode sort = BullseyeSearch.SortMode.Angle;

        private HurtBox trackingTarget;
        private CharacterBody characterBody;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        protected Indicator indicator;
        private readonly BullseyeSearch search = new BullseyeSearch();

        protected virtual void Awake()
        {
            this.indicator = new Indicator(base.gameObject, BayoAssets.trackerPrefab);
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void OnDestroy()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;

            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;

                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);

                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        protected virtual void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = sort;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }
    }
}