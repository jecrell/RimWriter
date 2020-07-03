using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace RimWriter
{
    // Token: 0x02000061 RID: 97
    public class JobDriver_ReturnBook: JobDriver
    {
        // Token: 0x060002A5 RID: 677 RVA: 0x00018AF3 File Offset: 0x00016EF3
        public JobDriver_ReturnBook()
        {
            this.rotateToFace = TargetIndex.B;
        }

        // Token: 0x1700008D RID: 141
        // (get) Token: 0x060002A6 RID: 678 RVA: 0x00018B04 File Offset: 0x00016F04
        private ThingBook Book
        {
            get
            {
                return (ThingBook)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x1700008E RID: 142
        // (get) Token: 0x060002A7 RID: 679 RVA: 0x00018B2C File Offset: 0x00016F2C
        private Building_InternalStorage Storage
        {
            get
            {
                return (Building_InternalStorage)this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        // Token: 0x060002A8 RID: 680 RVA: 0x00018B54 File Offset: 0x00016F54
        public override bool TryMakePreToilReservations(bool b)
        {
            return this.pawn.Reserve(this.Book, this.job, 1, -1, null) && this.pawn.Reserve(this.Book, this.job, 1, -1, null);
        }

        // Token: 0x060002A9 RID: 681 RVA: 0x00018BA8 File Offset: 0x00016FA8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
            this.FailOn(() => !this.Storage.Accepts(this.Book));
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
            yield return Toils_Haul.CarryHauledThingToContainer();
            Toil prepare = Toils_General.Wait(250);
            prepare.WithProgressBarToilDelay(TargetIndex.B, false, -0.5f);
            yield return prepare;
            yield return new Toil
            {
                initAction = delegate
                {
                    if (this.pawn.carryTracker.CarriedThing == null)
					{
                        Log.Error(this.pawn + " tried to place hauled corpse in grave but is not hauling anything.");
                        return;
                    }
                    if (this.Storage.Accepts(this.Book))
					{
                        bool flag = false;
                        if (Book.holdingOwner != null)
                        {
                            Book.holdingOwner.TryTransferToContainer(Book, this.Storage.TryGetInnerInteractableThingOwner(), Book.stackCount, true);
                            flag = true;
                        }
                        else
                        {
                            flag = this.Storage.TryGetInnerInteractableThingOwner().TryAdd(Book, true);
                        }
                        Storage.CompStorageGraphic.UpdateGraphics();
                        this.pawn.carryTracker.innerContainer.Remove(this.Book);
                        this.pawn.records.Increment(RecordDefOf.CorpsesBuried);
                    }
                }
            };
            yield break;
        }

        // Token: 0x040001F8 RID: 504
        private const TargetIndex CorpseIndex = TargetIndex.A;

        // Token: 0x040001F9 RID: 505
        private const TargetIndex GraveIndex = TargetIndex.B;
    }
}
