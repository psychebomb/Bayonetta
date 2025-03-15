using RoR2;
using EntityStates;
using UnityEngine;
using BayoMod.Modules.Components;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BayoCharacterMain : GenericCharacterMain
    {

        private BayoTracker tracker;
        private PunishTracker pTracker;
        private bool fallRemoved = false;
        public override void OnEnter()
        {
            pTracker = GetComponent<PunishTracker>();
            tracker = GetComponent<BayoTracker>();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {

            base.FixedUpdate();

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
                    EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetNextState(new Strut());
                    return;
                }
                if (Input.GetKeyDown(Modules.Config.emote4Keybind.Value))
                {
                    outer.SetNextState(new LetsDance());
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
        }

        public override void Update()
        {
            base.Update();
            useRootMotion = characterBody && characterBody.rootMotionInMainState;
        }

        public override void OnExit()
        {
            //if(pTracker) Destroy(pTracker);
            base.OnExit();
        }
    }
}
