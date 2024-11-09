using RoR2;
using EntityStates;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine;
using BayoMod.Modules.Components;
using BayoMod.Survivors.Bayo;



namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class BayoCharacterMain : GenericCharacterMain
    {

        private BayoTracker tracker;
        private bool fallRemoved = false;
        public override void OnEnter()
        {
            useRootMotion = true;
            this.tracker = base.GetComponent<BayoTracker>();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {

            base.FixedUpdate();

            if (this.tracker) Destroy(this.tracker);

            if (Util.HasEffectiveAuthority(base.gameObject) && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value))
                {
                    outer.SetNextState(new Emote1());
                    return;
                }
            }

            if (base.characterMotor.isGrounded && !fallRemoved)
            {
                characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                fallRemoved = true;
            }
        }

        public override void Update()
        {
            base.Update();
            useRootMotion = (base.characterBody && base.characterBody.rootMotionInMainState);
        }
    }
}
