using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
//using VerseBase;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace RimWriter
{
    public class JobDriver_FreeWrite : JobDriver
    {
        private float sanityRestoreRate = 0.025f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
            yield return Toils_Reserve.Reserve(TargetIndex.A, base.job.def.joyMaxParticipants);
            if (this.TargetB != null)
                yield return Toils_Reserve.Reserve(TargetIndex.B, 1);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell);
            Toil toil = new Toil();
            toil.PlaySustainerOrSound(TargetThingA?.def?.defName == "RimWriter_TableTypewriter"
                ? DefDatabase<SoundDef>.GetNamed("RimWriter_SoundManualTypewriter") : DefDatabase<SoundDef>.GetNamed(
                    "RimWriter_SoundManualPencil"));
            toil.tickAction = delegate
            {
                this.pawn.rotationTracker.FaceCell(this.TargetA.Cell);
                this.pawn.GainComfortFromCellIfPossible();
                float statValue = this.TargetThingA.GetStatValue(StatDefOf.JoyGainFactor, true);
                float extraJoyGainFactor = statValue;
                JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.GoToNextToil, extraJoyGainFactor);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = base.job.def.joyDuration;
            toil.AddFinishAction(delegate
            {
                 RimWriterUtility.TryGainLibraryThought(pawn);
            });
            yield return toil;
            Toil finishedToil = new Toil();
            finishedToil.initAction = delegate
            {
                if (RimWriterUtility.IsCosmicHorrorsLoaded() || RimWriterUtility.IsCultsLoaded())
                {
                    try
                    {
                        if (RimWriterUtility.HasSanityLoss(this.pawn))
                        {
                            RimWriterUtility.ApplySanityLoss(this.pawn, -sanityRestoreRate, 1);
                            Messages.Message(this.pawn.ToString() + " has restored some sanity using the " + this.TargetA.Thing.def.label + ".", new TargetInfo(this.pawn.Position, this.pawn.Map), MessageTypeDefOf.NeutralEvent);// .Standard);
                        }
                    }
                    catch
                    {
                        Log.Message("Error loading Sanity Hediff.");    
                    }
                }
            };
            yield return finishedToil;
            yield break;
        }
    }
}