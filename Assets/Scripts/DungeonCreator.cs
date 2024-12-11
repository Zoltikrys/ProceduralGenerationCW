using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth;
    public int dungeonLength;
    
    public int roomWidthMin;
    public int roomLengthMin;

    public int maxIterations;
    public int corridorWidth;

    public Material material; //material of meshes

    [Range(0f, 0.3f)]
    public float roomBottomCornerModifier;

    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;

    [Range(0, 2)]
    public int roomOffset;

    public GameObject wallVert;
    public GameObject WallHorz;

    List<Vector3Int> possibleDoorVertPos;
    List<Vector3Int> possibleDoorHorzPos;
    List<Vector3Int> possibleWallHorzPos;
    List<Vector3Int> possibleWallVertPos;

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        possibleDoorVertPos = new List<Vector3Int>();
        possibleDoorHorzPos = new List<Vector3Int>();
        possibleWallHorzPos = new List<Vector3Int>();
        possibleWallVertPos = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
        CreateWalls(wallParent);
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach(var wallPos in possibleWallHorzPos)
        {
            CreateWall(wallParent, wallPos, WallHorz);
        }
        foreach(var wallPos in possibleWallVertPos)
        {
            CreateWall(wallParent, wallPos, wallVert);
        }

    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPos, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightCorner.x, 0 , bottomLeftCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertex = new Vector3(topRightCorner.x, 0, topRightCorner .y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftVertex, topRightVertex, bottomLeftVertex, bottomRightVertex
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0, 1, 2, 2, 1, 3 //clockwise order
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //setting default values of mesh
        GameObject dungeonFloor = new GameObject("Mesh"+bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
        dungeonFloor.transform.position = Vector3.zero; //set position to (0,0,0)
        dungeonFloor.transform.localScale = Vector3.one; //set scale to 1
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        // Adjust the BoxCollider to fit the mesh
        BoxCollider boxCollider = dungeonFloor.GetComponent<BoxCollider>();

        // Calculate the center and size for the BoxCollider
        Vector3 meshCenter = (bottomLeftVertex + topRightVertex) / 2f;
        Vector3 meshSize = topRightVertex - bottomLeftVertex;

        boxCollider.center = dungeonFloor.transform.InverseTransformPoint(meshCenter);
        boxCollider.size = new Vector3(meshSize.x, 0.1f, meshSize.z); // Slight thickness for the Y axis

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

    private void AddWallPosToList(Vector3 wallPos, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPos);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
}
