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
    //private GameObject gridVisualization; // ��

    //upgrade�ý���
    public NodeUI nodeUI;
    private Vector3Int selectedTile; // ���õ� Ÿ��

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

    public void StartPlacement(int ID) // ui�� ��ư�� ������ ȣ��
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
            Debug.Log("�ܰ� ����");
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
            Debug.Log("UI Ŭ��");
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(!MonsterPath.Instance.IsBuilderable(gridPosition)) //��ġ �Ǵ�
        {
            Debug.Log("��� ����");
            audioSource.PlayOneShot(effectAudio[1], 0.8f);
            return;
        }
        else if (!hexTileDict[gridPosition].isWalkable)
        {
            Debug.Log("�̹� ��ġ��");
            audioSource.PlayOneShot(effectAudio[1], 0.8f);
            return;
        }
        else
        {
            //mouseIndicator.transform.position = mousePosition;
            GameObject newTower = Instantiate(database.towerData[selectedTowerIndex].Prefab);
            newTower.transform.position = grid.CellToLocal(gridPosition);
            hexTileDict[gridPosition].turret = newTower; // �ش� ���� �ִ� Ÿ���� ����
            hexTileDict[gridPosition].isWalkable = false;
            MonsterPath.Instance.change = true;
            GameManager.Money -= database.towerData[selectedTowerIndex].Cost; // ��ġ�� �� ����
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
            Debug.Log("UI Ŭ��");
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (hexTileDict[gridPosition].isWalkable)
        {
            Debug.Log("Ÿ���� ����");
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


