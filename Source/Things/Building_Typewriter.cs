// ----------------------------------------------------------------------
// These are basic usings. Always let them be here.
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// ----------------------------------------------------------------------
// These are RimWorld-specific usings. Activate/Deactivate what you need:
// ----------------------------------------------------------------------
using UnityEngine;         // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.AI;          // Needed when you do something with the AI
using Verse.Sound;       // Needed when you do something with Sound
using Verse.Noise;       // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace RimWriter
{
    /// <summary>
    /// This is the main class for the Gramophone.
    /// Major coding credits go to mrofa and Haplo.
    /// I am but an amateur working on the shoulders of
    /// giants.
    /// </summary>
    /// <author>Jecrell</author>
    /// <permission>Free to use by all.</permission>
    public class Building_Typewriter : Building_WorkTable
    {
        // ===================== Variables =====================

        // Work variable
        //private int counter = 0;                  // 60Ticks = 1s // 20000Ticks = 1 Day
        
        // Destroyed flag. Most of the time not really needed, but sometimes...
        private bool destroyedFlag = false;
        private float sanityRestoreRate = 0.000025f;

        // ===================== Destroy =====================

        /// <summary>
        /// Clean up when this is destroyed
        /// </summary>
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            // block further ticker work
            destroyedFlag = true;

            base.Destroy(mode);
        }

        #region Ticker
        // ===================== Ticker =====================

        /// <summary>
        /// This is used, when the Ticker in the XML is set to 'Rare'
        /// This is a tick thats done once every 250 normal Ticks
        /// </summary>
        public override void TickRare()
        {
            if (destroyedFlag) // Do nothing further, when destroyed (just a safety)
                return;

            // Don't forget the base work
            base.TickRare();

            // Call work function
            DoTickerWork(250);
        }


        /// <summary>
        /// This is used, when the Ticker in the XML is set to 'Normal'
        /// This Tick is done often (60 times per second)
        /// </summary>
        public override void Tick()
        {
            if (destroyedFlag) // Do nothing further, when destroyed (just a safety)
                return;

            base.Tick();

            // Call work function
            DoTickerWork(1);
        }

        // ===================== Main Work Function =====================

        /// <summary>
        /// This will be called from one of the Ticker-Functions.
        /// </summary>
        /// <param name="tickerAmount"></param>
        private void DoTickerWork(int tickerAmount)
        {
            
        }

        #endregion Ticker

        // ===================== Inspections =====================

        /// <summary>
        /// This string will be shown when the object is selected (focus)
        /// </summary>
        /// <returns></returns>
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Add the inspections string from the base
            stringBuilder.Append(base.GetInspectString());

            // return the complete string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Debug button play, for now.
        /// </summary>
        /// <returns></returns>
		[DebuggerHidden]
        public IEnumerable<Command> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Set fuel to 0.1",
                    action = delegate
                    {
                        //this.<> f__this.fuel = 0.1f;
                    }
                };
            }
            yield break;
        }

        public override void UsedThisTick()
        {
            base.UsedThisTick();
            if (Spawned && InteractionCell.IsValid && InteractionCell.GetFirstPawn(this.MapHeld) is Pawn pawn)
            {
                if (RimWriterUtility.IsCosmicHorrorsLoaded() || RimWriterUtility.IsCultsLoaded())
                {
                    try
                    {
                        if (RimWriterUtility.HasSanityLoss(pawn))
                        {
                            RimWriterUtility.ReduceSanityLoss(pawn, sanityRestoreRate);
                            if (Find.TickManager.TicksGame % 1500 == 0)
                            {
                                Messages.Message(
                                    pawn.ToString() + " has restored some sanity using the " +
                                    this.def.label + ".", new TargetInfo(pawn.Position, pawn.Map),
                                    MessageTypeDefOf.NeutralEvent); // .Standard);
                            }
                        }
                    }
                    catch
                    {
                        //Log.Message("Error loading Sanity Hediff.");
                    }
                }
            }
        }

        /// <summary>
        /// All the menu options for the Gramophone.
        /// </summary>
        /// <param name="myPawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            base.GetFloatMenuOptions(myPawn);

            Thing building = null;
            bool flag = false;
            for (int i = 0; i < 4; i++)
            {
                IntVec3 c = this.Position + GenAdj.CardinalDirections[i];
                if (!c.IsForbidden(myPawn))
                {
                    Building edifice = c.GetEdifice(Map);
                    if (edifice != null && edifice.def.building.isSittable && myPawn.CanReserve(edifice, 1))
                    {
                        building = edifice;
                        break;
                    }
                }
            }
            if (building == null)
            {
                flag = false;
            }
            flag = true;
            
            if (!myPawn.CanReserve(this, 1))
            {
                FloatMenuOption item = new FloatMenuOption("CannotUseReserved".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item
                };
            }
            if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some, false, TraverseMode.ByPawn))
            {
                FloatMenuOption item2 = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (building == null)
            {
                FloatMenuOption item2 = new FloatMenuOption("Seat required", null, MenuOptionPriority.Default, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                FloatMenuOption item4 = new FloatMenuOption(TranslatorFormattedStringExtensions.Translate("CannotUseReason",
                    TranslatorFormattedStringExtensions.Translate("IncapableOfCapacity", PawnCapacityDefOf.Manipulation.label)), null, MenuOptionPriority.Default, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item4
                };
            }
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            if (flag)
            {
                Action action0 = delegate
                {
                    Job job = new Job(DefDatabase<JobDef>.GetNamed("RimWriter_FreeWrite"), this, building);
                    if (job != null)
                    {
                        if (myPawn.jobs.TryTakeOrderedJob(job))
                        {
                            //Lala
                        }
                    }
                };
                list.Add(new FloatMenuOption("Practice writing", action0, MenuOptionPriority.Default, null, null, 0f, null));
  
            }
            return list;

        }

    }
}
