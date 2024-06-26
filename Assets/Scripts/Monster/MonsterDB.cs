using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class MonsterDB : ScriptableObject
{
    public List<MonsterData> monsterData;
}

[Serializable]
public class MonsterData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public float SpawnRate { get; private set; }

    [field: SerializeField]
    public GameObject Prefab { get; private set; }


}