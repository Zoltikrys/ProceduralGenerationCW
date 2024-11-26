using System;
using System.Collections.Generic;
using UnityEngine;
public class DungeonGenerator
{
    RoomNode rootNode;
    List<RoomNode> allSpaceNodes = new List<RoomNode>();
    private int dungeonWidth;
    private int dungeonLength;

    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    internal object CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        throw new NotImplementedException();
    }
}