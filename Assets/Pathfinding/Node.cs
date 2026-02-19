using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public float X;
    public float Y;
    public Vector3 position;

    public bool isPassable = true;
    public float G;
    public float H;

    public GameObject nodeObject;
    public Node parentNode;

    public List<Node> neighborNodes = new List<Node>();

    public Node(float X, float Y, Vector3 position, GameObject nodeObject)
    {
        this.X = X; 
        this.Y = Y;
        this.position = position;
        this.nodeObject = nodeObject;
    }

    public Node(){}

    public float GetFCost()
    {
        return (G + H);
    }
}
