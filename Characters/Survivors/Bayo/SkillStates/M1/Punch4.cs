using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch4 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.4f;
            holdTime = 0.1f;
            endDuration = 0.52f;
            animStart = "P4";
            animEnd = "P4E";
            base.OnEnter();
        }
    }
}