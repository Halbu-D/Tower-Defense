using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class Hex : MonoBehaviour
{
    private HexCoordinates hexCoordinates;
    public bool isWalkable;
    public int gCost; // 인접한 노드 비용
    public int hCost; // 목적지 노드까지의 비용
    public Hex parentHex;

    public Vector3Int HexCoords => hexCoordinates.GetHexCoords();

    private void Awake()
    {
        hexCoordinates = GetComponent<HexCoordinates>();
        isWalkable = true;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Vector3 worldCoordinates
    {
        get { return transform.GetChild(0).transform.position; }
    }
}
