using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StructureHelper
{
    // Traverses the graph starting from the parentNode and extracts the leaf nodes (nodes with no children).
    public static List<Node> TraverseGraphToExtractLowestLeaves(Node parentNode)
    {
        // A queue for BFS traversal of nodes
        Queue<Node> nodesToCheck = new Queue<Node>();
        // A list to store the leaf nodes
        List<Node> listToReturn = new List<Node>();

        // If the parent node has no children, return it as the only leaf node
        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode };
        }

        // Add all children of the parent node to the queue
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }

        // Perform breadth first search to find all leaf nodes
        while (nodesToCheck.Count > 0)
        {
            // Dequeue the next node to check
            var currentNode = nodesToCheck.Dequeue();

            // If the current node has no children, it's a leaf node, so add it to the list
            if (currentNode.ChildrenNodeList.Count == 0)
            {
                listToReturn.Add(currentNode);
            }
            else
            {
                // Otherwise, enqueue all of its children to continue the traversal
                foreach (var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue((Node)child);
                }
            }
        }
        return listToReturn;
    }

    // Generates a random point inside the region defined by boundaryLeftPoint and boundaryRightPoint
    // for the bottom-left corner with the specified point modifier and offset.
    public static Vector2Int GenerateBottomLeftCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        // Calculate the min and max ranges for both x and y with the provided offset
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        // Generate a random point within the specified range, factoring in the pointModifier
        return new Vector2Int(Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (maxY - minY) * pointModifier)));
    }

    // Generates a random point inside the region defined by boundaryLeftPoint and boundaryRightPoint
    // for the top-right corner with the specified point modifier and offset.
    public static Vector2Int GenerateTopRightCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        // Calculate the min and max ranges for both x and y with the provided offset
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        // Generate a random point within the specified range, factoring in the pointModifier
        return new Vector2Int(Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY));
    }

    // Calculates the middle point between two points.
    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        // Add the two vectors and divide by 2 to get the middle point
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;

        // Return the middle point as an integer Vector2Int
        return new Vector2Int((int)tempVector.x, (int)(tempVector.y));
    }
}
public enum RelativePosition
{
    Up, Down, Left, Right
}
