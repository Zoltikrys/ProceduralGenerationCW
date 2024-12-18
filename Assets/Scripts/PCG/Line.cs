using UnityEngine;

public class Line
{
    // Enum that defines the possible orientations of a line
    Orientation orientation;

    Vector2Int coordinates;

    // Constructor to initialize a new line with a specified orientation and coordinates
    public Line(Orientation orientation, Vector2Int coordinates)
    {
        this.orientation = orientation; // Set the orientation (Horizontal or Vertical)
        this.coordinates = coordinates; // Set the coordinates of the line
    }

    public Orientation Orientation { get => orientation; set => orientation = value; }

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
}
public enum Orientation
{
    Horizontal = 0, // Represents a horizontal line
    Vertical = 1    // Represents a vertical line
}
