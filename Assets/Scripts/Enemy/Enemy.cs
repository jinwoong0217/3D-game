using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody rigid;
    BoxCollider boxCollider;
    CapsuleCollider capsuleCollider;
    Transform target;
    Animator animator;

    bool isFollowingPlayer = false;
    
    [Header("이동 관련")]
    public float moveSpeed = 4.0f;
    public float rotationSpeed = 180.0f;

    /// <summary>
    /// 파라미터를 해시값으로 변환
    /// </summary>
    readonly int onTarget = Animator.StringToHash("onTarget");
    readonly int canAttack = Animator.StringToHash("canAttack");

    public Transform attackRange;

    float range = 3.0f;  // 적과 나 사이의 거리

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

    }

    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            animator.SetBool(onTarget, true);
            FollowPlayer();
        }
        else
        {
            MoveRandom();
        }
    }

    /// <summary>
    /// 랜덤한 위치로 스스로 움직이는 함수
    /// </summary>
    void MoveRandom()
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

    /// <summary>
    /// 타겟을 쫒아가는 함수
    /// </summary>
    void FollowPlayer()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);

        if(dir.magnitude >= range)
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
        if (other.CompareTag("HitArea"))
        {
            animator.SetBool(canAttack, true);
        }
    }

}


