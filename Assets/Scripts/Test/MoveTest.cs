using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public GameObject target;
    private Vector3 position;
    void Start()
    {
        target = GameObject.FindWithTag("Finish");
        position = target.transform.position + new Vector3(0, 1.4f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(gameObject.transform.position, position, 0.1f);
    }
}
