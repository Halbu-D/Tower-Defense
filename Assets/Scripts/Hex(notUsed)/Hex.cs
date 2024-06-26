using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class Hex : MonoBehaviour
{
    private HexCoordinates hexCoordinates;
    public bool isWalkable;
    public int gCost; // ������ ��� ���
    public int hCost; // ������ �������� ���
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
