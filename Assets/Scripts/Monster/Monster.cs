using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    public float startSpeed = 1f;
    public float startHealth = 100f;
    public float hpRate = 2f;

    [HideInInspector]
    public float speed = 1f;
    public float health = 100f;
    public int reward = 10;


    private void Start()
    {
        speed = startSpeed + MonsterSpawner.waveIndex * 0.01f;
        health = startHealth + startHealth * hpRate * MonsterSpawner.waveIndex;

    }
    public void TakeDamage(float amount)
    {
        health -= amount;

        if(health<=0)
        {
            Die();
        }
    }

    public void Slow(float percentage)
    {
        speed = startSpeed * (1- percentage);
    }


    void Die()
    {
        MonsterSpawner.monsterCount--;
        Destroy(gameObject);
        GameManager.Money += reward;
    }
}