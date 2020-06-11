using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyIdleState : StateMachineBehaviour
{
    private BruteEnemyScript m_scriptComp = null;      
    private float m_idleStart = -1f;                    // Time we entered state

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<BruteEnemyScript>();
        m_scriptComp.movementComponent.SetMoveInput(0f);

        m_idleStart = Time.time;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float endTime = m_idleStart + m_scriptComp.idleTime;
        if (Time.time >= endTime)
            animator.SetBool("Idle", false);
    }
}
