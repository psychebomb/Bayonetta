using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch2 : BasePunch
    {
        public override void OnEnter()
        {
            if (characterMotor.isGrounded)
            {
                animStart = "P2";
                animEnd = "P2E";
                earlyExitPercentTime = 0.26f;
                endDuration = 0.48f;
            }
            else
            {
                animStart = "P2A";
                animEnd = "P2AE";
                earlyExitPercentTime = 0.24f;
                endDuration = 0.44f;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "gunlh4";
            base.OnEnter();
        }
    }
}