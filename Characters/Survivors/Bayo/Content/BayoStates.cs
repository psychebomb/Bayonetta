using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Weave;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using BayoMod.Survivors.Bayo.SkillStates.PunishStates;

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

            Modules.Content.AddEntityState(typeof(Emote2));

            Modules.Content.AddEntityState(typeof(e2temp));

            Modules.Content.AddEntityState(typeof(Strut));

            Modules.Content.AddEntityState(typeof(StrutNew));

            Modules.Content.AddEntityState(typeof(LetsDance));

            Modules.Content.AddEntityState(typeof(PunishStun));

            Modules.Content.AddEntityState(typeof(PunishEntry));

            Modules.Content.AddEntityState(typeof(StepStart));

            Modules.Content.AddEntityState(typeof(Step));

            Modules.Content.AddEntityState(typeof(StepEnd));

            Modules.Content.AddEntityState(typeof(SmackStart));

            Modules.Content.AddEntityState(typeof(Smack));

            Modules.Content.AddEntityState(typeof(SmackEnd));

            Modules.Content.AddEntityState(typeof(GrabStart));

            Modules.Content.AddEntityState(typeof(Grab));

            Modules.Content.AddEntityState(typeof(Throw));
            #endregion
        }
    }
}
