using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth;
    public int dungeonLength;
    
    public int roomWidthMin;
    public int roomLengthMin;

    public int maxIterations;
    public int corridorWidth;


    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
