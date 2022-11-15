using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWriter;

public class JoyGiver_ReadABook : JoyGiver
{
    public override Job TryGiveJob(Pawn pawn)
    {
        var source = pawn.Map.listerThings.AllThings.FindAll(y => y is Building_Bookcase).Where(
            delegate(Thing x)
            {
                var building_bookcase = (Building_Bookcase)x;
                return x.TryGetInnerInteractableThingOwner()?.Count > 0 && x.Faction == Faction.OfPlayer &&
                       !building_bookcase.IsForbidden(pawn) &&
                       pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None) &&
                       building_bookcase.IsPoliticallyProper(pawn);
            });
        if (!source.TryRandomElementByWeight(
                delegate(Thing x)
                {
                    var lengthHorizontal = (x.Position - pawn.Position).LengthHorizontal;
                    return Mathf.Max(150f - lengthHorizontal, 5f);
                },
                out var t))
        {
            return null;
        }

        var tempJob = new Job(def.jobDef, t) { count = 1 };
        return tempJob;
    }
}