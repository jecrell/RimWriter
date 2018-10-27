using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWriter
{
    public class Page : ThingWithComps
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            if (!Destroyed)
            {
                yield return new Command_Action
                {
                    action = () => Destroy(DestroyMode.KillFinalize),
                    defaultLabel = "RimWriter_Destroy".Translate(),
                    defaultDesc = "RimWriter_DestroyDesc".Translate(Label),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/jecrellDestroyWriting")
                };
            }
        }
    }
}