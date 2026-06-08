using BayoMod.Characters.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch2 : BasePunch
    {
        public override void OnEnter()
        {
            if (characterMotor.isGrounded)
            {
                animStart = "P2";
                animEnd = "P2E";
                earlyExitPercentTime = 0.26f;
                endDuration = 0.48f;
                ReplacePrefab2(BayoAssets.p2s, BayoAssets.p2s2, BayoArtVFX.p2f);
                playSwing = 0.2f;
            }
            else
            {
                animStart = "P2A";
                animEnd = "P2AE";
                earlyExitPercentTime = 0.24f;
                endDuration = 0.44f;
                swingEffectPrefab = BayoAssets.p2as;
                playSwing = 0.22f;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "muzlh";
            voiceString = "p2v";
            swingSoundString = "p2";
            animDur = earlyExitPercentTime * 2f;
            voice = true;

            SkinDef curSkin = SkinCatalog.FindCurrentSkinDefForBodyInstance(this.characterBody.gameObject);
            if (curSkin == BayoSurvivor.artSkin)
            {
                gunName = gunName + "art";
            }
            base.OnEnter();
        }
    }
}