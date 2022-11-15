using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimWriter;

[StaticConstructorOnStartup]
public static class RimWriterSetup
{
    static RimWriterSetup()
    {
        var campFireDef = ThingDefOf.Campfire;
        var electricCrematoriumDef = ThingDef.Named("ElectricCrematorium");
        campFireDef?.recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnBooks"));
        electricCrematoriumDef?.recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnBooks"));
        campFireDef?.recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnScrolls"));
        electricCrematoriumDef?.recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnScrolls"));
        Log.Message("Added recipes for burning books/scrolls successfully.");

        var cultsGrimoire = DefDatabase<ThingDef>.GetNamedSilentFail("Cults_Grimoire");
        if (cultsGrimoire != null)
        {
            cultsGrimoire.thingCategories = new List<ThingCategoryDef> { ThingCategoryDef.Named("RimWriter_Books") };
        }

        var cultsKingInYellow = DefDatabase<ThingDef>.GetNamedSilentFail("Cults_TheKingInYellow");
        if (cultsKingInYellow != null)
        {
            cultsKingInYellow.thingCategories = new List<ThingCategoryDef>
                { ThingCategoryDef.Named("RimWriter_Books") };
        }
    }
}