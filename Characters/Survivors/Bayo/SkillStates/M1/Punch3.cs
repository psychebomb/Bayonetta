using BayoMod.Characters.Bayo;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Survivors.Bayo;
using RoR2;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.M1
{
    public class Punch3 : BasePunch
    {
        public override void OnEnter()
        {
            if (characterMotor.isGrounded)
            {
                animStart = "P3";
                animEnd = "P3E";
                earlyExitPercentTime = 0.32f;
                endDuration = 0.64f;
                playSwing = 0.22f;
                ReplacePrefab2(BayoAssets.p3s, BayoAssets.p3s2, BayoArtVFX.p3f);
            }
            else
            {
                animStart = "P3A";
                animEnd = "P3AE";
                earlyExitPercentTime = 0.16f;
                endDuration = 0.76f;
                playSwing = 0.2f;
                swingEffectPrefab = BayoAssets.p3as;
            }
            holdTime = 0.5f - earlyExitPercentTime;
            gunStr = "muzlh";
            voiceString = "pv3";
            swingSoundString = "p1p3";
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