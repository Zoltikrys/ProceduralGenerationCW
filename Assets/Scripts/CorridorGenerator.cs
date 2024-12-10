using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorGenerator
{
    
    public List<Node> CreateCorridors(List<RoomNode> allSpaceNodes, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> roomsToCheck = new Queue<RoomNode>(allSpaceNodes.OrderByDescending(node => node.TreeLayerIndex).ToList());
        while (roomsToCheck.Count > 0)
        {
            var node = roomsToCheck.Dequeue();
            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);
            corridorList.Add(corridor);
        }
        return corridorList;


    }
}