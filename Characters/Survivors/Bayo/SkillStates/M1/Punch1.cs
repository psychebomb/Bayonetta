using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch1 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.24f;
            if (characterMotor.isGrounded)
            {
                animStart = "P1";
                animEnd = "P1E";
                endDuration = 0.72f;
            }
            else
            {
                animStart = "P1A";
                animEnd = "P1AE";
                endDuration = 0.68f;
            }
            gunStr = "gunrh4";
            holdTime = 0.5f - earlyExitPercentTime;
            base.OnEnter();
        }
    }
}
