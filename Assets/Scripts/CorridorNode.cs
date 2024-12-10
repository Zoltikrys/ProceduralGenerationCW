using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node struct1;
    private Node struct2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.struct1 = node1;
        this.struct2 = node2;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePosOfStruct2 = CheckPosStruct2AgainstStruct1();
        switch (relativePosOfStruct2)
        {
            case RelativePosition.Up:
                ProcessRoomUpOrDown(this.struct1, this.struct2);
                break;
            case RelativePosition.Down:
                ProcessRoomUpOrDown(this.struct2, this.struct1);
                break;
            case RelativePosition.Left:
                ProcessRoomLeftOrRight(this.struct2, this.struct1);
                break;
            case RelativePosition.Right:
                ProcessRoomLeftOrRight(this.struct1, this.struct2);
                break;
            default:
                break;
        }
    }


    private void ProcessRoomUpOrDown(Node struct1, Node struct2)
    {
        throw new NotImplementedException();
    }

    private void ProcessRoomLeftOrRight(Node struct2, Node struct1)
    {
        Node rightStruct = null;
        List<Node> rightStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct2);
        Node leftStruct = null;
        List<Node> leftStructChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(struct1);

        var sortedLeftStruct = leftStructChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        if (sortedLeftStruct.Count == 1)
        {
            leftStruct = sortedLeftStruct[0];
        }
        else
        {
            int maxX = sortedLeftStruct[0].TopRightAreaCorner.x;
            sortedLeftStruct = sortedLeftStruct.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStruct.Count);
            leftStruct = sortedLeftStruct[index];
        }

        var possibleNeighboursInRightStructList = rightStructChildren.Where(
            child => GetValidYForNeighbourLeftRight(
                leftStruct.TopRightAreaCorner,
                leftStruct.BottomRightAreaCorner,
                child.TopLeftAreaCorner,
                child.BottomLeftAreaCorner
                ) != -1).ToList();

        if (possibleNeighboursInRightStructList.Count <= 0)
        {
            rightStruct = struct2;
        }
        else
        {
            rightStruct = possibleNeighboursInRightStructList[0];
        }
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

    private int GetValidYForNeighbourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
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
        return -1; //returned when rooms aren't neighbours
    }

    private RelativePosition CheckPosStruct2AgainstStruct1()
    {
        Vector2 middlePointStruct1Temp = ((Vector2)struct1.TopRightAreaCorner + struct1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStruct2Temp = ((Vector2)struct1.TopRightAreaCorner + struct2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStruct1Temp, middlePointStruct2Temp);

        if ((angle < 45  && angle >= 0) || (angle > -45 && angle < 0))
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

    private float CalculateAngle(Vector2 middlePointStruct1Temp, Vector2 middlePointStruct2Temp)
    {
        return Mathf.Atan2(middlePointStruct2Temp.y - middlePointStruct1Temp.y, middlePointStruct2Temp.x - middlePointStruct1Temp.x) * Mathf.Rad2Deg;
    }
}