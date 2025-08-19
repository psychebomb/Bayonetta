using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;

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
                playSwing = 0.34f;
                ReplacePrefab2(BayoAssets.p4s, BayoAssets.p4s2, BayoAssets.p4art);
            }
            else
            {
                animStart = "P4A";
                animEnd = "P4AE";
                earlyExitPercentTime = 0.22f;
                endDuration = 0.56f;
                playSwing = 0.22f;
                swingEffectPrefab = BayoAssets.p4as;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "muzrh";
            voiceString = "pv4";
            swingSoundString = "p4";
            voice = true;
            base.OnEnter();
            juggleHop = 12f;
        }
    }
}