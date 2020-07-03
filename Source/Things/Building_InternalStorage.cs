using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWriter
{
    public class Building_InternalStorage : Building, IThingHolder, IStoreSettingsParent
    {
        protected ThingOwner innerContainer;
        private StorageSettings storageSettings;
        private CompStorageGraphic compStorageGraphic = null;
        public CompStorageGraphic CompStorageGraphic
        {
            get
            {
                if (compStorageGraphic == null)
                {
                    compStorageGraphic = this.TryGetComp<CompStorageGraphic>();
                }
                return compStorageGraphic;
            }
        }
        
        public override Graphic Graphic
        {
            get
            {
                if (CompStorageGraphic?.CurStorageGraphic != null)
                {
                    return CompStorageGraphic.CurStorageGraphic;
                }
                return base.Graphic;
            }
        }


        public bool StorageTabVisible => true;
        public Building_InternalStorage()
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }
        
        public bool TryAccept(Thing thing)
        {
            return true;
        }

        public bool Accepts(Thing thing)
        {
            if (!this.storageSettings.AllowedToAccept(thing))
            {
                return false;
            }
            if (this.innerContainer.Count + 1 > this.CompStorageGraphic.Props.countFullCapacity)
            {
                return false;
            }
            return true;
        }

        public override void PostMake()
        {
            base.PostMake();
            this.storageSettings = new StorageSettings(this);
            if (this.def.building.defaultStorageSettings != null)
            {
                this.storageSettings.CopyFrom(this.def.building.defaultStorageSettings);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                            this
            });
            Scribe_Deep.Look<StorageSettings>(ref this.storageSettings, "storageSettings", new object[]
            {
                this
            });
        }



        public bool TryDropRandom(out Thing droppedThing, bool forbid = false)
        {
            Thing outThing;
            droppedThing = null;
            if (this.innerContainer.Count > 0)
            {
                this.innerContainer.TryDrop(this.innerContainer.RandomElement(), ThingPlaceMode.Near, out outThing);
                if (forbid) outThing.SetForbidden(true);
                droppedThing = outThing as ThingBook;
                return true;
            }
            else
            {
                Log.Warning("Building_InternalStorage : TryDropRandom - failed to get a book.");
            }
            return false;
        }

        public bool TryDrop(Thing item, bool forbid = true)
        {
            if (this.innerContainer.Contains(item))
            {
                Thing outThing;
                this.innerContainer.TryDrop(item, ThingPlaceMode.Near, out outThing);
                if (forbid) outThing.SetForbidden(true);
                return true;
            }
            return false;
        }


        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return this.def.building.fixedStorageSettings;
        }

        public StorageSettings GetStoreSettings()
        {
            return this.storageSettings;
        }
    }
}
