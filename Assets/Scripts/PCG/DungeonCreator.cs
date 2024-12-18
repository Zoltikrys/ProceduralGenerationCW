using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    // Dungeon properties
    public int dungeonWidth;
    public int dungeonLength;

    public int roomWidthMin;
    public int roomLengthMin;

    public int maxIterations;
    public int corridorWidth;

    public Material material; // Material used for the dungeon meshes

    // Modifiers for room size adjustments
    [Range(0f, 0.3f)]
    public float roomBottomCornerModifier;

    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;

    [Range(0, 2)]
    public int roomOffset;

    // Prefabs for wall creation
    public GameObject wallVert;
    public GameObject WallHorz;

    // Lists to track possible positions for doors and walls
    List<Vector3Int> possibleDoorVertPos;
    List<Vector3Int> possibleDoorHorzPos;
    List<Vector3Int> possibleWallHorzPos;
    List<Vector3Int> possibleWallVertPos;

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon(); // Call method to create the dungeon
    }

    // Method to create the dungeon layout, including rooms and walls
    private void CreateDungeon()
    {
        // Instantiate the DungeonGenerator with the specified width and length
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);

        // Generate a list of rooms (nodes) based on the dungeon parameters
        var listOfRooms = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth);

        // Create a parent GameObject to hold all the wall objects
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        // Initialize lists to track possible door and wall positions
        possibleDoorVertPos = new List<Vector3Int>();
        possibleDoorHorzPos = new List<Vector3Int>();
        possibleWallHorzPos = new List<Vector3Int>();
        possibleWallVertPos = new List<Vector3Int>();

        // Iterate over each room and create a mesh for it
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        // Create the walls for the dungeon based on calculated positions
        CreateWalls(wallParent);
    }

    // Method to create walls at the specified positions
    private void CreateWalls(GameObject wallParent)
    {
        // Instantiate horizontal walls at possible horizontal positions
        foreach (var wallPos in possibleWallHorzPos)
        {
            CreateWall(wallParent, wallPos, WallHorz);
        }

        // Instantiate vertical walls at possible vertical positions
        foreach (var wallPos in possibleWallVertPos)
        {
            CreateWall(wallParent, wallPos, wallVert);
        }
    }

    // Method to instantiate a wall at a given position using a wall prefab
    private void CreateWall(GameObject wallParent, Vector3Int wallPos, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
    }

    // Method to create a mesh for a room given the bottom-left and top-right corner coordinates
    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        // Convert 2D corner coordinates to 3D vertices (X, Y, Z)
        Vector3 bottomLeftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertex = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        // Create an array of vertices for the mesh
        Vector3[] vertices = new Vector3[] { topLeftVertex, topRightVertex, bottomLeftVertex, bottomRightVertex };

        // Generate UVs for the mesh (2D texture mapping)
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z); // Map x and z coordinates to UV space
        }

        // Define the triangles for the mesh (clockwise order)
        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        // Create a new mesh object
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Create the GameObject to hold the mesh
        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
        dungeonFloor.transform.position = Vector3.zero; // Set position to (0,0,0)
        dungeonFloor.transform.localScale = Vector3.one; // Set scale to 1
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        // Adjust the BoxCollider to fit the mesh
        BoxCollider boxCollider = dungeonFloor.GetComponent<BoxCollider>();

        // Calculate the center and size for the BoxCollider based on the mesh
        Vector3 meshCenter = (bottomLeftVertex + topRightVertex) / 2f;
        Vector3 meshSize = topRightVertex - bottomLeftVertex;

        // Set the BoxCollider's center and size
        boxCollider.center = dungeonFloor.transform.InverseTransformPoint(meshCenter);
        boxCollider.size = new Vector3(meshSize.x, 0.1f, meshSize.z); // Slight thickness for the Y axis

        // Add positions for horizontal walls (bottom and top sides of the room)
        for (int row = (int)bottomLeftVertex.x; row < (int)bottomRightVertex.x; row++)
        {
            var wallPos = new Vector3(row, 0, bottomLeftVertex.z);
            AddWallPosToList(wallPos, possibleWallHorzPos, possibleDoorHorzPos);
        }

        for (int row = (int)topLeftVertex.x; row < (int)topRightCorner.x; row++)
        {
            var wallPos = new Vector3(row, 0, topRightVertex.z);
            AddWallPosToList(wallPos, possibleWallHorzPos, possibleDoorHorzPos);
        }

        // Add positions for vertical walls (left and right sides of the room)
        for (int col = (int)bottomLeftVertex.z; col < (int)topLeftVertex.z; col++)
        {
            var wallPos = new Vector3(bottomLeftVertex.x, 0, col);
            AddWallPosToList(wallPos, possibleWallVertPos, possibleDoorVertPos);
        }

        for (int col = (int)bottomRightVertex.z; col < (int)topRightVertex.z; col++)
        {
            var wallPos = new Vector3(bottomRightVertex.x, 0, col);
            AddWallPosToList(wallPos, possibleWallVertPos, possibleDoorVertPos);
        }
    }

    // Helper method to add wall positions to the wall list or door list
    private void AddWallPosToList(Vector3 wallPos, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPos); // Round wall position to nearest integer
        if (wallList.Contains(point)) // If wall already exists at this position, it's a door
        {
            doorList.Add(point);
            wallList.Remove(point); // Remove from wall list since it's now a door
        }
        else
        {
            wallList.Add(point); // Add position to the wall list if it's not a door
        }
    }
}
