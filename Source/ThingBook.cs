using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimWriter
{
    public class ThingBook : ThingWithComps
    {
        CompArt compArt = null;
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
                    return "RimWriter_BookTitle".Translate(new object[] { CompArt.Title, CompArt.AuthorName }) + " (" + base.Label + ")";
                }
                return base.Label;
            }
        }
    }
}
