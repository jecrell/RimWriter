using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimWriter;

public class WorkGiver_ReturnBooks : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var book = t as ThingBook;
        if (!MeetsExceptionList(t) && book == null)
        {
            return null;
        }

        if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
        {
            return null;
        }

        var Building_InternalStorage = FindBestStorage(pawn, t);
        if (Building_InternalStorage != null)
        {
            if (book != null)
            {
                return new Job(DefDatabase<JobDef>.GetNamed("RimWriter_ReturnBook"), t, Building_InternalStorage)
                    { count = book.stackCount };
            }
        }

        JobFailReason.Is("RimWriter_NoInternalStorage".Translate());
        return null;
    }

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public bool MeetsExceptionList(Thing t)
    {
        return t?.def?.defName == "Cults_Grimoire" || t?.def?.defName == "Cults_TheKingInYellow";
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerThings.AllThings.FindAll(x => x is ThingBook);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !pawn.Map.listerThings.AllThings.Any(x => x is ThingBook);
    }

    private Building_InternalStorage FindBestStorage(Pawn p, Thing book)
    {
        bool predicate(Thing m)
        {
            return !m.IsForbidden(p) && p.CanReserveNew(m) && ((Building_InternalStorage)m).Accepts(book);
        }

        float priorityGetter(Thing t)
        {
            var result = 0f;
            result += (float)((IStoreSettingsParent)t).GetStoreSettings().Priority;
            if (t is Building_InternalStorage bS && bS.TryGetInnerInteractableThingOwner()?.Count > 0)
            {
                result -= bS.TryGetInnerInteractableThingOwner().Count;
            }

            return result;
        }

        var position = book.Position;
        var map = book.Map;
        var searchSet = book.Map.listerThings.AllThings.FindAll(x => x is Building_InternalStorage);
        var peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(p);
        Predicate<Thing> validator = predicate;
        return (Building_InternalStorage)GenClosest.ClosestThing_Global_Reachable(position, map, searchSet, peMode,
            traverseParams, 9999f, validator, priorityGetter);
    }
}