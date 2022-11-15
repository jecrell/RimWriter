using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter;

public class Item_JournalPage : ThingWithComps
{
    private CompArt artComp;

    private bool IsBook;

    private Pawn owner;

    private bool saveOwner;

    public void ClaimJournal(Pawn claimant)
    {
        if (claimant != null && owner == null)
        {
            owner = claimant;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        // Save and load the work variables, so they don't default after loading
        Scribe_Values.Look(ref IsBook, "IsBook");
        Scribe_Values.Look(ref saveOwner, "saveOwner");
        if (saveOwner)
        {
            Scribe_References.Look(ref owner, "owner");
        }
    }

    [DebuggerHidden]
    public override IEnumerable<Gizmo> GetGizmos()
    {
        using var enumerator = base.GetGizmos().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            yield return current;
        }

        yield return new Command_Action
        {
            defaultLabel = "Discard",
            icon = ContentFinder<Texture2D>.Get("UI/Commands/Detonate"),
            defaultDesc = "Disposes of unwanted journal pages.",
            action = delegate { DeSpawn(); },
            hotKey = KeyBindingDefOf.Misc3
        };
    }

    public void ResolveOwner()
    {
        if (artComp == null)
        {
            return;
        }

        foreach (var colonist in Map.mapPawns.FreeColonistsSpawned)
        {
            if (colonist.Name.ToStringFull != artComp.AuthorName)
            {
                continue;
            }

            owner = colonist;
            saveOwner = true;
            break;
        }
    }

    public override void SpawnSetup(Map map, bool bla)
    {
        base.SpawnSetup(map, bla);
        artComp = GetComp<CompArt>();
        ResolveOwner();
    }
}