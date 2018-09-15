using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using System.Linq;

namespace RimWriter
{
    public class WorkGiver_ReturnBooks : WorkGiver_Scanner
    {

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerThings.AllThings.FindAll(x => x is ThingBook);
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.Map.listerThings.AllThings.Any(x => x is ThingBook);
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            ThingBook book = t as ThingBook;
            if (book == null)
            {
                return null;
            }
            if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
            {
                return null;
            }
            Building_InternalStorage Building_InternalStorage = this.FindBestStorage(pawn, book);
            if (Building_InternalStorage == null)
            {
                JobFailReason.Is("NoEmptyGraveLower".Translate());
                return null;
            }
            return new Job(DefDatabase<JobDef>.GetNamed("RimWriter_ReturnBook"), t, Building_InternalStorage)
            {
                count = book.stackCount
            };
        }

        private Building_InternalStorage FindBestStorage(Pawn p, ThingBook book)
        {
            Predicate<Thing> predicate = (Thing m) => !m.IsForbidden(p) && p.CanReserveNew(m) && ((Building_InternalStorage)m).Accepts(book);
            Func<Thing, float> priorityGetter = delegate (Thing t)
            {
                float result = 0f;
                result += (float)((IStoreSettingsParent)t).GetStoreSettings().Priority;
                if (t is Building_InternalStorage bS && bS.TryGetInnerInteractableThingOwner()?.Count > 0)
                    result -= bS.TryGetInnerInteractableThingOwner().Count;
                return result;
            };
            IntVec3 position = book.Position;
            Map map = book.Map;
            List<Thing> searchSet = book.Map.listerThings.AllThings.FindAll(x => x is Building_InternalStorage);
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(p, Danger.Deadly, TraverseMode.ByPawn, false);
            Predicate<Thing> validator = predicate;
            return (Building_InternalStorage)GenClosest.ClosestThing_Global_Reachable(position, map, searchSet, peMode, traverseParams, 9999f, validator, priorityGetter);
        }
    }
}
