using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile
{
    public GameObject turret;
    public bool isWalkable; // �ǹ��� ��ġ�Ǹ� �� �ɾ.
    public int gCost; // ������ ��� ���
    public int hCost; // ������ �������� ���
    public Vector3Int parentHex; // �θ����� ��ǥ

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
