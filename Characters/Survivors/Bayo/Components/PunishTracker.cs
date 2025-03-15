using RoR2;
using System.Linq;
using UnityEngine;
using BayoMod.Survivors.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;

namespace BayoMod.Modules.Components
{
    public class PunishTracker : MonoBehaviour
    {
        public float maxTrackingDistance = 10f;
        public float maxTrackingAngle = 60f;
        public float trackerUpdateFrequency = 10f;
        public BullseyeSearch.SortMode sort = BullseyeSearch.SortMode.Distance;

        private HurtBox trackingTarget;
        private CharacterBody characterBody;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private GameObject curTarget = null;
        private readonly BullseyeSearch search = new BullseyeSearch();
        private GameObject evil;
        public bool punishing = false;
        private bool highRemoved = false;

        protected virtual void Awake()
        {
            evil = Object.Instantiate(BayoAssets.evilObject);
            //evil.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.characterBody.gameObject);
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();

            if (this.characterBody && this.characterBody.gameObject)
            {
                //evil = Object.Instantiate(BayoAssets.evilObject);
                evil.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.characterBody.gameObject);
            }

        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }
        private void OnEnable()
        {
            if (!evil && this.characterBody && this.characterBody.gameObject)
            {
                //evil = Object.Instantiate(BayoAssets.evilObject);
                evil.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.characterBody.gameObject);
            }
            if (evil) evil.SetActive(true);
        }
        private void OnDisable()
        {
            if (curTarget != null)
            {
                int numHigh = curTarget.gameObject.GetComponents<Highlight>().Length;
                for (int i = 0; i < numHigh; i++)
                {
                    Destroy(curTarget.gameObject.GetComponents<Highlight>()[i]);
                }
            }
            if (evil) evil.SetActive(false);
        }

        private void OnDestroy()
        {
            if (curTarget != null)
            {
                int numHigh = curTarget.gameObject.GetComponents<Highlight>().Length;
                for (int i = 0; i < numHigh; i++)
                {
                    Destroy(curTarget.gameObject.GetComponents<Highlight>()[i]);
                }
            }
            if (evil) evil.SetActive(false);
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;

            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency && !punishing)
            {
                highRemoved = false;
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;

                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);

                if(curTarget != null)
                {
                    int numHigh = curTarget.gameObject.GetComponents<Highlight>().Length;
                    //Chat.AddMessage(numHigh.ToString());
                    for(int i = 0;i< numHigh; i++)
                    {
                        Destroy(curTarget.gameObject.GetComponents<Highlight>()[i]);
                    }
                }
                if (this.trackingTarget && this.trackingTarget.healthComponent.body.modelLocator)
                {
                    AddInteract();
                }
                else
                {
                    if(evil) evil.GetComponent<GenericInteraction>().interactability = Interactability.Disabled;
                }
            }
            if (punishing)
            {
                if (curTarget != null && !highRemoved)
                {
                    highRemoved = true;
                    int numHigh = curTarget.gameObject.GetComponents<Highlight>().Length;
                    //Chat.AddMessage(numHigh.ToString());
                    for (int i = 0; i < numHigh; i++)
                    {
                        Destroy(curTarget.gameObject.GetComponents<Highlight>()[i]);
                    }
                }
                if (evil) evil.GetComponent<GenericInteraction>().interactability = Interactability.Disabled;
            }
        }

        private void AddInteract()
        {
            Transform modelTransform = this.trackingTarget.healthComponent.body.modelLocator.modelTransform;
            if (modelTransform)
            {
                CharacterModel component4 = modelTransform.GetComponent<CharacterModel>();
                if (component4)
                {
                    CharacterModel.RendererInfo[] baseRendererInfos = component4.baseRendererInfos;
                    for (int i = 0; i < baseRendererInfos.Length; i++)
                    {
                        CharacterModel.RendererInfo rendererInfo = baseRendererInfos[i];
                        if (!rendererInfo.ignoreOverlays)
                        {
                            curTarget = this.trackingTarget.healthComponent.body.gameObject;
                            Highlight hl = curTarget.AddComponent<Highlight>();
                            hl.highlightColor = Highlight.HighlightColor.teleporter;
                            hl.targetRenderer = rendererInfo.renderer;
                            hl.strength = 1f;
                            hl.isOn = true;
                        }
                    }
                    if(!punishing && evil) if (evil) evil.GetComponent<GenericInteraction>().interactability = Interactability.Available;
                }
            }
        }

        static void SetBayoState(Interactor activator)
        {

            if (activator.GetComponent<CharacterBody>() && activator.GetComponent<CharacterBody>().gameObject.name.Contains("BayoBody"))
            {
                EntityStateMachine component = activator.GetComponent<EntityStateMachine>();
                if ((bool)component)
                {
                    component.SetNextState(new PunishEntry());
                }
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
            this.search.filterByLoS = false;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
            while (this.trackingTarget && (!this.trackingTarget.healthComponent.body.HasBuff(BayoBuffs.punishable)))// || !this.trackingTarget.healthComponent.body.GetComponent<CapsuleCollider>()))
            {
                this.search.FilterOutGameObject(this.trackingTarget.healthComponent.gameObject);
                this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
            }
        }
    }
}
