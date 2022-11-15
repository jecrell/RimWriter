using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter;

public class RimWriterUtility
{
    public const string AltSanityLossDef = "Cults_SanityLoss";

    public const string SanityLossDef = "ROM_SanityLoss";

    private static bool loadedCosmicHorrors;

    private static bool loadedCults;

    private static bool modCheck;

    public static string Prefix => $"{ModProps.main} :: {ModProps.mod} :: ";

    /// <summary>
    ///     This method handles the application of Sanity Loss in multiple mods.
    ///     It returns true and false depending on if it applies successfully.
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="sanityLoss"></param>
    /// <param name="sanityLossMax"></param>
    public static void ApplySanityLoss(Pawn pawn, float sanityLoss = 0.3f, float sanityLossMax = 1.0f)
    {
        if (pawn == null)
        {
            return;
        }

        var sanityLossDef = !IsCosmicHorrorsLoaded() ? AltSanityLossDef : SanityLossDef;

        var pawnSanityHediff =
            pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef));
        if (pawnSanityHediff != null)
        {
            if (pawnSanityHediff.Severity > sanityLossMax)
            {
                sanityLossMax = pawnSanityHediff.Severity;
            }

            var result = pawnSanityHediff.Severity;
            result += sanityLoss;
            result = Mathf.Clamp(result, 0.0f, sanityLossMax);
            pawnSanityHediff.Severity = result;
        }
        else if (sanityLoss > 0)
        {
            var sanityLossHediff =
                HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef), pawn);
            if (sanityLossHediff == null)
            {
                return;
            }

            sanityLossHediff.Severity = sanityLoss;
            pawn.health.AddHediff(sanityLossHediff);
        }
    }

    public static void DebugReport(string x)
    {
        if (Prefs.DevMode && DebugSettings.godMode)
        {
            Log.Message(Prefix + x);
        }
    }

    public static bool HasSanityLoss(Pawn pawn)
    {
        var sanityLossDef = !IsCosmicHorrorsLoaded() ? AltSanityLossDef : SanityLossDef;
        var pawnSanityHediff =
            pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed(sanityLossDef));

        return pawnSanityHediff != null;
    }

    public static bool IsCosmicHorrorsLoaded()
    {
        if (!modCheck)
        {
            ModCheck();
        }

        return loadedCosmicHorrors;
    }

    public static bool IsCultsLoaded()
    {
        if (!modCheck)
        {
            ModCheck();
        }

        return loadedCults;
    }

    public static void ModCheck()
    {
        loadedCosmicHorrors = false;
        loadedCults = false;
        foreach (var ResolvedMod in LoadedModManager.RunningMods)
        {
            if (loadedCosmicHorrors && loadedCults)
            {
                break; // Save some loading
            }

            if (ResolvedMod.Name.Contains("Call of Cthulhu - Cosmic Horrors"))
            {
                DebugReport("Loaded - Call of Cthulhu - Cosmic Horrors");
                loadedCosmicHorrors = true;
            }

            if (!ResolvedMod.Name.Contains("Call of Cthulhu - Cults"))
            {
                continue;
            }

            DebugReport("Loaded - Call of Cthulhu - Cults");
            loadedCults = true;
        }

        modCheck = true;
    }

    /// <summary>
    ///     This method handles the application of Sanity Loss in multiple mods.
    ///     It returns true and false depending on if it applies successfully.
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="sanityRestored"></param>
    public static void ReduceSanityLoss(Pawn pawn, float sanityRestored)
    {
        if (pawn == null)
        {
            return;
        }

        var sanityLossDef = !IsCosmicHorrorsLoaded() ? AltSanityLossDef : SanityLossDef;

        var pawnSanityHediff =
            pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail(sanityLossDef));
        if (pawnSanityHediff == null)
        {
            return;
        }

        var result = pawnSanityHediff.Severity;
        result -= sanityRestored;
        pawnSanityHediff.Severity = result;
    }

    public static void TryGainLibraryThought(Pawn pawn)
    {
        var room = pawn.GetRoom();
        if (room?.Role?.defName != "RimWriter_Library")
        {
            return;
        }

        var scoreStageIndex =
            RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
        var libraryThought = ThoughtDef.Named("RimWriter_ReadingInImpressiveLibrary");
        if (libraryThought?.stages[scoreStageIndex] != null)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(libraryThought,
                scoreStageIndex));
        }
    }
}