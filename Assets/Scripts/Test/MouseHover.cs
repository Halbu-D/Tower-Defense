using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseHover : MonoBehaviour
{
    public GameObject prefab; // 생성할 오브젝트의 프리팹(Prefab)
    private GameObject spawnedObject; // 생성된 오브젝트의 참조
    public Tilemap tile;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Land"))
            {
                Vector3 center = hit.collider.bounds.center;
                Debug.Log(tile.WorldToCell(hit.point));
                // 해당 위치에 오브젝트가 없으면 생성하고, 있으면 위치를 업데이트
                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(prefab, center, Quaternion.identity);
                }
                else
                {
                    spawnedObject.transform.position = center;
                }
            }
        }
        else
        {
            // 마우스가 오브젝트 밖으로 나갔을 때 생성된 오브젝트 삭제
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }
}
