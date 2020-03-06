﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWriter
{
    public class CompProperties_StorageGraphic : CompProperties
    {
        public GraphicData graphicEmpty = null;
        public GraphicData graphicSparse = null;
        public GraphicData graphicFull = null;

        public int countSparseThreshhold = 5;
        public int countFullCapacity = 30;

        public CompProperties_StorageGraphic()
        {
            compClass = typeof(CompStorageGraphic);
        }
    }
}
