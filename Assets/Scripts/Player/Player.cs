using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;
    Transform playerTransform;

    [Header("플레이어 이동속도 및 회전속도")]
    public float moveSpeed = 2.0f;
    float moveDirection = 0.0f;
    public float rotateSpeed = 90.0f;
    float rotateDirection = 0.0f;

    public Transform firePosition;
    public Transform crosshair;
    public GameObject muzzleFlashPrefab;

    [Header("마우스 감도")]
    public float mouseRotationSpeed = 0.3f;
    float rotationY;
    float rotationX;

    private void Awake()
    {
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        playerTransform = transform;
        Cursor.lockState = CursorLockMode.Locked;  // 게임창에 마우스 커서 안보임
    }

    private void Update()
    {
        crosshair.position = playerTransform.position;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();   
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Shooting.performed += OnShootingInput;
        inputActions.Player.Shooting.canceled += OnShootingInput;
        inputActions.Player.MouseMove.performed += MouseMove;
        inputActions.Player.MouseMove.canceled += MouseMove;
    }

    private void OnDisable()
    {
        inputActions.Player.MouseMove.canceled -= MouseMove;
        inputActions.Player.MouseMove.performed -= MouseMove;
        inputActions.Player.Shooting.canceled -= OnShootingInput;
        inputActions.Player.Shooting.performed -= OnShootingInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }

    /// <summary>
    /// 마우스 움직임에 따라 카메라가 회전함
    /// </summary>
    /// <param name="context"></param>
    private void MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();  // inputActions에 마우스 path를 delta로 설정 액션 타입 value vector2 

        rotationY += delta.x * mouseRotationSpeed;
        rotationX -= delta.y * mouseRotationSpeed;

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        
    }

    /// <summary>
    /// 마우스 좌클릭으로 총을 쏘게 함
    /// </summary>
    /// <param name="context"></param>
    private void OnShootingInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shooting();
        }
    }

    /// <summary>
    /// 총 쏘는 모습 구현 함수 레이캐스트와 파티클을 활용하여 총 쏘는 모습을 구현함
    /// </summary>
    private void Shooting()
    {
        RaycastHit hit;
        Ray ray = new Ray(firePosition.position, firePosition.forward); 

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                ScoreBoard scoreBoard = FindObjectOfType<ScoreBoard>();
                if (scoreBoard != null)
                {
                    scoreBoard.KillEnemy();
                }
            }
        }

        Instantiate(muzzleFlashPrefab, firePosition.position, firePosition.rotation);
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
    }

    void Move()
    {
        Vector3 dir = transform.forward * moveDirection + transform.right * rotateDirection;
        dir.Normalize();

        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * dir);
    }

    void Rotate()
    {
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        rigid.MoveRotation(rigid.rotation * rotate);
    }


    private void OnDrawGizmosSelected()  // Ray의 방향을 체크하기 위해 그림
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePosition.position, firePosition.forward * 50f); 
    }
}
