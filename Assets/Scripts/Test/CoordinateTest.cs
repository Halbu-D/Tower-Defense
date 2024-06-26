using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateTest : MonoBehaviour
{
    [SerializeField]
    Grid grid;

    
    void Update()
    {
        Vector3Int gridPosition = grid.WorldToCell(transform.position);
        Debug.Log(gridPosition);
    }
}
