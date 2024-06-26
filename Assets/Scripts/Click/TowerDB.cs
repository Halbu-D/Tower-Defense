using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class TowerDB : ScriptableObject
{
    public List<TowerData> towerData;
}


[Serializable]
public class TowerData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public int Cost { get; private set; }

[field: SerializeField]
    public GameObject Prefab { get; private set; }


}