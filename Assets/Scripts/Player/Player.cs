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

    public float moveSpeed = 5.0f;

    float rotateDirection = 0.0f;

    public float rotateSpeed = 180.0f;

    readonly int IsMoveHash = Animator.StringToHash("IsMove");

    public PoolObjectType bulletType = PoolObjectType.Bullet;
    protected Transform fireTransform;
    public GameObject bulletPrefab;

    private void Awake()
    {
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Shooting.performed += OnShootingInput;
        inputActions.Player.Shooting.canceled += OnShootingInput;
        inputActions.Player.Zoom.performed += OnZoomInput;
        inputActions.Player.Zoom.canceled += OnZoomInput;
    }

    private void OnDisable()
    {
        inputActions.Player.Zoom.canceled -= OnZoomInput;
        inputActions.Player.Zoom.performed -= OnZoomInput;
        inputActions.Player.Shooting.canceled -= OnShootingInput;
        inputActions.Player.Shooting.performed -= OnShootingInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }

    private void OnZoomInput(InputAction.CallbackContext context)
    {
    }


    private void OnShootingInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (bulletPrefab != null)
            {       
                GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, fireTransform.rotation);
                bullet.GetComponent<Bullet>().Fire(); 
            }
        }

        else if (context.canceled)
        {
            if (bulletPrefab != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, fireTransform.rotation);
                bullet.GetComponent<Bullet>().StopFiring();
            }
        }
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
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

        animator.SetBool(IsMoveHash, isMove);
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
}
