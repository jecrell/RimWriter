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
        private HediffDef sanityLossHediff;
        private float sanityRestoreRate = 0.1f;
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
            toil.PlaySustainerOrSound(DefDatabase<SoundDef>.GetNamed("RimWriter_SoundManualTypewriter"));
            toil.tickAction = delegate
            {
                this.pawn.rotationTracker.FaceCell(this.TargetA.Cell);
                this.pawn.GainComfortFromCellIfPossible();
                float statValue = this.TargetThingA.GetStatValue(StatDefOf.JoyGainFactor, true);
                float extraJoyGainFactor = statValue;
                JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = base.job.def.joyDuration;
/*            toil.AddFinishAction(delegate
            {
                if (Cthulhu.Utility.IsCosmicHorrorsLoaded())
                {
                    try
                    {
                        if (Cthulhu.Utility.HasSanityLoss(this.pawn))
                        {
                            Cthulhu.Utility.ApplySanityLoss(this.pawn, -sanityRestoreRate, 1);
                            Messages.Message(this.pawn.ToString() + " has restored some sanity using the " + this.TargetA.Thing.def.label + ".", new TargetInfo(this.pawn.Position, this.pawn.Map), MessageTypeDefOf.NeutralEvent);// .Standard);
                        }
                    }
                    catch
                    {
                        Log.Message("Error loading Sanity Hediff.");    
                    }
                }

                JoyUtility.TryGainRecRoomThought(this.pawn);
            });*/
            yield return toil;
            yield break;
        }
    }

}
