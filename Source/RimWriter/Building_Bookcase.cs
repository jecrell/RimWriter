using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter;

public class Building_Bookcase : Building_InternalStorage
{
    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var g in base.GetGizmos())
        {
            yield return g;
        }

        if (innerContainer.Count > 0)
        {
            yield return new Command_Action
            {
                defaultLabel = "RimWriter_RetrieveBook".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport"),
                defaultDesc = "RimWriter_RetrieveBookDesc".Translate(), action = ProcessInput
            };
        }
    }

    public override string GetInspectString()
    {
        var s = new StringBuilder();
        var baseStr = base.GetInspectString();
        if (baseStr != string.Empty)
        {
            s.AppendLine(baseStr);
        }

        if (innerContainer.Count > 0)
        {
            s.AppendLine("RimWriter_ContainsXBooks".Translate(innerContainer.Count));
        }

        s.AppendLine("RimWriter_XSlotsForBooks".Translate(CompStorageGraphic.Props.countFullCapacity));
        return s.ToString().TrimEndNewlines();
    }

    private void ProcessInput()
    {
        var list = new List<FloatMenuOption>();
        if (innerContainer.Count != 0)
        {
            foreach (var thing in innerContainer)
            {
                var current = (ThingBook)thing;
                var text = current.Label;
                if (current.TryGetComp<CompArt>() != null)
                {
                    var compArt = current.TryGetComp<CompArt>();
                    text = "RimWriter_BookTitle".Translate(compArt.Title, compArt.AuthorName);
                }

                bool extraPartOnGUI(Rect rect)
                {
                    return Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), current);
                }

                list.Add(new FloatMenuOption(text, delegate { TryDrop(current); }, MenuOptionPriority.Default,
                    null, null, 29f, extraPartOnGUI));
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }
}