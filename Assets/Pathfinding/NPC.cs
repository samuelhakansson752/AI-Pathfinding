using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NPC : MonoBehaviour
{
    Node currentNode;
    Node prevNode;
    Node rndGoal;
    List<Node> nodePath;

    PathfindingGrid pfGrid;

    GameObject npcGO;
    float moveTimer = 0;

    private void Start()
    {
        pfGrid = GameObject.Find("Grid").GetComponent<PathfindingGrid>();

        currentNode = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];
        rndGoal = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];

        while (!currentNode.isPassable && !rndGoal.isPassable)
        {
            if (!currentNode.isPassable && currentNode == rndGoal)
            {
                currentNode = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];
            }
            if (!rndGoal.isPassable && rndGoal == currentNode)
            {
                rndGoal = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];
            }
        }

        currentNode.isPassable = false;
        rndGoal.nodeObject.GetComponent<MeshRenderer>().material.color = Color.red;

        npcGO = gameObject;
        npcGO.GetComponent<MeshRenderer>().material.color = Color.purple;

        Vector3 position = currentNode.position;
        position.y = 1;
        npcGO.transform.position = position;

        Move();
    }
    private void Move()
    {
        StartCoroutine(pfGrid.FindPath(currentNode, rndGoal, nodePath));

        if (nodePath[1].isPassable)
        {
            prevNode = currentNode;
            prevNode.nodeObject.GetComponent<MeshRenderer>().material.color = Color.gray;
            prevNode.isPassable = true;

            currentNode = nodePath[1];
            currentNode.isPassable = false;
        }
    }

    private void RandomizeGoal() 
    {
        rndGoal = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];

        while (!rndGoal.isPassable)
        {
            if (rndGoal == currentNode)
            {
                rndGoal = pfGrid.Grid[Random.Range(0, pfGrid.width - 1)][Random.Range(0, pfGrid.height - 1)];
            }
        }

        rndGoal.nodeObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    private void Update()
    {
        if (currentNode == rndGoal)
        {
            RandomizeGoal();
        }

        if (moveTimer > 1)
        {
            moveTimer = 0;
            Move();
        }
        else
        {
            moveTimer += Time.deltaTime;
        }
    }

}
