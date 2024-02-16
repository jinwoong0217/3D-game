using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody rigid;
    private CapsuleCollider capsuleCollider;
    public Transform target;

    private bool isFollowingPlayer = false;
    public float moveSpeed = 4.0f;
    public float rotationSpeed = 180.0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            FollowPlayer();
        }
        else
        {
            MoveRandom();
        }
    }

    private void MoveRandom()
    {
        if (Random.value < 0.25f)
        {
            float randomRotation = Random.Range(-1f, 1f);
            Quaternion rotation = Quaternion.AngleAxis(randomRotation * rotationSpeed * Time.fixedDeltaTime, transform.up);
            rigid.MoveRotation(rotation * rigid.rotation);
        }
        rigid.MovePosition(transform.position + transform.forward * moveSpeed * Time.fixedDeltaTime);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.CompareTag("Wall")) 
            {
                Vector3 newDirection = Vector3.Reflect(transform.forward, hit.normal);
                Quaternion rotation = Quaternion.LookRotation(newDirection);
                rigid.MoveRotation(rotation * rigid.rotation);
            }
        }
    }

    private void FollowPlayer()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.CompareTag("Wall")) 
            {
                Vector3 newDir = Vector3.Reflect(transform.forward, hit.normal);
                Quaternion newRotation = Quaternion.LookRotation(newDir);
                transform.rotation = newRotation;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFollowingPlayer)
        {
            isFollowingPlayer = true;
            FollowPlayer(); 
        }
    }
}


