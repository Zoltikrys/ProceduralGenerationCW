using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    private List<Node> childrenNodeList;
    public List<Node> ChildrenNodeList { get => childrenNodeList; }
    public bool Visited { get; set; }
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Node Parent { get; set; }
    public int TreeLayerIndex { get; set; }

    // Automatically adds the node to the parent's child list if the parent is not null
    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>(); // Initializes the children node list
        this.Parent = parentNode; // Sets the parent of the current node

        // If there is a parent node, add the current node as its child
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    // Add a child node to the current node's child list
    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    // Remove a child node from the current node's child list
    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}
