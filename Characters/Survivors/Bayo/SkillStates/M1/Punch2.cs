using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch2 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.26f;
            holdTime = 0.24f;
            endDuration = 0.48f;
            animStart = "P2";
            animEnd = "P2E";
            base.OnEnter();
        }
    }
}