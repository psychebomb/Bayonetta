using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch1 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.24f;
            holdTime = 0.26f;
            endDuration = 0.72f;
            animStart = "P1";
            animEnd = "P1E";
            base.OnEnter();
        }
    }
}
