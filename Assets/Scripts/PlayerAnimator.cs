using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public PlayerCharacter PlayerCharacter;
    public Animator animator;

    private Vector2 MoveVelocity;

    // Start is called before the first frame update
    private void Awake()
    {
        PlayerCharacter = GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {


        animator.SetFloat("xVelocity", MoveVelocity.x);
        animator.SetFloat("zVelocity", MoveVelocity.y);
        animator.SetBool("grounded", PlayerCharacter.controller.isGrounded);
        var dir = PlayerCharacter.MoveDirection;
        dir.y = 0;
        dir.Normalize();

        MoveVelocity = Vector2.MoveTowards(MoveVelocity, new Vector2(dir.x, dir.z), Time.deltaTime * 5f);
        //Debug.Log(MoveVelocity);
    }

    internal void Attack()
    {
        animator.SetLayerWeight(1, 1);
        animator.SetTrigger("attacking");
    }

    internal void Jump()
    {
        animator.SetTrigger("Jump");
    }

    internal void Dash()
    {
        //animator.SetTrigger("Dash");
    }
}
