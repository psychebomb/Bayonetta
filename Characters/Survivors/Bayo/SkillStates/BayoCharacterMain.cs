using RoR2;
using EntityStates;
using BayoMod.Survivors.Bayo.SkillStates;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class BayoCharacterMain : GenericCharacterMain
    {
        public override void OnEnter()
        {
            useRootMotion = true;
            base.OnEnter();
        }
        public override void FixedUpdate()
        {

            base.FixedUpdate();

            if (Util.HasEffectiveAuthority(base.gameObject) && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value))
                {
                    outer.SetNextState(new Emote1());
                    return;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            useRootMotion = (base.characterBody && base.characterBody.rootMotionInMainState);
        }
    }
}
