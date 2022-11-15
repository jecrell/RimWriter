using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWriter;

public class ThingBook : ThingWithComps
{
    private CompArt compArt;

    public override string Label
    {
        get
        {
            if (CompArt != null)
            {
                return "RimWriter_BookTitle".Translate(CompArt.Title, CompArt.AuthorName) + " (" + base.Label + ")";
            }

            return base.Label;
        }
    }

    private CompArt CompArt
    {
        get
        {
            if (compArt == null)
            {
                compArt = this.TryGetComp<CompArt>();
            }

            return compArt;
        }
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
    {
        foreach (var op in base.GetFloatMenuOptions(selPawn))
        {
            yield return op;
        }

        if (!selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) ||
            !selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            yield return new FloatMenuOption(
                "RimWriter_CannotRead".Translate(Label) + " (" + "Incapable".Translate() + ")", null);
        }
        else
        {
            yield return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(
                    "RimWriter_Read".Translate(Label),
                    delegate
                    {
                        var job = new Job(DefDatabase<JobDef>.GetNamedSilentFail("RimWriter_ReadABook"), this)
                            { count = 1 };
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
                    },
                    MenuOptionPriority.Low),
                selPawn,
                this);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (!Destroyed)
        {
            yield return new Command_Action
            {
                action = () => Destroy(DestroyMode.KillFinalize), defaultLabel = "RimWriter_Destroy".Translate(),
                defaultDesc = "RimWriter_DestroyDesc".Translate(Label),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/jecrellDestroyWriting")
            };
        }
    }
}