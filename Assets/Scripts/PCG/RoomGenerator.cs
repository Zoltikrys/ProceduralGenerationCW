using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    // Constructor to initialize the RoomGenerator with max iterations, minimum length, and minimum width
    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
    }

    // This method generates rooms inside the given room spaces by modifying their corners
    // It returns a list of RoomNode objects with updated positions based on the modifiers.
    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        // Iterate through each space in the list of room spaces
        foreach (var space in roomSpaces)
        {
            // Generate a new bottom-left corner for the room
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset);

            // Generate a new top-right corner for the room
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerModifier, roomOffset);

            // Update the bottom-left and top-right corners of the current space
            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;

            // Recalculate and set the bottom-right and top-left corners based on the new bottom-left and top-right corners
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            // Add the updated space to the return list
            listToReturn.Add((RoomNode)space);
        }

        // Return the list of RoomNode objects with the updated corner positions
        return listToReturn;
    }
}
