using BayoMod.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class Smack : Step
    {

        public override void OnEnter()
        {
            attackStart = 0.05454f;
            rotation = Quaternion.AngleAxis(165f, Vector3.up);
            zoomOut = 0.5f;
            base.OnEnter();
        }
        protected override void PlayAnim()
        {
            PlayAnimation("Body", "Spank", "Slash.playbackRate", fireFreq * 2);
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = characterMotor.velocity * 0f;
            }
        }

        protected override void SetNext()
        {
            outer.SetNextState(new SmackEnd
            {
                cameraParamsOverrideHandle = cameraParamsOverrideHandle,
                enemyBody = enemyBody,
                forwardDir = characterDirection.forward,
                cameraDir = cameraDir,
            });
        }
    }
}
