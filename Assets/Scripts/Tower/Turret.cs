using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    private Monster targetEnemy;
    public AudioSource attackSound;

    [Header("포탑 설정")]
    public float range = 3f;
    public int upgradeCost = 100;
    public int upgradeCost2 = 100;


    [Header("단발식 타워")]
    public GameObject bulletPrefab; // 사용할 총알
    public float fireRate = 1f; //초당 발사속도
    private float fireCountdown = 0f;
    public float fireRateIncrease = 0.5f;


    [Header("지속딜")]
    public bool useLaser = false;
    public int damageOverTime = 30;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public float slowPercent = 0.5f;
    public int damageIncrease = 20;
    private float loopStartTime = 1.0f;
    private float loopEndTime = 3.0f;

    [Header("마법타워")]
    public bool mageTower = false;
    public ParticleSystem magicEffect;

    [Header("유니티 설정")]
    public string enemyTag = "Enemy";
    public Transform partToRotate; // 회전에 사용
    public float turnSpeed = 10f;

    
    public Transform firePoint; // 총알 발사위치
    public Grid worldMap;
    GameObject grid;


    public int rangeUpgraded = 1; //사거리 업그레이드 상태
    public int secondUpgraded = 1; // 데미지 or 공격속도 업그레이드 상태

    private int maxUpgrade = 3;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f); // 1초에 2번씩 호출
        grid = GameObject.FindWithTag("Grid");
        worldMap = grid.GetComponent<Grid>();
        attackSound = GetComponent<AudioSource>();

        Debug.Log(worldMap.WorldToCell(transform.position));
    }

    private void UpdateTarget() // 추적할 타겟 갱신
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null; //가까운 적

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance) //가까운적 발견시 갱신
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Monster>();
        }
        else // 범위를 벗어날 경우
        {
            target = null;
        }
    }

    void Update()
    {
        if(target == null) // 타겟이 없으면 아무것도 안함.
        {
            if(useLaser)
            {
                if(lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                }
                
                attackSound.Stop();
            }
            
            return;
        }
 

        LockOnTarget();

        if (useLaser)
        {
            Laser();
        }
        else if (mageTower)
        {
            if (fireCountdown <= 0f)
            { 
                Magic();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }

    }

    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir); //회전을 처리
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f); // y축만 회전을 하기 위함.
    }

    void Laser()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowPercent);

        if (!lineRenderer.enabled) // 라인 렌더러 활성화
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            attackSound.Play();
        }

        if (attackSound.time >= loopEndTime)
            attackSound.time = loopStartTime;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.GetChild(0).position);

        Vector3 dir = firePoint.position - target.position;
        impactEffect.transform.position = target.position + dir.normalized * 0.5f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    void Magic()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        ParticleSystem magicObj = Instantiate(magicEffect, firePoint.position, firePoint.rotation);
        magicObj.transform.localScale = GetScale();
        magicObj.Play();

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if(distanceToEnemy <= range)
            {
                enemy.GetComponent<Monster>().TakeDamage(damageOverTime);
            }
        }

        Destroy(magicObj.gameObject, 4f);
    }

    void Shoot()
    {
        GameObject bulletObj = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        attackSound.Play();

        if (bullet != null)
        {
            //Debug.Log(target.GetChild(0));
            bullet.Seek(target.GetChild(0).transform); //첫번째 자식의 위치를 전달-> 타격위치
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void UpgradeRange()
    {
        if(rangeUpgraded <= maxUpgrade)
        {
            range += 2f;
            rangeUpgraded++;
        }
            
    }

    public void UpgradeDamage() // 레이저, 번개타워 전용
    {
        if(secondUpgraded <= maxUpgrade)
        {
            damageOverTime += damageIncrease;
            secondUpgraded++;
        }
            
    }

    public void UpgradeSpeed()
    {
        if(secondUpgraded <= maxUpgrade)
        {
            fireRate += fireRateIncrease;
            secondUpgraded++;
        }
            
    }
    
    Vector3 GetScale()
    {
        Vector3 scale = new Vector3(0.75f, 0.75f, 0.75f); ;

        if (rangeUpgraded == 2)
            scale = new Vector3(1.25f, 1.25f, 1.25f) ;
        else if(rangeUpgraded == 3)
            scale = new Vector3(1.75f, 1.75f, 1.75f);
        else if(rangeUpgraded == 4)
            scale = new Vector3(2.25f, 2.25f, 2.25f);

        return scale;
    }
}
