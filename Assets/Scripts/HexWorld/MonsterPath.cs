using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterPath : MonoBehaviour
{
    public static MonsterPath Instance;

    public Tilemap tilemap;
    public Transform startPoint;
    public Transform endPoint;
    private Vector3Int startNode;
    private Vector3Int endNode;
    private List<Vector3Int> worldPath; // Ž���� ���
    public bool change = true; // ��ο� ������ ������ ��� ��Ž��
    TileManager tileManager;
    Dictionary<Vector3Int, HexTile> hexTileDict;
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeighboursDict;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        tileManager = GetComponent<TileManager>();

        startNode = tilemap.WorldToCell(startPoint.position);
        endNode = tilemap.WorldToCell(endPoint.position);

        PathFinding();
    }

    private void Start()
    {
        tileManager = TileManager.instance;
        hexTileDict = TileManager.instance.hexTileDict;
    }

    private void Update()
    {
        if(change)
        {
            PathFinding();
            change = false;
        }

/*        foreach (Vector3Int coordinates in worldPath)
        {
            Debug.Log(coordinates);
        }*/
    }

    private void PathFinding()
    {
        List<Vector3Int> openList = new List<Vector3Int>();
        List<Vector3Int> closeList = new List<Vector3Int>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Vector3Int currentNode = openList[0];

            for (int i = 1; i< openList.Count; i++)
            {
                Vector3Int keyNode = openList[i];
                if (hexTileDict[keyNode].fCost < hexTileDict[currentNode].fCost || (hexTileDict[keyNode].fCost == hexTileDict[currentNode].fCost && hexTileDict[keyNode].hCost < hexTileDict[currentNode].hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Vector3Int neighbour in tileManager.GetNeighboursFor(currentNode))
            {
                if (!hexTileDict[neighbour].isWalkable || closeList.Contains(neighbour))// �̵��Ұ� ��� Ȥ�� �̹� Ž���� �������
                    continue;

                int neighbourCost = hexTileDict[currentNode].gCost + GetDistanceCost(currentNode, neighbour);

                if (neighbourCost < hexTileDict[neighbour].gCost || !openList.Contains(neighbour))
                {
                    hexTileDict[neighbour].gCost = neighbourCost;
                    hexTileDict[neighbour].hCost = GetDistanceCost(currentNode, endNode);
                    hexTileDict[neighbour].parentHex = currentNode;

                    if(!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Vector3Int startNode, Vector3Int endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentNode = endNode; //���������� �θ��带 ã�ư�.

        while(currentNode != startNode) //������� �����ϸ� ���� ã�� ��
        {
            path.Add(currentNode);
            currentNode = hexTileDict[currentNode].parentHex; //�θ���� �̵�

        }
        path.Reverse();
        worldPath = path;
    }

    int GetDistanceCost(Vector3Int A, Vector3Int B)
    {
        int distX = Mathf.Abs(A.x - B.x);
        int distY = Mathf.Abs(A.y - B.y);

        float Z = Mathf.Pow(distX, 2) + Mathf.Pow(distY, 2);

        if(A.y % 2 == 0) // ¦��
            return Mathf.RoundToInt(Mathf.Sqrt(Z));
        else // Ȧ��
            return Mathf.FloorToInt(Mathf.Sqrt(Z));
        //return distX + distY;
    }

    public List<Vector3> GetPath()
    {
        List<Vector3> newPath = new List<Vector3>();

        foreach(Vector3Int node in worldPath)
        {
            Vector3 pos = tilemap.CellToWorld(node);
            pos.y = 0;
            newPath.Add(pos);
            
        }

        return newPath;
    }

    public bool IsBuilderable(Vector3Int buildPos)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, bool> found = new Dictionary<Vector3Int, bool>(); // �̹� Ž���� Ÿ������ ���

        queue.Enqueue(startNode);
        found.Add(startNode, true);

        while(queue.Count > 0 )
        {
            Vector3Int currentNode = queue.Dequeue(); // ���� ���

            foreach (Vector3Int node in tileManager.GetNeighboursFor(currentNode)) // ���� ��忡 �̿��� ��� ��带 ������
            {
                if (node == endNode)
                    return true;
                if (node == buildPos)
                    continue;

                if (found.ContainsKey(node)) //Ű�� ���ԵǾ�� ��
                {
                    if (!found[node] == true) // Ž���� ��尡 �ƴ� ��
                    {
                        HexTile hex = hexTileDict[node];
                        if (hex.isWalkable == true) //��� ������ ���
                        {
                            queue.Enqueue(node);
                        }
                        found[node] = true;
                    }

                }
                else // found ��ųʸ��� Ű ��ǥ�� ���� ��
                {
                    HexTile hex = hexTileDict[node];
                    if (hex.isWalkable == true) // ��������� ���
                        queue.Enqueue(node);
                    found.Add(node, true); // ���� �߰�
                }
            }
        }

        return false;
    }

}
