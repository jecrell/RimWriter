using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;
// using VerseBase;

namespace RimWriter;

public class JobDriver_FreeWrite : JobDriver
{
    private readonly float sanityRestoreRate = 0.025f;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.EndOnDespawnedOrNull(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.A, job.def.joyMaxParticipants);
        if (TargetB != null)
        {
            yield return Toils_Reserve.Reserve(TargetIndex.B);
        }

        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell);
        var toil = new Toil();
        toil.PlaySustainerOrSound(TargetThingA?.def?.defName == "RimWriter_TableTypewriter"
            ? DefDatabase<SoundDef>.GetNamed("RimWriter_SoundManualTypewriter")
            : DefDatabase<SoundDef>.GetNamed("RimWriter_SoundManualPencil"));
        toil.tickAction = delegate
        {
            pawn.rotationTracker.FaceCell(TargetA.Cell);
            pawn.GainComfortFromCellIfPossible();
            var statValue = TargetThingA.GetStatValue(StatDefOf.JoyGainFactor);
            JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.GoToNextToil, statValue);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Delay;
        toil.defaultDuration = job.def.joyDuration;
        toil.AddFinishAction(delegate { RimWriterUtility.TryGainLibraryThought(pawn); });
        yield return toil;
        var finishedToil = new Toil
        {
            initAction = delegate
            {
                if (!RimWriterUtility.IsCosmicHorrorsLoaded() && !RimWriterUtility.IsCultsLoaded())
                {
                    return;
                }

                try
                {
                    if (!RimWriterUtility.HasSanityLoss(pawn))
                    {
                        return;
                    }

                    RimWriterUtility.ApplySanityLoss(pawn, -sanityRestoreRate);
                    Messages.Message(
                        $"{pawn} has restored some sanity using the {TargetA.Thing.def.label}.",
                        new TargetInfo(pawn.Position, pawn.Map),
                        MessageTypeDefOf.NeutralEvent); // .Standard);
                }
                catch
                {
                    Log.Message("Error loading Sanity Hediff.");
                }
            }
        };
        yield return finishedToil;
    }
}