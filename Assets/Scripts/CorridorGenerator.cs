using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorGenerator
{
    // Generates corridors connecting child nodes of the given space nodes
    public List<Node> CreateCorridors(List<RoomNode> allSpaceNodes, int corridorWidth)
    {
        // List to store the generated corridors
        List<Node> corridorList = new List<Node>();

        // Queue to process RoomNodes in descending order of their tree layer index
        Queue<RoomNode> roomsToCheck = new Queue<RoomNode>(allSpaceNodes.OrderByDescending(node => node.TreeLayerIndex).ToList());

        // Process each RoomNode in the queue
        while (roomsToCheck.Count > 0)
        {
            var node = roomsToCheck.Dequeue(); // Get the next RoomNode

            // Skip leaf nodes (nodes without children)
            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }

            // Create a corridor connecting the first two child nodes
            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);

            // Add the created corridor to the list
            corridorList.Add(corridor);
        }

        return corridorList; // Return the list of corridors
    }
}
