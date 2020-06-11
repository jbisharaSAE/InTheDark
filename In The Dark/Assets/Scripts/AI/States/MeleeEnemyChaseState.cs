﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyChaseState : StateMachineBehaviour
{
    private BruteEnemyScript m_scriptComp = null;

    private CharacterMovement m_movementComp = null;
    private PatrolArea m_patrolAreaComp = null;
    private EnemyTargetSelector m_selectorComp = null;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<BruteEnemyScript>();

        m_movementComp = m_scriptComp.movementComponent;
        m_patrolAreaComp = m_scriptComp.patrolArea;

        m_selectorComp = animator.GetComponent<EnemyTargetSelector>();

        m_scriptComp.OnEnterChase();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject target = m_selectorComp.target;
        if (!target)
        {
            // Shouldn't be called by us ultimately, but as a fallback
            animator.SetBool("HasTarget", false);
            return;
        }

        // Try attacking if in range
        Vector2 displacement = target.transform.position - animator.transform.position;
        if (displacement.sqrMagnitude <= (m_scriptComp.attackRange * m_scriptComp.attackRange))
        {
            animator.SetTrigger("Attack");
            return;
        }
        // Only continue chasing if we are inside the patrol area
        else if (m_patrolAreaComp.IsInChaseArea(animator.transform.position))
        {
            // If target is far to left or right us, move forward. If close (might be above
            // or below us) we just want to wait for them to either reach us
            if (Mathf.Abs(displacement.x) > (m_scriptComp.attackRange * 0.25f))
            {
                m_movementComp.SetMoveInput(Mathf.Sign(displacement.x));
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Idle", true);
            }
        }
        else
        {
            animator.SetBool("Idle", true);
            m_movementComp.SetMoveInput(0f);
        }      

        // Face our target
        float desiredRot = displacement.x > 0f ? 0f : 180f;
        if (animator.transform.eulerAngles.y != desiredRot)
            animator.transform.eulerAngles = new Vector3(0f, desiredRot, 0f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset trigger we may have activated
        animator.ResetTrigger("Attack");
    }
}
