using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : MonoBehaviour
{
    public CharacterController controller;
    public float Speed = 10f;
    public float LookSpeed = 10f;
    public Vector3 MoveDirection;
    public Vector3 DashDirection;
    public float DashDistance = 100f;
    public GameObject Aim;
    public GameObject Camera;

    public PlayerAnimator Animator;

    public StatHolder PlayerStats;


    private int JumpCount = 0;



    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        Animator = GetComponent<PlayerAnimator>();

        Controls controls = new Controls();
        controls.Player.Enable();
        controls.Player.Move.performed += Movement;
        controls.Player.Move.canceled += MovementCanceled;
        controls.Player.Jump.performed += Jump;
        controls.Player.Look.performed += Look;
        controls.Player.Dash.performed += Dash;
        controls.Player.Fire.performed += Attack;

        Aim.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        Animator.Attack();
        Physics.BoxCast(transform.position, new Vector3(0.3f, 0.5f, 0.5f), transform.forward, out RaycastHit hit, transform.rotation, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<IEnemy>() != null)
            {
                hit.collider.gameObject.GetComponent<IEnemy>().TakeDamage(PlayerStats.Strength * 33f);
            }
        }
    }

    public void Update()
    {

        //transform the movement vector to the camera's local space so that the player moves in the direction of the camera when moving

        //camera forward and right vectors:
        var forward = Camera.transform.forward;
        var right = Camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * MoveDirection.z + right * MoveDirection.x;
        desiredMoveDirection.y = MoveDirection.y;



        //move player Relative to camera
        controller.Move(TransformMovement(MoveDirection + DashDirection) * Time.deltaTime);
        if (!controller.isGrounded)
            MoveDirection.y -= 20.8f * Time.deltaTime;
        else
            JumpCount = 0;

        DashDirection *= 0.9f;
    }

    public Vector3 TransformMovement(Vector3 movement)
    {
        //transform the movement vector to the camera's local space so that the player moves in the direction of the camera when moving

        //camera forward and right vectors:
        var forward = Camera.transform.forward;
        var right = Camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * movement.z + right * movement.x;
        desiredMoveDirection.y = movement.y;



        //move player Relative to camera
        return desiredMoveDirection;
    }

    public void Movement(InputAction.CallbackContext contex)
    {
        Vector2 move = contex.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        //transform the movement vector to the camera's local space so that the player moves in the direction of the camera when moving


        moveDirection *= Speed;
        MoveDirection.x = moveDirection.x;
        MoveDirection.z = moveDirection.z;


    }
    public void MovementCanceled(InputAction.CallbackContext contex)
    {
        MoveDirection.x = 0;
        MoveDirection.z = 0;
    }
    public void Look(InputAction.CallbackContext contex)
    {
        Vector2 look = contex.ReadValue<Vector2>();
        transform.Rotate(0, look.x * LookSpeed * Time.deltaTime, 0);
        Aim.transform.Rotate(-look.y * LookSpeed * Time.deltaTime, 0, 0);
    }
    public void Jump(InputAction.CallbackContext contex)
    {
        if (controller.isGrounded || JumpCount < 2)
        {
            MoveDirection.y = 8f;
            JumpCount++;
        }
    }
    public void Dash(InputAction.CallbackContext contex)
    {
        if (MoveDirection.x != 0 || MoveDirection.z != 0)
        {
            var movement = MoveDirection;
            movement.y = 0;

            DashDirection = movement.normalized * DashDistance;
        }
    }
}
