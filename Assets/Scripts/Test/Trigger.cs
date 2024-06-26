using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            MonsterSpawner.monsterCount--;
            Destroy(other.gameObject);
            GameManager.Lives--;
        }
    }
}
