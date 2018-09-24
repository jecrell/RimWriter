using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter
{
    public class ThingBook : ThingWithComps
    {
        CompArt compArt;
        CompArt CompArt
        {
            get
            {
                if (compArt == null)
                {
                    compArt = this.TryGetComp<CompArt>();
                }
                return compArt;
            }
        }

        public override string Label
        {
            get
            {
                if (CompArt != null)
                {
                    return "RimWriter_BookTitle".Translate(CompArt.Title, CompArt.AuthorName) + " (" + base.Label + ")";
                }
                return base.Label;
            }
        }

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
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/jecrellDestroyWriting", true)
                };   
            }
        }
    }
}
