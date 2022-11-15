using Verse;

namespace RimWriter;

public class CompProperties_StorageGraphic : CompProperties
{
    public int countFullCapacity = 30;

    public int countSparseThreshhold = 5;

    public GraphicData graphicEmpty = null;

    public GraphicData graphicFull = null;

    public GraphicData graphicSparse = null;

    public CompProperties_StorageGraphic()
    {
        compClass = typeof(CompStorageGraphic);
    }
}