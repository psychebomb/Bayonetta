using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch4 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.4f;
            endDuration = 0.52f;
            if (characterMotor.isGrounded)
            {
                animStart = "P4";
                animEnd = "P4E";
                earlyExitPercentTime = 0.4f;
                endDuration = 0.52f;
            }
            else
            {
                animStart = "P4A";
                animEnd = "P4AE";
                earlyExitPercentTime = 0.22f;
                endDuration = 0.56f;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "muzrh";
            base.OnEnter();
            juggleHop = 12f;
        }
    }
}