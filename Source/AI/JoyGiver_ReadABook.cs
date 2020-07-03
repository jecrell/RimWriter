using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace RimWriter
{
    public class JoyGiver_ReadABook : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            IEnumerable<Thing> source = pawn.Map.listerThings.AllThings.FindAll(y => y is Building_Bookcase).Where(delegate (Thing x)
            {
                Building_Bookcase building_bookcase = (Building_Bookcase)x;
                return x?.TryGetInnerInteractableThingOwner()?.Count > 0 && x.Faction == Faction.OfPlayer && !building_bookcase.IsForbidden(pawn) && 
                pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None, 1, -1, null, false) && building_bookcase.IsPoliticallyProper(pawn);
            });
            Thing t;
            if (!source.TryRandomElementByWeight(delegate (Thing x)
            {
                float lengthHorizontal = (x.Position - pawn.Position).LengthHorizontal;
                return Mathf.Max(150f - lengthHorizontal, 5f);
            }, out t))
            {
                return null;
            }
            Job tempJob = new Job(this.def.jobDef, t);
            tempJob.count = 1;
            return tempJob;
        }
    }
}
