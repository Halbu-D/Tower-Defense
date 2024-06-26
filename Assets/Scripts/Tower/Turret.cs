using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    private Monster targetEnemy;
    public AudioSource attackSound;

    [Header("��ž ����")]
    public float range = 3f;
    public int upgradeCost = 100;
    public int upgradeCost2 = 100;


    [Header("�ܹ߽� Ÿ��")]
    public GameObject bulletPrefab; // ����� �Ѿ�
    public float fireRate = 1f; //�ʴ� �߻�ӵ�
    private float fireCountdown = 0f;
    public float fireRateIncrease = 0.5f;


    [Header("���ӵ�")]
    public bool useLaser = false;
    public int damageOverTime = 30;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public float slowPercent = 0.5f;
    public int damageIncrease = 20;
    private float loopStartTime = 1.0f;
    private float loopEndTime = 3.0f;

    [Header("����Ÿ��")]
    public bool mageTower = false;
    public ParticleSystem magicEffect;

    [Header("����Ƽ ����")]
    public string enemyTag = "Enemy";
    public Transform partToRotate; // ȸ���� ���
    public float turnSpeed = 10f;

    
    public Transform firePoint; // �Ѿ� �߻���ġ
    public Grid worldMap;
    GameObject grid;


    public int rangeUpgraded = 1; //��Ÿ� ���׷��̵� ����
    public int secondUpgraded = 1; // ������ or ���ݼӵ� ���׷��̵� ����

    private int maxUpgrade = 3;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f); // 1�ʿ� 2���� ȣ��
        grid = GameObject.FindWithTag("Grid");
        worldMap = grid.GetComponent<Grid>();
        attackSound = GetComponent<AudioSource>();

        Debug.Log(worldMap.WorldToCell(transform.position));
    }

    private void UpdateTarget() // ������ Ÿ�� ����
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null; //����� ��

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance) //������� �߽߰� ����
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
        else // ������ ��� ���
        {
            target = null;
        }
    }

    void Update()
    {
        if(target == null) // Ÿ���� ������ �ƹ��͵� ����.
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
        Quaternion lookRotation = Quaternion.LookRotation(dir); //ȸ���� ó��
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f); // y�ุ ȸ���� �ϱ� ����.
    }

    void Laser()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowPercent);

        if (!lineRenderer.enabled) // ���� ������ Ȱ��ȭ
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
            bullet.Seek(target.GetChild(0).transform); //ù��° �ڽ��� ��ġ�� ����-> Ÿ����ġ
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

    public void UpgradeDamage() // ������, ����Ÿ�� ����
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
