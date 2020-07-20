using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyPatrolState : StateMachineBehaviour
{
    private BruteEnemyScript m_scriptComp = null;

    private CharacterMovement m_movementComp = null;
    private PatrolArea m_patrolAreaComp = null;

    private float m_movementInput = 0f;     // Input to set each update

    private TouchPerception m_touchComp = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<BruteEnemyScript>();

        m_movementComp = m_scriptComp.movementComponent;
        m_patrolAreaComp = m_scriptComp.patrolArea;

        m_patrolAreaComp = m_scriptComp.patrolArea;
        if (!m_patrolAreaComp)
        {
            Debug.LogWarning("Missing Patrol Area Component! Will be unable to properly function!", this);
            return;
        }

        // First time entering this state, just starting moving in any direction
        if (Mathf.Approximately(m_movementInput, 0f) && m_patrolAreaComp.IsInPatrolArea(animator.transform.position))
            m_movementInput = 1f;
        // Keep following way we were going if we can
        else if (!m_patrolAreaComp.IsInPatrolArea(animator.transform.position))
            m_movementInput = Mathf.Sign((m_patrolAreaComp.position - (Vector2)animator.transform.position).x);

        if (m_touchComp)
            m_touchComp.OnPerceptionUpdated += OnTouchedByObject;

        m_scriptComp.OnEnterPatrol();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Keep moving until we are barely just outside of the patrol area
        if (m_patrolAreaComp && m_patrolAreaComp.HasPassedPatrolArea(animator.transform.position, m_movementInput))
        {
            animator.SetBool("Idle", true);
            return;
        }

        m_movementComp.SetMoveInput(m_movementInput);
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
