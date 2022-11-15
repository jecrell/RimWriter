using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWriter;

public class JobDriver_ReadABook : JobDriver
{
    protected const TargetIndex TargetThingIndex = TargetIndex.A;

    public static Toil CarryBookToReadSpot(Pawn pawn, TargetIndex ingestibleInd)
    {
        var toil = new Toil();
        toil.initAction = delegate
        {
            var actor = toil.actor;
            var intVec = IntVec3.Invalid;
            var unused = actor.CurJob.GetTarget(ingestibleInd).Thing;

            bool baseChairValidator(Thing t)
            {
                if (t.def.building is not { isSittable: true })
                {
                    return false;
                }

                if (t.IsForbidden(pawn))
                {
                    return false;
                }

                if (!actor.CanReserve(t))
                {
                    return false;
                }

                if (!t.IsSociallyProper(actor))
                {
                    return false;
                }

                if (t.IsBurning())
                {
                    return false;
                }

                if (t.HostileTo(pawn))
                {
                    return false;
                }

                var result = false;
                for (var i = 0; i < 4; i++)
                {
                    var c = t.Position + GenAdj.CardinalDirections[i];
                    var edifice = c.GetEdifice(t.Map);
                    if (edifice == null || edifice.def.surfaceType != SurfaceType.Eat)
                    {
                        continue;
                    }

                    result = true;
                    break;
                }

                return result;
            }

            var thing = GenClosest.ClosestThingReachable(actor.Position, actor.Map,
                ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell,
                TraverseParms.For(actor), 25f,
                t => baseChairValidator(t) && t.Position.GetDangerFor(pawn, t.Map) == Danger.None);
            if (thing == null)
            {
                intVec = RCellFinder.SpotToChewStandingNear(actor, actor.CurJob.GetTarget(ingestibleInd).Thing);
                var chewSpotDanger = intVec.GetDangerFor(pawn, actor.Map);
                if (chewSpotDanger != Danger.None)
                {
                    thing = GenClosest.ClosestThingReachable(actor.Position, actor.Map,
                        ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell,
                        TraverseParms.For(actor), 25f,
                        t => baseChairValidator(t) && t.Position.GetDangerFor(pawn, t.Map) <= chewSpotDanger);
                }
            }

            if (thing != null)
            {
                intVec = thing.Position;
                actor.Reserve(thing, actor.CurJob);
            }

            actor.Map.pawnDestinationReservationManager.Reserve(actor, actor.CurJob, intVec);
            actor.pather.StartPath(intVec, PathEndMode.OnCell);
        };
        toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
        return toil;
    }

    public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
    {
        if (pawn.Rotation == Rot4.East)
        {
            drawPos += new Vector3(0.6f, 0, 0);
            flip = true;
        }
        else
        {
            drawPos -= new Vector3(0.6f, 0, 0);
            flip = false;
        }

        behind = pawn.Rotation == Rot4.North;

        return true;
    }

    public override bool TryMakePreToilReservations(bool v)
    {
        return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(100);
        yield return new Toil
        {
            initAction = delegate
            {
                switch (TargetA.Thing)
                {
                    case Building_Bookcase bld when bld.TryGetInnerInteractableThingOwner()?.Count > 0:
                    {
                        if (bld.TryDropRandom(out var book))
                        {
                            job.SetTarget(TargetIndex.B, book as ThingBook);
                        }

                        break;
                    }
                    case ThingBook tBook:
                        job.SetTarget(TargetIndex.B, tBook);
                        break;
                }
            }
        };
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Ingest.PickupIngestible(TargetIndex.B, pawn);
        yield return CarryBookToReadSpot(pawn, TargetIndex.B);
        yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.C, TargetIndex.B);
        var wait = Toils_General.Wait(job.def.joyDuration);
        wait.FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);
        wait.tickAction = ReadTickAction;
        yield return wait;
        var libraryThoughtToil = new Toil { initAction = delegate { RimWriterUtility.TryGainLibraryThought(pawn); } };
        yield return libraryThoughtToil;
        yield return Toils_Reserve.Release(TargetIndex.B);
    }

    protected void ReadTickAction()
    {
        pawn.rotationTracker.FaceCell(TargetB.Cell);
        pawn.GainComfortFromCellIfPossible();
        var statValue = TargetThingA.GetStatValue(StatDefOf.JoyGainFactor);
        var pawn1 = pawn;
        if (TargetThingA is GuideBook gBook)
        {
            gBook.Teach(pawn1);
        }

        JoyUtility.JoyTickCheckEnd(pawn1, JoyTickFullJoyAction.GoToNextToil, statValue);
    }
}