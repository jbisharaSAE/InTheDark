using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyChaseState : StateMachineBehaviour
{
    [SerializeField] private float m_attackRange = 1.5f;        // Range target must be within to attack

    private CharacterMovement m_movementComp = null;
    private EnemyTargetSelector m_brain = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_movementComp = animator.GetComponent<CharacterMovement>();
        m_brain = animator.GetComponent<EnemyTargetSelector>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject target = m_brain.target;
        if (!target)
        {
            // Shouldn't be called by us ultimately, but as a fallback
            animator.SetBool("HasTarget", false);
            return;
        }

        // Try attacking if in range
        Vector2 displacement = target.transform.position - animator.transform.position;
        if (displacement.sqrMagnitude <= (m_attackRange * m_attackRange))
        {
            animator.SetTrigger("Attack");
            return;
        }
        // If target is far to left or right us, move forward. If close (might be above
        // or below us) we just want to wait for them to either reach us
        if (Mathf.Abs(displacement.x) > m_attackRange)
        {
            m_movementComp.SetHorizontalInput(Mathf.Sign(displacement.x));
        }

        // Face our target
        float desiredRot = displacement.x > 0f ? 0f : 180f;
        if (animator.transform.eulerAngles.y != desiredRot)
            animator.transform.eulerAngles = new Vector3(0f, desiredRot, 0f);
    }
}
