using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Node Settings")]
    public int width;
    public int height;
    public float nodeSizeX;
    public float nodeSizeY;

    [Header("Random Settings")]
    public int randomWallChance = 1;

    [Header("Start Pathfinding / Reset Grid")]
    public bool start;

    [Header("Grid")]
    public List<List<Node>> Grid = new List<List<Node>>();
    public List<Node> path = new List<Node>();
    public GameObject GridNodes;
    
    [Header("Start/End Node GameObjects")]
    [SerializeField] GameObject startNodeGO;
    [SerializeField] GameObject endNodeGO;


    [Header("Run NPCs")]
    public bool npcs;
    public bool spawnNPC;
    public List<GameObject> npcList = new List<GameObject>(); 

    void Start()
    {
        CreateGrid();
        AddNeighbors();
    }

    private void ResetGrid()
    {
        Destroy(GridNodes);
        Grid = new List<List<Node>>();
        CreateGrid();
        AddNeighbors();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray;

                if (!Grid[x][y].isPassable)
                {
                    Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray2;
                }
            }
        }
    }

    private void Update()
    {
        if (start)
        {
            start = !start;

            ResetGrid();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray;

                    if (!Grid[x][y].isPassable)
                    {
                        Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray2;
                    }
                }
            }

            Node startNode = Grid[Random.Range(0, width - 1)][Random.Range(0, height - 1)];
            Node endNode = Grid[Random.Range(0, width - 1)][Random.Range(0, height - 1)];

            while (!startNode.isPassable && !endNode.isPassable)
            {
                if (startNode.isPassable && startNode != endNode)
                {
                    startNodeGO = startNode.nodeObject;
                }
                else
                {
                    startNode = Grid[Random.Range(0, width - 1)][Random.Range(0, height - 1)];
                }

                if (endNode.isPassable && endNode != startNode)
                {
                    endNodeGO = endNode.nodeObject;
                }
                else
                {
                    endNode = Grid[Random.Range(0, width - 1)][Random.Range(0, height - 1)];
                }
            }

            StartCoroutine(FindPath(startNode, endNode));
        }

        if (spawnNPC && npcList.Count < 3)
        {
            spawnNPC = !spawnNPC;

            GameObject newNPC = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newNPC.AddComponent<NPC>();

            newNPC.name = "NPC " + npcList.Count;
            npcList.Add(newNPC);
        }
    }

    private void CreateGrid()
    {
        GridNodes = Instantiate(new GameObject());
        GridNodes.name = "Grid Nodes";

        for (int x = 0; x < width; x++)
        {
            Grid.Add(new List<Node>()); // Grid[x] = new List

            for (int y = 0; y < height; y++)
            {
                Vector3 position;

                if (nodeSizeX <= 0 || nodeSizeY <= 0)
                {
                    position = new Vector3
                    (
                        x,
                        0,
                        y
                    );
                }
                else
                {
                    position = new Vector3
                    (
                        x * nodeSizeX,
                        0,
                        y * nodeSizeY
                    );
                }

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.name = "X: " + position.x + ", Y: " + position.z;
                cube.transform.SetParent(GridNodes.transform);

                Grid[x].Add(new Node(x, y, position, cube)); // Adding the y: Grid[x][y] = the new Node

                Grid[x][y].isPassable = RandomPassable();
            }
        }
    }

    private bool RandomPassable()
    {
        bool ret;
        int rnd = Random.Range(0, randomWallChance);

        if (rnd > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }

        return ret;
    }

    private void AddNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = Grid[x][y];
                node.neighborNodes.Clear();

                if (x > 0)
                {
                    node.neighborNodes.Add(Grid[x - 1][y]);  //left 
                }
                if (x < width - 1)
                {
                    node.neighborNodes.Add(Grid[x + 1][y]);  //right
                }
                if (y > 0)
                {
                    node.neighborNodes.Add(Grid[x][y - 1]);  //down
                }
                if (y < height - 1)
                {
                    node.neighborNodes.Add(Grid[x][y + 1]);  //up
                }
            }
        }
    }

    public float GetDistance(Node nodeFrom, Node nodeTo)
    {
        float distance;
        float xDiff = nodeTo.X - nodeFrom.X;
        float yDiff = nodeTo.Y - nodeFrom.Y;

        distance = xDiff + yDiff;

        return distance;
    }

    public IEnumerator FindPath(Node startNode, Node endNode)
    {
        List<Node> closedSet = new List<Node>();
        List<Node> openSet = new List<Node>();

        startNode.G = 0;
        startNode.H = GetDistance(startNode, endNode);
        startNode.parentNode = null;
        
        openSet.Add(startNode);
        Node currentNode = openSet[0];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!Grid[x][y].isPassable)
                {
                    Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray2;
                }
            }
        }

        while (openSet.Count > 0)
        {
            #region ChangeColor
            if (!npcs)
            {
                if (closedSet.Count > 0)
                {
                    foreach (Node node in closedSet)
                    {
                        node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.lightBlue;
                    }
                }
                foreach (Node node in openSet)
                {
                    node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.lightGreen;
                }
                startNode.nodeObject.GetComponent<MeshRenderer>().material.color = Color.green;
                endNode.nodeObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            #endregion

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (!closedSet.Contains(neighbor) && neighbor.isPassable)
                {
                    float potentialG = currentNode.G + GetDistance(currentNode, neighbor);

                    if (!openSet.Contains(neighbor) || potentialG < neighbor.G)
                    {
                        neighbor.G = potentialG;
                        neighbor.H = GetDistance(neighbor, endNode);
                        neighbor.parentNode = currentNode;
                        openSet.Add(neighbor);
                    }
                }
            }

            foreach (Node node in openSet)
            {
                if (node.GetFCost() <= currentNode.GetFCost())
                {
                    currentNode = node;
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);
                    break;
                }
            }

            if (currentNode == endNode)
            {
                break;
            }

            yield return new WaitForSeconds(0.005f);
        }

        RetracePath(startNode, endNode);

        yield break;
    }

    public IEnumerator FindPathNPC(Node startNode, Node endNode, GameObject NPC)
    {
        NPC.GetComponent<NPC>().trackingPath = true;

        List<Node> closedSet = new List<Node>();
        List<Node> openSet = new List<Node>();

        startNode.G = 0;
        startNode.H = GetDistance(startNode, endNode);
        startNode.parentNode = null;

        openSet.Add(startNode);
        Node currentNode = openSet[0];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!Grid[x][y].isPassable)
                {
                    Grid[x][y].nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray2;
                }
            }
        }

        while (openSet.Count > 0)
        {
            #region ChangeColor
            if (!npcs)
            {
                if (closedSet.Count > 0)
                {
                    foreach (Node node in closedSet)
                    {
                        node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.lightBlue;
                    }
                }
                foreach (Node node in openSet)
                {
                    node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.lightGreen;
                }
                startNode.nodeObject.GetComponent<MeshRenderer>().material.color = Color.green;
                endNode.nodeObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            #endregion

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (!closedSet.Contains(neighbor) && neighbor.isPassable)
                {
                    float potentialG = currentNode.G + GetDistance(currentNode, neighbor);

                    if (!openSet.Contains(neighbor) || potentialG < neighbor.G)
                    {
                        neighbor.G = potentialG;
                        neighbor.H = GetDistance(neighbor, endNode);
                        neighbor.parentNode = currentNode;
                        openSet.Add(neighbor);
                    }
                }
            }

            foreach (Node node in openSet)
            {
                if (node.GetFCost() <= currentNode.GetFCost())
                {
                    currentNode = node;
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);
                    break;
                }
            }

            if (currentNode == endNode)
            {
                break;
            }

            yield return new WaitForSeconds(0.005f);
        }

        NPC.GetComponent<NPC>().nextNode = NextPathNodeNPC(startNode, endNode);

        NPC.GetComponent<NPC>().trackingPath = false;
    }

    public List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> pathList = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathList.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        pathList.Add(startNode);
        pathList.Reverse();

        if (!npcs)
        {
            foreach (Node node in pathList)
            {
                node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }

        return pathList;
    }

    public Node NextPathNodeNPC(Node startNode, Node endNode)
    {
        List<Node> pathList = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathList.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        pathList.Add(startNode);
        pathList.Reverse();

        foreach (Node node in pathList)
        {
            node.nodeObject.GetComponent<MeshRenderer>().material.color = Color.lightBlue;
        }

        return pathList.Count > 1 ? pathList[1] : pathList[0];
    }
}
