using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefab; //this is the object to respawn
    public int spawnCount = 10; // will spawn this many times

    public DungeonCreator dungeonCreator;

    // Start is called before the first frame update
    void Start()
    {

        dungeonCreator = GetComponent<DungeonCreator>();

        for (int i = 0; i < spawnCount; i++)
        {
            // Generate random positions within a certain range to spawn the game object in
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(0,dungeonCreator.dungeonWidth), 0.2f, UnityEngine.Random.Range(0,dungeonCreator.dungeonLength)); //spawn coins within the confines of the dungeon
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Vector3 randomPosition = new Vector3(dungeonCreator.dungeonWidth, 0.2f, dungeonCreator.dungeonLength); //spawn coins within the confines of the dungeon
        //Instantiate(prefab, randomPosition, Quaternion.identity);
    }
}
