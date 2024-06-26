using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Monster))]
public class MonsterMovement : MonoBehaviour
{
    private Vector3 target;
    private int wayPointIndex = 0;
    List<Vector3> wayPoints;

    private Monster enemy;

    private void Start()
    {
        enemy = GetComponent<Monster>();

        wayPoints = MonsterPath.Instance.GetPath();

        target = wayPoints[wayPointIndex];
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, enemy.speed * Time.deltaTime);

        if (transform.position == target)
        {
            GetNextWayPoint();
        }

        Rotation();

        enemy.speed = enemy.startSpeed;
    }

    void GetNextWayPoint()
    {
        if (wayPointIndex >= wayPoints.Count)
            Destroy(gameObject);
        else
        {
            wayPointIndex++;
            target = wayPoints[wayPointIndex];
        }
    }

    void Rotation()
    {
        Vector3 dir = target - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.deltaTime);
    }
}
