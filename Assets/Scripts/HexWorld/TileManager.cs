using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public Tilemap tilemap;
    public Transform tilePos;
    public Dictionary<Vector3Int, HexTile> hexTileDict = new Dictionary<Vector3Int, HexTile>();//해당 좌표의 타일 정보
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();// 타일과 근접한 타일정보

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        Vector3Int endPoint = tilemap.WorldToCell(tilePos.position);
        for (int x = 0; x <= endPoint.x; x++) // 행
            for (int y = 0; y <= endPoint.y; y++) // 열
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                hexTileDict.Add(pos, new HexTile());
                //Debug.Log(pos);
            }
/*        foreach(KeyValuePair<Vector3Int, HexTile> item in hexTileDict)
        {
            Debug.Log(item.Key);
        }*/
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates) //인접한 타일목록 추가
    {
        if(hexTileDict.ContainsKey(hexCoordinates) == false)
            return new List<Vector3Int>();
        
        if(hexTileNeighboursDict.ContainsKey(hexCoordinates))
            return hexTileNeighboursDict[hexCoordinates];

        hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3Int>());

        foreach (Vector3Int direction in Direction.GetDirectionList(hexCoordinates.y))
        {
            if(hexTileDict.ContainsKey(hexCoordinates + direction))
            {
                hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
            }
        }
        return hexTileNeighboursDict[hexCoordinates];
    }
}

public static class Direction
{
    public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>() // 홀수행
        {
            new Vector3Int(1, 0, 0), // 동
            new Vector3Int(1, 1, 0), // 북동
            new Vector3Int(1, -1, 0), // 남동
            new Vector3Int(0, -1, 0), // 남서
            new Vector3Int(0, 1, 0), // 북서
            new Vector3Int(-1, 0, 0) // 서

        };
    public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>() //짝수행
        {
            new Vector3Int(1, 0, 0), // E
            new Vector3Int(0, 1, 0), // N2
            new Vector3Int(0, -1, 0), // S2
            new Vector3Int(-1, -1, 0), // S1
            new Vector3Int(-1, 1, 0), // N1
            new Vector3Int(-1, 0, 0) // W
        };

    public static List<Vector3Int> GetDirectionList(int y) => y % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;
}
