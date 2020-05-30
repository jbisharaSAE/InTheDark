using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyWatch : StateMachineBehaviour
{
    private GuardLookoutRoutine m_lookoutComp = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_lookoutComp = animator.GetComponent<GuardLookoutRoutine>();
        if (m_lookoutComp)
            // TODO: Resume routine starting from entry that looks closest to
            // direction the enemy is currently facing!
            m_lookoutComp.ResumeRoutine();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_lookoutComp)
            m_lookoutComp.PauseRoutine();
    }
}
