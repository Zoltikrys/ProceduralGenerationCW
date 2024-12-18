using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node struct1;
    private Node struct2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    // Constructor to initialize CorridorNode with two structure nodes and the desired corridor width
    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.struct1 = node1;
        this.struct2 = node2;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    // Generate the corridor based on relative position of struct2 against struct1
    private void GenerateCorridor()
    {
        var relativePosOfStruct2 = CheckPosStruct2AgainstStruct1();  // Get relative position of struct2 to struct1
        switch (relativePosOfStruct2)  // Handle corridor generation based on relative position
        {
            case RelativePosition.Up:
                ProcessRoomUpOrDown(this.struct1, this.struct2);  // Process corridor when struct2 is above struct1
                break;
            case RelativePosition.Down:
                ProcessRoomUpOrDown(this.struct2, this.struct1);  // Process corridor when struct2 is below struct1
                break;
            case RelativePosition.Right:
                ProcessRoomLeftOrRight(this.struct1, this.struct2);  // Process corridor when struct2 is to the right of struct1
                break;
            case RelativePosition.Left:
                ProcessRoomLeftOrRight(this.struct2, this.struct1);  // Process corridor when struct2 is to the left of struct1
                break;
            default:
                break;
        }
    }

    // Pprocess corridor when rooms are aligned vertically
    private void ProcessRoomUpOrDown(Node struct1, Node struct2)
    {
        Node bottomStruct = null;
        List<Node> bottomStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct1);  // Get child nodes of struct1
        Node topStruct = null;
        List<Node> topStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct2);  // Get child nodes of struct2

        var sortedBottomStruct = bottomStructChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStruct.Count == 1)
        {
            bottomStruct = bottomStructChildren[0];  // If only one child, set as bottom structure
        }
        else
        {
            int maxY = sortedBottomStruct[0].TopLeftAreaCorner.y;  // Get max Y value of bottom structure
            // Filter children within a vertical range of 10 from max Y
            sortedBottomStruct = sortedBottomStruct.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStruct.Count);  // Randomly pick one child
            bottomStruct = sortedBottomStruct[index];  // Set selected bottom structure
        }

        // Find potential neighbours in top structure that are aligned with bottom structure
        var possibleNeighboursInTopStruct = topStructChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStruct.TopLeftAreaCorner,
                bottomStruct.TopRightAreaCorner,
                child.BottomLeftAreaCorner,
                child.BottomRightAreaCorner
                ) != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();

        if (possibleNeighboursInTopStruct.Count == 0)
        {
            topStruct = struct2;  // If no valid neighbour, use struct2 as top structure
        }
        else
        {
            topStruct = possibleNeighboursInTopStruct[0];  // Pick the first valid neighbour as top structure
        }

        // Get the valid X position for the corridor between bottom and top structures
        int x = GetValidXForNeighbourUpDown(bottomStruct.TopLeftAreaCorner, bottomStruct.TopRightAreaCorner, topStruct.BottomLeftAreaCorner, topStruct.BottomRightAreaCorner);

        // Retry if the X position is invalid, and adjust bottom structure if necessary
        while (x == -1 && sortedBottomStruct.Count > 1)
        {
            sortedBottomStruct = sortedBottomStruct.Where(child => child.TopLeftAreaCorner.x != topStruct.TopLeftAreaCorner.x).ToList();
            bottomStruct = sortedBottomStruct[0];
            x = GetValidXForNeighbourUpDown(bottomStruct.TopLeftAreaCorner, bottomStruct.TopRightAreaCorner, topStruct.BottomLeftAreaCorner, topStruct.BottomRightAreaCorner);
        }
        // Set the area corner coordinates for the corridor
        BottomLeftAreaCorner = new Vector2Int(x, bottomStruct.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStruct.BottomLeftAreaCorner.y);
    }

    // Calculate a valid X position for the corridor when rooms are vertically aligned
    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        // Check various conditions to determine the valid X position between two vertically aligned rooms
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)).x;
        }
        if (bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)).x;
        }
        return -1;  // Return -1 if no valid X position found
    }

    // Process corridor when rooms are aligned horizontally (Left or Right)
    private void ProcessRoomLeftOrRight(Node struct1, Node struct2)
    {
        Node leftStruct = null;
        List<Node> leftStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct1);  // Get children of struct1
        Node rightStruct = null;
        List<Node> rightStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct2);  // Get children of struct2

        var sortedLeftStruct = leftStructChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        if (sortedLeftStruct.Count == 1)
        {
            leftStruct = sortedLeftStruct[0];  // If only one child, set as left structure
        }
        else
        {
            int maxX = sortedLeftStruct[0].TopRightAreaCorner.x;  // Get max X value of left structure
            sortedLeftStruct = sortedLeftStruct.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();  // Filter children within a horizontal range of 10 from max X
            int index = UnityEngine.Random.Range(0, sortedLeftStruct.Count);  // Randomly pick one child
            leftStruct = sortedLeftStruct[index];  // Set selected left structure
        }

        // Find potential neighbours in right structure that are aligned with left structure
        var possibleNeighboursInRightStruct = rightStructChildren.Where(
            child => GetValidYForNeighbourLeftRight(
                leftStruct.TopRightAreaCorner,
                leftStruct.BottomRightAreaCorner,
                child.TopLeftAreaCorner,
                child.BottomLeftAreaCorner
                ) != -1).OrderBy(child => child.BottomRightAreaCorner.x).ToList();

        if (possibleNeighboursInRightStruct.Count <= 0)
        {
            rightStruct = struct2;  // If no valid neighbour, use struct2 as right structure
        }
        else
        {
            rightStruct = possibleNeighboursInRightStruct[0];  // Pick the first valid neighbour as right structure
        }

        // Get the valid Y position for the corridor between left and right structures
        int y = GetValidYForNeighbourLeftRight(leftStruct.TopLeftAreaCorner, leftStruct.BottomRightAreaCorner, rightStruct.TopLeftAreaCorner, rightStruct.BottomLeftAreaCorner);

        while (y == -1 && sortedLeftStruct.Count > 1)
        {
            sortedLeftStruct = sortedLeftStruct.Where(child => child.TopLeftAreaCorner.y != leftStruct.TopLeftAreaCorner.y).ToList();
            leftStruct = sortedLeftStruct[0];
            y = GetValidYForNeighbourLeftRight(leftStruct.TopLeftAreaCorner, leftStruct.BottomRightAreaCorner, rightStruct.TopLeftAreaCorner, rightStruct.BottomLeftAreaCorner);
        }
        BottomLeftAreaCorner = new Vector2Int(leftStruct.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStruct.TopLeftAreaCorner.x, y + this.corridorWidth);
    }

    // Calculate a valid Y position for the corridor when rooms are horizontally aligned
    private int GetValidYForNeighbourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        // Determine the valid Y position between two horizontally aligned rooms
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        return -1;  // Return -1 if no valid Y position found
    }

    // Method to determine the relative position of struct2 compared to struct1
    private RelativePosition CheckPosStruct2AgainstStruct1()
    {
        // Calculate the middle point of struct1 and struct2
        Vector2 middlePointStruct1Temp = ((Vector2)struct1.TopRightAreaCorner + struct1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStruct2Temp = ((Vector2)struct2.TopRightAreaCorner + struct2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStruct1Temp, middlePointStruct2Temp);  // Calculate angle between middle points

        // Return the relative position based on the calculated angle
        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    // Method to calculate the angle between two points
    private float CalculateAngle(Vector2 middlePointStruct1Temp, Vector2 middlePointStruct2Temp)
    {
        return Mathf.Atan2(middlePointStruct2Temp.y - middlePointStruct1Temp.y, middlePointStruct2Temp.x - middlePointStruct1Temp.x) * Mathf.Rad2Deg;
    }
}
