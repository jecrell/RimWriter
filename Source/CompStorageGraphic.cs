using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWriter
{
    public class CompStorageGraphic : ThingComp
    {
        private Graphic cachedGraphic = null;
        public CompProperties_StorageGraphic Props => this.props as CompProperties_StorageGraphic;

        public void UpdateGraphics()
        {
            cachedGraphic = null;
        }

        public Graphic CurStorageGraphic
        {
            get
            {
                if (cachedGraphic == null)
                {
                    if (parent.TryGetInnerInteractableThingOwner() is ThingOwner thingOwner &&
                        thingOwner.Count is int count)
                    {
                        if (count >= Props.countFullCapacity)
                        {
                            cachedGraphic = Props.graphicFull.GraphicColoredFor(this.parent);
                        }
                        else if (count >= Props.countSparseThreshhold)
                        {
                            cachedGraphic = Props.graphicSparse.GraphicColoredFor(this.parent);
                        }
                        else
                        {
                            cachedGraphic = Props.graphicEmpty.GraphicColoredFor(this.parent);
                        }
                    }
                }
                return cachedGraphic;
            }
        }
    }
}
