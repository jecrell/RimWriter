using System;
using System.Collections.Generic;
using Verse.AI;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimWriter
{
    public class JobDriver_ReadABook : JobDriver
    {
        public override bool TryMakePreToilReservations(bool v)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null);
        }

        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
        {
            if (this.pawn.Rotation == Rot4.East)
            {
                drawPos += new Vector3(0.6f, 0, 0);
                flip = true;
            }
            else
            {
                drawPos -= new Vector3(0.6f, 0, 0);
                flip = false;
            }
            if (this.pawn.Rotation == Rot4.North)
            {
                behind = true;
            }
            else
            {
                behind = false;
            }
            return true;
        }

        protected void ReadTickAction()
        {
            this.pawn.rotationTracker.FaceCell(base.TargetB.Cell);
            this.pawn.GainComfortFromCellIfPossible();
            float statValue = base.TargetThingA.GetStatValue(StatDefOf.JoyGainFactor, true);
            Pawn pawn = this.pawn;
            float extraJoyGainFactor = statValue;
            if (TargetThingA is GuideBook gBook)
            {
                gBook.Teach(pawn);
            }
            JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.GoToNextToil, extraJoyGainFactor);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(100);
            yield return new Toil()
            {
                initAction = delegate
                {
                    if (TargetA.Thing is Building_Bookcase bld && bld?.TryGetInnerInteractableThingOwner()?.Count > 0)
                    {
                        Thing book = null;
                        if (bld.TryDropRandom(out book))
                        {
                            this.job.SetTarget(TargetIndex.B, book as ThingBook);
                        }
                    }
                }
            };
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Ingest.PickupIngestible(TargetIndex.B, this.pawn);
            yield return JobDriver_ReadABook.CarryBookToReadSpot(this.pawn, TargetIndex.B);
            yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.C, TargetIndex.B);
            Toil wait = Toils_General.Wait(this.job.def.joyDuration);
            wait.FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);
            wait.tickAction = this.ReadTickAction;
            yield return wait;
            Toil libraryThoughtToil = new Toil();
            libraryThoughtToil.initAction = delegate { RimWriterUtility.TryGainLibraryThought(pawn); };
            yield return libraryThoughtToil;
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield break;
        }




        // Token: 0x060003BE RID: 958 RVA: 0x00025D2C File Offset: 0x0002412C
        public static Toil CarryBookToReadSpot(Pawn pawn, TargetIndex ingestibleInd)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                IntVec3 intVec = IntVec3.Invalid;
                Thing thing = null;
                Thing thing2 = actor.CurJob.GetTarget(ingestibleInd).Thing;
                Predicate<Thing> baseChairValidator = delegate (Thing t)
                {
                    if (t.def.building == null || !t.def.building.isSittable)
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
                    {
                        return false;
                    }
                    if (!actor.CanReserve(t, 1, -1, null, false))
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
                    bool result = false;
                    for (int i = 0; i < 4; i++)
                    {
                        IntVec3 c = t.Position + GenAdj.CardinalDirections[i];
                        Building edifice = c.GetEdifice(t.Map);
                        if (edifice != null && edifice.def.surfaceType == SurfaceType.Eat)
                        {
                            result = true;
                            break;
                        }
                    }
                    return result;
                };
                thing = GenClosest.ClosestThingReachable(actor.Position, actor.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), 
                    PathEndMode.OnCell, TraverseParms.For(actor, Danger.Deadly, TraverseMode.ByPawn, false), 25f, (Thing t) => baseChairValidator(t) &&
                    t.Position.GetDangerFor(pawn, t.Map) == Danger.None, null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing == null)
                {
                    intVec = RCellFinder.SpotToChewStandingNear(actor, actor.CurJob.GetTarget(ingestibleInd).Thing);
                    Danger chewSpotDanger = intVec.GetDangerFor(pawn, actor.Map);
                    if (chewSpotDanger != Danger.None)
                    {
                        thing = GenClosest.ClosestThingReachable(actor.Position, actor.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                            PathEndMode.OnCell, TraverseParms.For(actor, Danger.Deadly, TraverseMode.ByPawn, false), 25f, (Thing t) => baseChairValidator(t) &&
                            t.Position.GetDangerFor(pawn, t.Map) <= chewSpotDanger, null, 0, -1, false, RegionType.Set_Passable, false);
                    }
                }
                if (thing != null)
                {
                    intVec = thing.Position;
                    actor.Reserve(thing, actor.CurJob, 1, -1, null);
                }
                actor.Map.pawnDestinationReservationManager.Reserve(actor, actor.CurJob, intVec);
                actor.pather.StartPath(intVec, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        protected const TargetIndex TargetThingIndex = TargetIndex.A;
        

    }


}
