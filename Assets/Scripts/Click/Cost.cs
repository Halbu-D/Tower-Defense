using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cost : MonoBehaviour
{
    [SerializeField]
    private TowerDB database;

    public int towerIndex;
    public Text towerCost;

    private void Start()
    {
        SetCost();
    }

    void SetCost()
    {
        towerCost.text = database.towerData[towerIndex].Cost.ToString();
    }
}
