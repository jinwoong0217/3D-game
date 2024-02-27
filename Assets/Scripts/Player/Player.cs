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
    /// <summary>
    /// 플레이어의 이동 속도
    /// </summary>
    public float moveSpeed = 2.0f;

    /// <summary>
    /// 플레이어의 이동 방향
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 플레이어의 회전 속도
    /// </summary>
    public float rotateSpeed = 90.0f;

    /// <summary>
    /// 플레이어의 회전 방향
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 발사 위치
    /// </summary>
    public Transform firePosition;

    /// <summary>
    /// 조준점
    /// </summary>
    public Transform crosshair;
    
    /// <summary>
    /// 플래시 파티클 
    /// </summary>
    public GameObject muzzleFlashPrefab;

    [Header("마우스 감도")]
    /// <summary>
    /// 마우스 회전속도
    /// </summary>
    public float mouseRotationSpeed = 0.3f;

    /// <summary>
    /// 마우스 Y축 회전
    /// </summary>
    float rotationY;

    /// <summary>
    /// 마우스 X축 회전
    /// </summary>
    float rotationX;

    /// <summary>
    /// 변수 초기화
    /// </summary>
    private void Awake()
    {
        inputActions = new();                     // 인풋액션 생성
        rigid = GetComponent<Rigidbody>();        // 리지드바디 컴포넌트 가져오기
        animator = GetComponent<Animator>();      // 애니메이터 컴포넌트 가져오기

        playerTransform = transform;              // 플레이어의 트랜스폼 저장
        Cursor.lockState = CursorLockMode.Locked;  // 게임창에 마우스 커서 안보임
    }

    private void Update()
    {
        crosshair.position = playerTransform.position;  // 조준점의 위치를 플레이어의 위치로 조정
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();   
    }

    /// <summary>
    /// 활성화
    /// </summary>
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

    /// <summary>
    /// 비활성화
    /// </summary>
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
        Vector2 delta = context.ReadValue<Vector2>();  // 마우스의 이동 변화량

        rotationY += delta.x * mouseRotationSpeed;
        rotationX -= delta.y * mouseRotationSpeed;

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);  // 플레이어의 회전 업데이트
        
    }

    /// <summary>
    /// 마우스 좌클릭 입력으로 총을 쏘는 액션을 수행
    /// </summary>
    /// <param name="context"></param>
    private void OnShootingInput(InputAction.CallbackContext context)
    {
        if (context.performed)  // 입력이 발생하면 
        {
            Shooting();  // 총을 쏨
        }
    }

    /// <summary>
    /// 총 쏘는 액션을 구현한 함수
    /// </summary>
    private void Shooting()
    {
        RaycastHit hit;
        Ray ray = new Ray(firePosition.position, firePosition.forward);  // 총알이 나가는 방향에 Ray 생성

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))  // Ray에 닿는 물체가 있는지 검사
        {
            if (hit.collider.CompareTag("Enemy"))  // Ray에 닿는 물체가 적인 경우
            {
                Destroy(hit.collider.gameObject);  // 적을 파괴 
                ScoreBoard scoreBoard = FindObjectOfType<ScoreBoard>();
                if (scoreBoard != null)  // 스코어보드가 있는 경우
                {
                    scoreBoard.KillEnemy();  // 적을 죽였다는걸 스코어보드에 알림
                }
            }
        }

        Instantiate(muzzleFlashPrefab, firePosition.position, firePosition.rotation);  // 총구화염 파티클 생성
    }

    /// <summary>
    /// 입력에 따라 이동 액션
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>());  // 입력값과 취소 여부를 SetInput 함수에 전달
    }

    /// <summary>
    /// 입력받은 이동 및 회전 값을 설정하는 함수
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isMove"></param>
    void SetInput(Vector2 input)
    {
        rotateDirection = input.x;  // 회전 방향을 입력 Y로 지정
        moveDirection = input.y;    // 이동 방향을 입력 X로 지정
    }

    /// <summary>
    /// 플레이어의 이동을 구현한 함수
    /// </summary>
    void Move()
    {
        Vector3 dir = transform.forward * moveDirection + transform.right * rotateDirection;  // 이동방향 계산
        dir.Normalize();  // 이동방향을 정규화 

        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * dir);  // 플레이어의 위치 이동
    }

    /// <summary>
    /// 플레이어의 회전을 구혀한 함수
    /// </summary>
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
