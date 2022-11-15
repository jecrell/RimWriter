using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimWriter;

public class Building_InternalStorage : Building, IThingHolder, IStoreSettingsParent
{
    private CompStorageGraphic compStorageGraphic;
    protected ThingOwner innerContainer;

    private StorageSettings storageSettings;

    public Building_InternalStorage()
    {
        innerContainer = new ThingOwner<Thing>(this, false);
    }

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

    public void Notify_SettingsChanged()
    {
    }

    public bool StorageTabVisible => true;

    public StorageSettings GetParentStoreSettings()
    {
        return def.building.fixedStorageSettings;
    }

    public StorageSettings GetStoreSettings()
    {
        return storageSettings;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public bool Accepts(Thing thing)
    {
        if (!storageSettings.AllowedToAccept(thing))
        {
            return false;
        }

        return innerContainer.Count + 1 <= CompStorageGraphic.Props.countFullCapacity;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
    }

    public override void PostMake()
    {
        base.PostMake();
        storageSettings = new StorageSettings(this);
        if (def.building.defaultStorageSettings != null)
        {
            storageSettings.CopyFrom(def.building.defaultStorageSettings);
        }
    }

    public bool TryAccept(Thing thing)
    {
        return true;
    }

    public void TryDrop(Thing item, bool forbid = true)
    {
        if (!innerContainer.Contains(item))
        {
            return;
        }

        innerContainer.TryDrop(item, ThingPlaceMode.Near, out var outThing);
        if (forbid)
        {
            outThing.SetForbidden(true);
        }
    }

    public bool TryDropRandom(out Thing droppedThing, bool forbid = false)
    {
        droppedThing = null;
        if (innerContainer.Count > 0)
        {
            innerContainer.TryDrop(innerContainer.RandomElement(), ThingPlaceMode.Near, out var outThing);
            if (forbid)
            {
                outThing.SetForbidden(true);
            }

            droppedThing = outThing as ThingBook;
            return true;
        }

        Log.Warning("Building_InternalStorage : TryDropRandom - failed to get a book.");
        return false;
    }
}