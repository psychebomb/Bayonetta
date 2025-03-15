using BayoMod.Survivors.Bayo.SkillStates.PunishStates;
using UnityEngine;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class SmackStart : StepStart
    {
        public override void OnEnter()
        {
            animName = "SpankStart";
            dur = 0.6f;
            atStart = 1.1f;
            stunTime = 4.28f;
            rotation = Quaternion.AngleAxis(165f, Vector3.up);
            z = -5.5f;
            x = 0f;
            y = -3f;
            strongModif = true;
            base.OnEnter();

        }

        protected override void SetNext()
        {
            outer.SetNextState(new Smack
            {
                cameraParamsOverrideHandle = cameraParamsOverrideHandle,
                enemyBody = enemyBody,
                forwardDir = characterDirection.forward,
                cameraDir = rotateAngle,
                lookY = lookY,
                oldDur = dur
            });
        }
    }
}