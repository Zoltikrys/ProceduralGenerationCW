using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator
{
    // List to store all the space nodes (rooms and corridors) in the dungeon
    List<RoomNode> allSpaceNodes = new List<RoomNode>();

    // Width and length of the dungeon grid
    private int dungeonWidth;
    private int dungeonLength;

    // Constructor to initialize the dungeon generator with a specified width and length
    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    // This method generates a dungeon by calculating the room and corridor layout
    // It returns a list of nodes representing rooms and corridors in the dungeon
    public List<Node> CalculateDungeon(int maxIterations, int roomWidthMin, int roomLengthMin,
                                       float roomBottomCornerModifier, float roomTopCornerModifier,
                                       int roomOffset, int corridorWidth)
    {
        // Initialize a binary space partitioner for the dungeon layout
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);

        // Prepare the collection of space nodes (rooms) using the BSP method
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);

        // Extract the lowest leaf nodes (room spaces) from the BSP tree
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaves(bsp.RootNode);

        // Initialize the room generator and generate rooms within the available spaces
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier,
                                                                          roomTopCornerModifier, roomOffset);

        // Initialize the corridor generator and generate corridors between rooms
        CorridorGenerator corridorGenerator = new CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridors(allSpaceNodes, corridorWidth);

        // Combine the list of rooms and corridors into a single list of nodes (rooms + corridors)
        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}
