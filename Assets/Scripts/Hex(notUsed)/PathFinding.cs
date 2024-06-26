using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance;

    HexGrid hexgrid;
    public List<Hex> worldPath;
    //public GameObject startPoint;
    //public GameObject endPoint;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        hexgrid = GetComponent<HexGrid>();

        FindPath();
    }

    void Update()
    {
        FindPath();

        /*
        foreach(Hex hex in worldPath)
        {
            Debug.Log(hex.HexCoords);
        }
        */
    }

    private void FindPath()
    {
        Hex startNode = hexgrid.GetTileAt(new Vector3Int(0, 0, 4));
        Hex endNode = hexgrid.GetTileAt(new Vector3Int(11, 0, 4));

        List<Hex> openList = new List<Hex>();
        List<Hex> closeList = new List<Hex>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Hex currentNode = openList[0];

            for(int i = 1; i< openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if(currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach(Vector3Int node in hexgrid.GetNeighboursFor(currentNode.HexCoords)) //이웃 노드 탐색
            {
                foreach(Hex hex in FindObjectsOfType<Hex>()) // 이웃인 노드만
                {
                    if(hex.HexCoords == node)
                    {
                        if (!hex.isWalkable || closeList.Contains(hex)) // 이동불가 노드이거나 이미 탐색이 끝난 경우
                            continue;//break;

                        int neighbourCost = currentNode.gCost + GetDistanceCost(currentNode.HexCoords, node);

                        if(neighbourCost < hex.gCost || !openList.Contains(hex))
                        {
                            hex.gCost = neighbourCost;
                            hex.hCost = GetDistanceCost(currentNode.HexCoords, endNode.HexCoords);
                            hex.parentHex = currentNode;

                            if(!openList.Contains(hex))
                                openList.Add(hex);
                        }
                    }
                }
            }

        }
    }


    void RetracePath(Hex startNode, Hex endNode)
    {
        List<Hex> path = new List<Hex>();
        Hex currentNode = endNode;

        while(currentNode != startNode) 
        {
            path.Add(currentNode);
            currentNode = currentNode.parentHex;
        }
        path.Reverse();
        worldPath = path; 
    }

    int GetDistanceCost(Vector3Int A, Vector3Int B)
    {
        int distX = Mathf.Abs(A.x - B.x);
        int distZ = Mathf.Abs(A.z - B.z);

        if (distX > distZ)
            return 17 * distZ + 10 * (distX - distZ);

        return 17 * distX + 10 * (distZ - distX);
    }
}
