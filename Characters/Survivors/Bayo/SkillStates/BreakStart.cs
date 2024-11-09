
namespace BayoMod.Characters.Survivors.Bayo.SkillStates
{
    public class BreakStart : SpinStart
    {
        public override void OnEnter()
        {
            base.OnEnter();
            duration = 0.36f;
            PlayAnimation("Body", "BreakStart", "Slash.playbackRate", duration);
        }

        protected override void NextState()
        {
            outer.SetNextState(new Break());
        }
    }
}
