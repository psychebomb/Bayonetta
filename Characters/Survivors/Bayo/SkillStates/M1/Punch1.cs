using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch1 : BasePunch
    {
        public override void OnEnter()
        {
            earlyExitPercentTime = 0.24f;
            playSwing = 0.18f;
            if (characterMotor.isGrounded)
            {
                animStart = "P1";
                animEnd = "P1E";
                endDuration = 0.72f;
                ReplacePrefabs(BayoAssets.p1s, BayoAssets.p1s2);
            }
            else
            {
                animStart = "P1A";
                animEnd = "P1AE";
                endDuration = 0.68f;
                swingEffectPrefab = BayoAssets.p1as;
            }
            gunStr = "muzrh";
            holdTime = 0.5f - earlyExitPercentTime;
            voiceString = "p1v";
            swingSoundString = "p1p3";
            voice = true;
            base.OnEnter();
        }
    }
}
