using RoR2;
using EntityStates;
using UnityEngine;
using BayoMod.Modules.Components;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class BayoCharacterMain : GenericCharacterMain
    {

        private BayoTracker tracker;
        private bool fallRemoved = false;
        public override void OnEnter()
        {
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
                    EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetNextState(new Strut());
                    return;
                }
            }

            if (characterMotor.isGrounded && !fallRemoved)
            {
                characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                fallRemoved = true;
            }
        }

        public override void Update()
        {
            base.Update();
            useRootMotion = characterBody && characterBody.rootMotionInMainState;
        }
    }
}
