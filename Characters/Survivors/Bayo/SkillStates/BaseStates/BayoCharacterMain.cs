using RoR2;
using EntityStates;
using UnityEngine;
using BayoMod.Modules.Components;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BayoCharacterMain : GenericCharacterMain
    {

        private BayoTracker tracker;
        private PunishTracker pTracker;
        private bool fallRemoved = false;
        private GameObject wingPrefab;
        private GameObject wingInstance;
        private GameObject jumpPrefab = BayoAssets.djump;
        public override void OnEnter()
        {
            pTracker = GetComponent<PunishTracker>();
            tracker = GetComponent<BayoTracker>();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            if (tracker) Destroy(tracker);

            if (Util.HasEffectiveAuthority(gameObject) && characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value))
                {
                    outer.SetNextState(new Emote1());
                    return;
                }
                if (Input.GetKeyDown(Modules.Config.emote2Keybind.Value))
                {
                    outer.SetNextState(new Emote2());
                    return;
                }
                if (Input.GetKeyDown(Modules.Config.emote3Keybind.Value))
                {
                    //EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetNextState(new StrutNew());
                    outer.SetNextState(new StrutNew());
                    return;
                }
                if (Input.GetKeyDown(Modules.Config.emote4Keybind.Value))
                {
                    outer.SetNextState(new LetsDance());
                    return;
                }
                if (Input.GetKeyDown(Modules.Config.emote5Keybind.Value))
                {
                    outer.SetNextState(new Kiss());
                    return;
                }
                if (pTracker)
                {
                    if (pTracker.GetTrackingTarget() && inputBank.interact.down)
                    {
                        outer.SetNextState(new PunishEntry());
                        return;
                    }
                }
            }

            if (characterMotor.isGrounded)
            {
                if (!fallRemoved)
                {
                    characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                    fallRemoved = true;
                }
                
                if(pTracker) pTracker.enabled = true;
            }
            if (pTracker && !characterMotor.isGrounded)
            {
                pTracker.enabled = false;
            }

            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();
            useRootMotion = characterBody && characterBody.rootMotionInMainState;
        }
        public override void ProcessJump()
        {
            if (!hasCharacterMotor)
            {
                return;
            }
            if (!jumpInputReceived || !base.characterBody || base.characterMotor.jumpCount >= base.characterBody.maxJumpCount)
            {
                return;
            }
            if (base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1)
            {
                //future single jump effects go here idk im lazy
            }
            else
            {
                if(wingInstance) UnityEngine.Object.Destroy(wingInstance);
                wingPrefab = BayoAssets.bwings;
                UnityEngine.Object.Destroy(wingPrefab.GetComponent<WingComponent>());
                wingPrefab.AddComponent<WingComponent>();
                ChildLocator childLocator = GetModelChildLocator();
                if (childLocator)
                {
                    Transform transform = childLocator.FindChild("WingCenter") ?? base.characterBody.coreTransform;
                    if (transform)
                    {
                        wingInstance = Object.Instantiate(wingPrefab, transform.position, transform.rotation);
                        wingInstance.transform.parent = transform;
                    }
                }
                //EffectManager.SimpleMuzzleFlash(wingPrefab, gameObject, "WingCenter", true);
                EffectManager.SimpleMuzzleFlash(jumpPrefab, gameObject, "SwingCenter", true);
            }
            base.ProcessJump();
            
        }
        public override void OnExit()
        {
            //if (wingInstance) UnityEngine.Object.Destroy(wingInstance);
            //if(pTracker) Destroy(pTracker);
            base.OnExit();
        }
    }
}
