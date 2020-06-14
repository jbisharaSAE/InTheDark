﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossRun : StateMachineBehaviour
{
    
    public float attackRange = 3f;

    private float speed;
    private Transform player;
    private Rigidbody2D rb;
    private JB_BossOne bossScript;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        bossScript = animator.GetComponent<JB_BossOne>();
        speed = animator.GetComponent<JB_BossOne>().moveSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.LookAtPlayer();

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        

        float distance = Vector2.Distance(player.position, rb.position);

        if (distance <= attackRange)
        {
            // attack
            animator.SetTrigger("Attack");
        }
        else
        {
            // move towards player
            rb.MovePosition(newPos);

        }

        if (distance > 10f)
        {
            // run throw script if far away enough from player
            bossScript.ThrowShuriken();
        }

    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Throw");

        
    }

}