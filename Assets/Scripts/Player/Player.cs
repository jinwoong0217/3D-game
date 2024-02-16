using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;

    float moveDirection = 0.0f;

    public float moveSpeed = 2.0f;

    float rotateDirection = 0.0f;

    public float rotateSpeed = 90.0f;

    public Transform firePosition;
    public Transform crosshair;
    private Transform playerTransform;

    public GameObject muzzleFlashPrefab;

    private void Awake()
    {
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //GameObject rifle = transform.GetChild(7).gameObject;
        //firePosition = rifle.transform.GetChild(0);
        //firePosition = GameObject.Find("Fire");

        playerTransform = transform;

        Cursor.lockState = CursorLockMode.Locked;
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

    private void MouseMove(InputAction.CallbackContext context)
    {
        //SetInput(context.ReadValue<Vector2>(), !context.canceled); 비슷하게
    }

    private void OnShootingInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shooting();
        }
    }

    private void Shooting()
    {
        RaycastHit hit;
        Ray ray = new Ray(firePosition.position, firePosition.forward); 

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject); 
            }
        }

        Instantiate(muzzleFlashPrefab, firePosition.position, firePosition.rotation);
    }



    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
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

    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
    }

    void Move()
    {
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);     
    }

    void Rotate()
    {
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        rigid.MoveRotation(rigid.rotation * rotate);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePosition.position, firePosition.forward * 50f); 
    }
}
