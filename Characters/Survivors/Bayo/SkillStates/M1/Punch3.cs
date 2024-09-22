using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch3 : BasePunch
    {
        public override void OnEnter()
        {
            if (characterMotor.isGrounded)
            {
                animStart = "P3";
                animEnd = "P3E";
                earlyExitPercentTime = 0.32f;
                endDuration = 0.64f;
            }
            else
            {
                animStart = "P3A";
                animEnd = "P3AE";
                earlyExitPercentTime = 0.16f;
                endDuration = 0.76f;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "gunlh4";
            base.OnEnter();
        }
    }
}