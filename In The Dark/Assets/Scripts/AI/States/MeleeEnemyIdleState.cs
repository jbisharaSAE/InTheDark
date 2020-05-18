using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyIdleState : StateMachineBehaviour
{
    [Min(0f)] public float m_idleTime = 2f;     // How long to remain idle for

    private float m_idleStart = -1f;        // Time we entered state

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_idleStart = Time.time;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float endTime = m_idleStart + m_idleTime;
        if (Time.time >= endTime)
            animator.SetBool("Idle", false);
    }
}
