using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseHover : MonoBehaviour
{
    public GameObject prefab; // ������ ������Ʈ�� ������(Prefab)
    private GameObject spawnedObject; // ������ ������Ʈ�� ����
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
                // �ش� ��ġ�� ������Ʈ�� ������ �����ϰ�, ������ ��ġ�� ������Ʈ
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
            // ���콺�� ������Ʈ ������ ������ �� ������ ������Ʈ ����
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }
}
