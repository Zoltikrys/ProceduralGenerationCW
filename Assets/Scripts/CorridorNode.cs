using System;
using System.CodeDom.Compiler;
using UnityEngine;

public class CorridorNode : Node
{
    private Node struct1;
    private Node struct2;
    private int corridorWidth;

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
        throw new NotImplementedException();
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