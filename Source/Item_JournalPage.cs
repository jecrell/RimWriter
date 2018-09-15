using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWriter
{
    public class Item_JournalPage : ThingWithComps
    {
        private bool IsBook = false;
        private bool saveOwner = false;
        private Pawn owner = null;
        private CompArt artComp;

        public override void SpawnSetup(Map map, bool bla)
        {
            base.SpawnSetup(map, bla);
            this.artComp = base.GetComp<CompArt>();
            ResolveOwner();
        }

        public void ResolveOwner()
        {
            if (artComp != null)
            {
                foreach (Pawn colonist in this.Map.mapPawns.FreeColonistsSpawned)
                {
                    if (colonist.Name.ToStringFull == artComp.AuthorName )
                    {
                        owner = colonist;
                        saveOwner = true;
                        break;
                    }
                }
            }
        }
        
        public void ClaimJournal(Pawn claimant)
        {
            if (claimant != null && this != null && owner == null)
            {
                owner = claimant;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // Save and load the work variables, so they don't default after loading

            Scribe_Values.Look<bool>(ref this.IsBook, "IsBook", false);
            Scribe_Values.Look<bool>(ref this.saveOwner, "saveOwner", false, false);
            if (this.saveOwner)
            {
                Scribe_References.Look<Pawn>(ref this.owner, "owner", false);
            }
        }


        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Gizmo current = enumerator.Current;
                yield return current;
            }
            
                yield return new Command_Action
                {
                    defaultLabel = "Discard",
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/Detonate", true),
                    defaultDesc = "Disposes of unwanted journal pages.",
                    action = delegate
                    {
                        this.DeSpawn();
                    },
                    hotKey = KeyBindingDefOf.Misc3
                };

            yield break;
        }

    }
}
