using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public PlayerCharacter PlayerCharacter;
    public Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
        PlayerCharacter = GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacter.MoveDirection.x != 0 || PlayerCharacter.MoveDirection.z != 0)
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    internal void Attack()
    {
        animator.SetLayerWeight(1, 1);
        animator.SetTrigger("attacking");
    }
}
