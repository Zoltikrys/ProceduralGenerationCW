using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioner
{
    // Root node of the binary space partitioning tree
    RoomNode rootNode;
    public RoomNode RootNode { get => rootNode; }

    // Constructor to initialize the binary space partitioner with the dungeon's dimensions
    public BinarySpacePartitioner(int dungeonWidth, int dungeonLength)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonLength), null, 0);
    }

    // Prepares the collection of RoomNodes by dividing the space iteratively
    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        // Queue to manage RoomNodes for processing
        Queue<RoomNode> graph = new Queue<RoomNode>();
        // List to store all RoomNodes
        List<RoomNode> listToReturn = new List<RoomNode>();

        graph.Enqueue(this.rootNode); // Add the root node to the queue
        listToReturn.Add(this.rootNode); // Add the root node to the result list

        int iterations = 0;

        // Iterate until maxIterations or until the queue is empty
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue(); // Get the next RoomNode from the queue

            // Check if the current RoomNode can be split
            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
            {
                // Split the space and update the collections
                SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }
        return listToReturn;
    }

    // Splits the given RoomNode into two smaller RoomNodes
    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomLengthMin, int roomWidthMin, Queue<RoomNode> graph)
    {
        // Determine the dividing line for splitting the space
        Line line = GetLineDividingSpace(currentNode.BottomLeftAreaCorner, currentNode.TopRightAreaCorner, roomWidthMin, roomLengthMin);

        RoomNode node1;
        RoomNode node2;

        // Split horizontally or vertically based on the line orientation
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y), currentNode, currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y), currentNode.TopRightAreaCorner, currentNode, currentNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y), currentNode, currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y), currentNode.TopRightAreaCorner, currentNode, currentNode.TreeLayerIndex + 1);
        }

        // Add the newly created nodes to the collections
        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);
    }

    // Adds a new RoomNode to both the list and the queue for further processing
    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    // Determines the dividing line based on the room dimensions and minimum sizes
    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;

        // Check if the area is large enough to split horizontally or vertically
        bool lengthStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= 2 * roomLengthMin;
        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= 2 * roomWidthMin;

        if (lengthStatus && widthStatus)
        {
            // Randomly decide the orientation if both options are valid
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            // Can only make corridor vertically
            orientation = Orientation.Vertical;
        }
        else
        {
            // Can only make corridor horizontally
            orientation = Orientation.Horizontal;
        }

        // Return the line object with the determined orientation and coordinates
        return new Line(orientation, GetCoordinatesForOrientation(orientation, bottomLeftAreaCorner, topRightAreaCorner, roomWidthMin, roomLengthMin));
    }

    // Calculates the coordinates of the dividing line based on its orientation
    private Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            // Determine the y-coordinate for a horizontal line
            coordinates = new Vector2Int(0, Random.Range(bottomLeftAreaCorner.y + roomLengthMin, topRightAreaCorner.y - roomLengthMin));
        }
        else
        {
            // Determine the x-coordinate for a vertical line
            coordinates = new Vector2Int(Random.Range(bottomLeftAreaCorner.x + roomWidthMin, topRightAreaCorner.x - roomWidthMin), 0);
        }
        return coordinates;
    }
}
