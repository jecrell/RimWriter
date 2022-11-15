using Verse;

namespace RimWriter;

public class RoomRoleWorker_Library : RoomRoleWorker
{
    public override float GetScore(Room room)
    {
        var num = 0;
        var containedAndAdjacentThings = room.ContainedAndAdjacentThings;
        foreach (var thing in containedAndAdjacentThings)
        {
            if (thing is Building_Bookcase)
            {
                num++;
            }
        }

        return 13.5f * num;
    }
}