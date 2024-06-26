using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile
{
    public GameObject turret;
    public bool isWalkable; // 건물이 설치되면 못 걸어감.
    public int gCost; // 인접한 노드 비용
    public int hCost; // 목적지 노드까지의 비용
    public Vector3Int parentHex; // 부모노드의 좌표

    public HexTile()
    {
        isWalkable = true;
        gCost = 0;
        hCost = 0;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }
}
