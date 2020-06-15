using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyWatchState : StateMachineBehaviour
{
    private GuardEnemyScript m_scriptComp = null;
    private GuardLookoutRoutine m_lookoutComp = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<GuardEnemyScript>();

        m_lookoutComp = animator.GetComponent<GuardLookoutRoutine>();
        if (m_lookoutComp)
            // Resume the lookout routine from the entry closest to way guard is already looking
            m_lookoutComp.ResumeRoutineClosestTo(m_scriptComp.eyeSightDirection);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_lookoutComp)
            m_lookoutComp.PauseRoutine();
    }
}
