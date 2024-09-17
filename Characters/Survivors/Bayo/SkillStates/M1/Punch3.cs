using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch3 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.32f;
            holdTime = 0.18f;
            endDuration = 0.64f;
            animStart = "P3";
            animEnd = "P3E";
            base.OnEnter();
        }
    }
}