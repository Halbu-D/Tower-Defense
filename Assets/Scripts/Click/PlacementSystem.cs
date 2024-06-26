using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private TowerDB database;
    private int selectedTowerIndex = -1;

    Dictionary<Vector3Int, HexTile> hexTileDict;
    //[SerializeField]
    //private GameObject gridVisualization; // 모름

    //upgrade시스템
    public NodeUI nodeUI;
    private Vector3Int selectedTile; // 선택된 타일

    public AudioClip[] effectAudio;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hexTileDict = TileManager.instance.hexTileDict;
        StopPlacement();
        StopSelect();
        DefaultMode();
    }

    private void Update()
    {
        if (selectedTowerIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //Debug.Log(gridPosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToLocal(gridPosition);
    }

    public void StartPlacement(int ID) // ui로 버튼이 눌리면 호출
    {
        StopPlacement();
        StopSelect();
        selectedTowerIndex = database.towerData.FindIndex(data => data.ID == ID);
        if(selectedTowerIndex < 0)
        {
            Debug.LogError($"No ID fount {ID}");
            return;
        }
        if (database.towerData[ID].Cost > GameManager.Money)
        {
            Debug.Log("잔고 부족");
            return;
        }

        //gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        mouseIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        StopSelect();
        if (inputManager.IsPointerOverUI())
        {
            Debug.Log("UI 클릭");
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(!MonsterPath.Instance.IsBuilderable(gridPosition)) //설치 판단
        {
            Debug.Log("경로 막힘");
            audioSource.PlayOneShot(effectAudio[1], 0.8f);
            return;
        }
        else if (!hexTileDict[gridPosition].isWalkable)
        {
            Debug.Log("이미 설치됨");
            audioSource.PlayOneShot(effectAudio[1], 0.8f);
            return;
        }
        else
        {
            //mouseIndicator.transform.position = mousePosition;
            GameObject newTower = Instantiate(database.towerData[selectedTowerIndex].Prefab);
            newTower.transform.position = grid.CellToLocal(gridPosition);
            hexTileDict[gridPosition].turret = newTower; // 해당 셀에 있는 타워를 지정
            hexTileDict[gridPosition].isWalkable = false;
            MonsterPath.Instance.change = true;
            GameManager.Money -= database.towerData[selectedTowerIndex].Cost; // 설치시 돈 차감
            audioSource.PlayOneShot(effectAudio[0], 0.8f);
        }
        StopPlacement();
    }

    private void StopPlacement()
    {
        selectedTowerIndex = -1;
        //gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        mouseIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        DefaultMode();
    }


    private void DefaultMode()
    {
        inputManager.OnClicked += SelectTile;
        //inputManager.OnExit += StopSelect;
    }
    public void SelectTile()
    {
        if (inputManager.IsPointerOverUI())
        {
            Debug.Log("UI 클릭");
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (hexTileDict[gridPosition].isWalkable)
        {
            Debug.Log("타워가 없음");
            nodeUI.Hide();
            return;
        }
        else
        {
            selectedTile = gridPosition;
            nodeUI.SetTarget(selectedTile);
        }
    }

    private void StopSelect()
    {
        nodeUI.Hide();
        inputManager.OnClicked -= SelectTile;
        //inputManager.OnExit -= StopSelect;
    }
}


