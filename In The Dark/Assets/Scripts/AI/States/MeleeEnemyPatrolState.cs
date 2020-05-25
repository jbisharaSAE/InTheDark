using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyPatrolState : StateMachineBehaviour
{
    private CharacterMovement m_movementComp = null;
    private PatrolArea m_patrolAreaComp = null;
    private TouchPerception m_touchComp = null;
    private float m_movementInput = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_movementComp = animator.GetComponent<CharacterMovement>();
        m_patrolAreaComp = animator.GetComponent<PatrolArea>();
        m_touchComp = animator.GetComponent<TouchPerception>();

        if (!m_patrolAreaComp)
        {
            Debug.LogError("Missing Patrol Area Component! Will be unable to properly function!", this);
            return;
        }

        // First time entering this state, just starting moving in any direction
        if (Mathf.Approximately(m_movementInput, 0f) && m_patrolAreaComp.IsInArea(animator.transform.position))
            m_movementInput = 1f;
        // Keep following way we were going if we can
        else if (!m_patrolAreaComp.IsInArea(animator.transform.position))
            m_movementInput = Mathf.Sign((m_patrolAreaComp.position - (Vector2)animator.transform.position).x);

        if (m_touchComp)
            m_touchComp.OnPerceptionUpdated += OnTouchedByObject;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Keep moving until we are barely just outside of the patrol area
        if (m_patrolAreaComp && m_patrolAreaComp.HasPassedArea(animator.transform.position, m_movementInput))
        {
            animator.SetBool("Idle", true);
            return;
        }

        m_movementComp.SetHorizontalInput(m_movementInput);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We only want this event called while we are active
        if (m_touchComp)
            m_touchComp.OnPerceptionUpdated -= OnTouchedByObject;
    }

    private void OnTouchedByObject(GameObject detectedObject, float side)
    {
        m_movementInput = side;
    }
}
