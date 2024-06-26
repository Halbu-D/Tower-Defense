using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MonsterSpawner : MonoBehaviour
{
    //public Transform enemyPrefab; // 단일 스폰
    public static int monsterCount = 0;
    private bool monsterFlag = true;

    public MonsterDB database;

    public Transform spawnPoint;
    public Text currentWave;
    public Text waveCountdown;

    public float waveRate = 5.5f;
    private float countdown = 5f;
    private float spawnRate = 2.5f;

    public static int waveIndex = 0;

    private void Start()
    {
        waveIndex = 0;
    }

    private void Update()
    {
        if (monsterCount > 0) // 몬스터가 없어야 다음 웨이브 넘어감
            return;

        waveCountdown.text = "Next Wave : " + Mathf.Round(countdown).ToString(); //다음 웨이브 남은 시간 표기
        if (countdown <= 0f)
        {
            monsterFlag = false;
            waveCountdown.gameObject.SetActive(false);
            StartCoroutine(StartWave());
            countdown = waveRate;
            if (spawnRate > 0.5f) //출현 속도 증가
                spawnRate -= 0.1f;
        }
        if(monsterFlag)
        {
            waveCountdown.gameObject.SetActive(true);
            countdown -= Time.deltaTime;
        }
    }

    private IEnumerator StartWave()
    {
        waveIndex++;
        GameManager.Rounds++;

        currentWave.text = "Wave " + waveIndex.ToString();
        for (int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy();
            if (i == waveIndex - 1)
                monsterFlag = true;
            yield return new WaitForSeconds(spawnRate);
        }
        
    }

    void SpawnEnemy()
    {
        int number = RandomPick();

        Instantiate(database.monsterData[number].Prefab, spawnPoint.position, Quaternion.Euler(0, 90, 0));
        monsterCount++;
    }

    public int RandomPick()
    {
        float pivot = Random.Range(0.0f, 1.0f);
        float sum = 0;

        for (int i = 0; i < database.monsterData.Count; i++)
        {
            sum += database.monsterData[i].SpawnRate;
            if (sum >= pivot)
                return i;
        }

        return 0;
    }

}