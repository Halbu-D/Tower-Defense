using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{

    public float moveSpeed = 8f; // �̵� �ӵ�
    public float minX = 0f;
    public float maxX = 23f;
    public float minZ = 0f;
    public float maxZ = 13f;

    void Update()
    {

        if(GameManager.GameIsOver)
        {
            this.enabled = false;
            return;
        }
            

        // Ű �Է��� �����Ͽ� �̵� ������ ���
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        float xSpeed = xInput * moveSpeed;
        float ySpeed = yInput * moveSpeed;

        Vector3 moveDirection = new Vector3(xSpeed, 0f, ySpeed);

        // �̵� ���⿡ ���� ĳ���͸� �̵�
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        //playerRigidbody.velocity = moveDirection;

        if(transform.position.x < minX)
        {
            Vector3 newLoc = transform.position;
            newLoc.x = minX;
            transform.position = newLoc;
        }
        if (transform.position.x > maxX)
        {
            Vector3 newLoc = transform.position;
            newLoc.x = maxX;
            transform.position = newLoc;
        }
        if (transform.position.z < minZ)
        {
            Vector3 newLoc = transform.position;
            newLoc.z = minZ;
            transform.position = newLoc;
        }
        if (transform.position.z > maxZ)
        {
            Vector3 newLoc = transform.position;
            newLoc.z = maxZ;
            transform.position = newLoc;
        }
    }

}