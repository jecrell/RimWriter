using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter
{
    [StaticConstructorOnStartup]
    public static class RimWriterSetup
    {
        static RimWriterSetup()
        {
            var campFireDef = ThingDefOf.Campfire;
            var electricCrematoriumDef = ThingDef.Named("ElectricCrematorium");
            campFireDef?
                .recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnBooks"));
            electricCrematoriumDef?
                .recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnBooks"));
            campFireDef?
                .recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnScrolls"));
            electricCrematoriumDef?
                .recipes?.Add(DefDatabase<RecipeDef>.GetNamed("RimWriter_BurnScrolls"));
            Log.Message("Added recipes for burning books/scrolls successfully.");
            
            var cultsGrimoire = DefDatabase<ThingDef>.GetNamedSilentFail("Cults_Grimoire");
            if (cultsGrimoire != null)
            {
                cultsGrimoire.thingCategories = new List<ThingCategoryDef>
                {
                    ThingCategoryDef.Named("RimWriter_Books")
                };
            }
            var cultsKingInYellow = DefDatabase<ThingDef>.GetNamedSilentFail("Cults_TheKingInYellow");
            if (cultsKingInYellow != null)
            {
                cultsKingInYellow.thingCategories = new List<ThingCategoryDef>
                {
                    ThingCategoryDef.Named("RimWriter_Books")
                };
            }
        }
    }

    public static class ModProps
    {
        public static string main = "Jecrell";

        public static string mod = "RimWriter";
        //public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public class RimWriterUtility
    {
        private static bool loadedCosmicHorrors;
        private static bool loadedCults;
        private static bool modCheck;

        public const string SanityLossDef = "ROM_SanityLoss";
        public const string AltSanityLossDef = "Cults_SanityLoss";

        public static void TryGainLibraryThought(Pawn pawn)
        {
            Room room = pawn.GetRoom(RegionType.Set_Passable);
            if (room?.Role?.defName == "RimWriter_Library")
            {
                int scoreStageIndex =
                    RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
                var libraryThought = ThoughtDef.Named("RimWriter_ReadingInImpressiveLibrary");
                if (libraryThought?.stages[scoreStageIndex] != null)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(
                        ThoughtMaker.MakeThought(libraryThought, scoreStageIndex), null);
                }
            }
        }

        public static bool IsCosmicHorrorsLoaded()
        {
            if (!modCheck) ModCheck();
            return loadedCosmicHorrors;
        }

        public static bool IsCultsLoaded()
        {
            if (!modCheck) ModCheck();
            return loadedCults;
        }

        public static bool HasSanityLoss(Pawn pawn)
        {
            string sanityLossDef = (!IsCosmicHorrorsLoaded()) ? AltSanityLossDef : SanityLossDef;
            var pawnSanityHediff =
                pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed(sanityLossDef));

            return pawnSanityHediff != null;
        }


        /// <summary>
        /// This method handles the application of Sanity Loss in multiple mods.
        /// It returns true and false depending on if it applies successfully.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="sanityLoss"></param>
        /// <param name="sanityLossMax"></param>
        public static bool ApplySanityLoss(Pawn pawn, float sanityLoss = 0.3f, float sanityLossMax = 1.0f)
        {
            bool appliedSuccessfully = false;
            if (pawn != null)
            {
                string sanityLossDef = (!IsCosmicHorrorsLoaded()) ? AltSanityLossDef : SanityLossDef;

                var pawnSanityHediff =
                    pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef));
                if (pawnSanityHediff != null)
                {
                    if (pawnSanityHediff.Severity > sanityLossMax) sanityLossMax = pawnSanityHediff.Severity;
                    float result = pawnSanityHediff.Severity;
                    result += sanityLoss;
                    result = Mathf.Clamp(result, 0.0f, sanityLossMax);
                    pawnSanityHediff.Severity = result;
                    appliedSuccessfully = true;
                }
                else if (sanityLoss > 0)
                {
                    var sanityLossHediff =
                        HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef), pawn, null);
                    if (sanityLossHediff != null)
                    {
                        sanityLossHediff.Severity = sanityLoss;
                        pawn.health.AddHediff(sanityLossHediff, null, null);
                        appliedSuccessfully = true;
                    }
                }
            }

            return appliedSuccessfully;
        }

        /// <summary>
        /// This method handles the application of Sanity Loss in multiple mods.
        /// It returns true and false depending on if it applies successfully.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="sanityLoss"></param>
        /// <param name="sanityLossMax"></param>
        public static bool ReduceSanityLoss(Pawn pawn, float sanityRestored)
        {
            bool appliedSuccessfully = false;
            if (pawn != null)
            {
                string sanityLossDef = (!IsCosmicHorrorsLoaded()) ? AltSanityLossDef : SanityLossDef;

                var pawnSanityHediff =
                    pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef));
                if (pawnSanityHediff != null)
                {
                    float result = pawnSanityHediff.Severity;
                    result = result - sanityRestored;
                    pawnSanityHediff.Severity = result;
                    appliedSuccessfully = true;
                }
            }

            return appliedSuccessfully;
        }


        public static void ModCheck()
        {
            loadedCosmicHorrors = false;
            loadedCults = false;
            foreach (ModContentPack ResolvedMod in LoadedModManager.RunningMods)
            {
                if (loadedCosmicHorrors && loadedCults) break; //Save some loading
                if (ResolvedMod.Name.Contains("Call of Cthulhu - Cosmic Horrors"))
                {
                    DebugReport("Loaded - Call of Cthulhu - Cosmic Horrors");
                    loadedCosmicHorrors = true;
                }

                if (ResolvedMod.Name.Contains("Call of Cthulhu - Cults"))
                {
                    DebugReport("Loaded - Call of Cthulhu - Cults");
                    loadedCults = true;
                }
            }

            modCheck = true;
        }

        public static void DebugReport(string x)
        {
            if (Prefs.DevMode && DebugSettings.godMode)
            {
                Log.Message(Prefix + x);
            }
        }

        public static string Prefix => ModProps.main + " :: " + ModProps.mod + " :: ";
    }
}