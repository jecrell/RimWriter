using System;
using Verse;

namespace ArkhamEstate
{
    public class CompProperties_Glower_StreetLamp : CompProperties
    {
        public float overlightRadius;

        public float glowRadius = 14f;

        public ColorInt glowColor = new ColorInt(255, 255, 255, 0) * 1.45f;

        public CompProperties_Glower_StreetLamp()
        {
            this.compClass = typeof(ArkhamEstate.CompGlower_StreetLamp);
        }
    }
}