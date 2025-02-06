using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Weave;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoStates
    {
        public static void Init()
        {

            #region Base states
            Modules.Content.AddEntityState(typeof(BayoCharacterMain));

            Modules.Content.AddEntityState(typeof(BaseEmote));

            Modules.Content.AddEntityState(typeof(BasePunch));
            #endregion

            #region M1
            Modules.Content.AddEntityState(typeof(Punch1));

            Modules.Content.AddEntityState(typeof(Punch2));

            Modules.Content.AddEntityState(typeof(Punch3));

            Modules.Content.AddEntityState(typeof(Punch4));

            Modules.Content.AddEntityState(typeof(FlurryStart));

            Modules.Content.AddEntityState(typeof(Flurry));

            Modules.Content.AddEntityState(typeof(FlurryEnd));
            #endregion

            #region M2 Skills
            Modules.Content.AddEntityState(typeof(M2Entry));

            Modules.Content.AddEntityState(typeof(FallingKickStart));

            Modules.Content.AddEntityState(typeof(FallingKick));

            Modules.Content.AddEntityState(typeof(FallingKickEnd));

            Modules.Content.AddEntityState(typeof(HeelSlide));

            Modules.Content.AddEntityState(typeof(HeelKick));

            Modules.Content.AddEntityState(typeof(RisingKick));

            Modules.Content.AddEntityState(typeof(RisingFinisher));

            Modules.Content.AddEntityState(typeof(ABK));

            Modules.Content.AddEntityState(typeof(ABKEnd));

            Modules.Content.AddEntityState(typeof(SpinStart));

            Modules.Content.AddEntityState(typeof(Spin));

            Modules.Content.AddEntityState(typeof(BreakStart));

            Modules.Content.AddEntityState(typeof(Break));
            #endregion

            #region Special
            Modules.Content.AddEntityState(typeof(Tetsu));

            Modules.Content.AddEntityState(typeof(WeaveEntry));

            Modules.Content.AddEntityState(typeof(WeaveDummy));

            Modules.Content.AddEntityState(typeof(Stomp));
            #endregion

            #region Other
            Modules.Content.AddEntityState(typeof(Dodge));

            Modules.Content.AddEntityState(typeof(Stance));

            Modules.Content.AddEntityState(typeof(Emote1));

            Modules.Content.AddEntityState(typeof(Strut));
            #endregion
        }
    }
}
