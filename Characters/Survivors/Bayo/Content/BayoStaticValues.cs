using System;
using System.Collections.Generic;

namespace BayoMod.Survivors.Bayo
{
    public static class BayoStaticValues
    {
        public const float swordDamageCoefficient = 2.8f;

        public const float gunDamageCoefficient = 0.5f;

        internal static List<Type> BLACKLIST_STATES = new List<Type>();
        public static void AddBlacklistStates()
        {
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelState));
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelEnterState));
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelExitState));
        }
    }
}