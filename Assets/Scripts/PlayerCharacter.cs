using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : MonoBehaviour, IEnemy
{
    public CharacterController controller;
    public float Speed = 10f;
    public float LookSpeed = 10f;
    public Vector3 MoveDirection;
    public Vector3 DashDirection;
    public float DashDistance = 10f;
    public GameObject Aim;
    public GameObject Camera;

    public PlayerAnimator Animator;

    public StatHolder PlayerStats => GameManager.Instance.PlayerStats;

    private int JumpCount = 0;

    private TimeSince LastAttack = default;
    private TimeSince LastDash = default;

    Controls controls;


    public Vector3 PlayerSpawnPos;
    public Quaternion PlayerSpawnRot;



    public bool IsActive { get; set; } = false;



    void Awake()
    {
        PlayerSpawnPos = transform.position;
        PlayerSpawnRot = transform.rotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        Animator = GetComponent<PlayerAnimator>();

        controls = new Controls();
        controls.Player.Enable();
        controls.Player.Jump.performed += Jump;
        controls.Player.Dash.performed += Dash;
        controls.Player.Fire.performed += Attack;
        controls.Player.CloseGame.performed += CloseGame;

        Aim.transform.localRotation = Quaternion.Euler(0, 0, 0);
        LockPlayer();
    }

    public static void CloseGame(InputAction.CallbackContext obj)
    {
        GameManager.Instance.PlayerStats.SavePlayerStats();
        Application.Quit();
    }
    private void OnDestroy()
    {
        controls.Player.Disable();
        controls.Player.Jump.performed -= Jump;
        controls.Player.Dash.performed -= Dash;
        controls.Player.Fire.performed -= Attack;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        if (LastAttack < 0.4f) return;
        DashDirection = Vector3.zero;
        DashDirection = Vector3.forward * DashDistance * 0.05f;
        Animator.Attack();
        Physics.BoxCast(transform.position, new Vector3(0.3f, 0.5f, 0.5f), transform.forward, out RaycastHit hit, transform.rotation, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<IEnemy>() != null)
            {
                hit.collider.gameObject.GetComponent<IEnemy>().TakeDamage(PlayerStats.Strength, transform.forward);
            }
        }

        LastAttack = 0;

    }

    public void Update()
    {
        if (controls == null) return;
        Move();
        Look();

        //move player Relative to camera
        controller.Move(TransformMovement(MoveDirection + DashDirection) * Time.deltaTime);
        if (!controller.isGrounded)
            MoveDirection.y -= 20.8f * Time.deltaTime;
        else
            JumpCount = 0;

        DashDirection -= DashDirection * 4f * Time.deltaTime;

        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f, ~LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponentInParent<Room>() is Room r)
            {
                GameManager.Instance.MainGameUI.MiniMap.SetRoom(r);
                return;
            }
            if (hit.collider.gameObject.GetComponent<Room>() is Room s)
            {
                GameManager.Instance.MainGameUI.MiniMap.SetRoom(s);
                return;
            }
            if (hit.collider.gameObject.GetComponent<Room>() is Room t)
            {
                GameManager.Instance.MainGameUI.MiniMap.SetRoom(t);
                return;
            }
        }

    }

    private void Move()
    {
        Vector2 move = controls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        moveDirection *= Speed;
        MoveDirection.x = moveDirection.x;
        MoveDirection.z = moveDirection.z;
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
    public void Look()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();
        transform.Rotate(0, look.x * LookSpeed * Time.deltaTime, 0);

        Aim.transform.localEulerAngles += new Vector3(-look.y * LookSpeed * Time.deltaTime, 0, 0);

        //Clamp the rotation to 80 degrees up and 280 down with roll over
        if (Aim.transform.localEulerAngles.x > 80 && Aim.transform.localEulerAngles.x < 288)
        {
            Aim.transform.localEulerAngles = new Vector3(Aim.transform.localEulerAngles.x - (-look.y * LookSpeed * Time.deltaTime), 0, 0);
        }

        //Aim.transform.localEulerAngles = new Vector3(Mathf.Clamp(Aim.transform.localEulerAngles.x, -80, 80), 0, 0);
    }
    public void Jump(InputAction.CallbackContext contex)
    {
        if (controller.isGrounded && JumpCount < 2)
        {
            Animator.Jump();
            MoveDirection.y = 8f;
            JumpCount++;
        }
        else if (!controller.isGrounded && JumpCount < 2)
        {
            Animator.Jump();
            MoveDirection.y = 8f;
            JumpCount += 2;
        }
    }
    public void Dash(InputAction.CallbackContext contex)
    {
        if (LastDash < 1f) return;
        DashDirection = Vector3.zero;
        if (MoveDirection.x != 0 || MoveDirection.z != 0)
        {
            var movement = MoveDirection;
            movement.y = 0;

            DashDirection = movement.normalized * DashDistance * 0.2f;
        }
        else
        {
            DashDirection = Vector3.forward * DashDistance * 0.2f;
        }
        Animator.Dash();
        LastDash = 0;

    }

    public void TakeDamage(float damage, Vector3 Direction = default)
    {
        PlayerStats.Health -= damage;
        if (PlayerStats.Health <= 0)
        {
            PlayerStats.Health = 0;
            GameManager.Instance.GameOver();
        }
    }

    internal void LockPlayer()
    {
        controls.Disable();
    }

    internal void UnlockPlayer()
    {
        controls.Enable();
    }
}
