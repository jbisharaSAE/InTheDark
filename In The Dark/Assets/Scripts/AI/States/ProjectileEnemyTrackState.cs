using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyTrackState : StateMachineBehaviour
{
    private GuardEnemyScript m_scriptComp = null;
    private EnemyTargetSelector m_targetComp = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<GuardEnemyScript>();
        m_targetComp = animator.GetComponent<EnemyTargetSelector>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.isPaused || JB_DialogueManager.isPlayingDialogue)
            return;

        // We can just exit this state now if target is not in sight,
        // as we most likely won't be moving 
        GameObject target = m_targetComp.target;
        if (!target)
        {
            animator.SetBool("HasTarget", false);
            return;
        }

        float side = target.transform.position.x - animator.transform.position.x;
        animator.transform.eulerAngles = Helpers.FlipRotation(side);

        EnemyProjectileAttack attackComp = m_scriptComp.attackComponent;
        if (attackComp && attackComp.canThrowProjectile)
        {
            animator.SetTrigger("Attack");
            // temp, while waiting on animations
            m_scriptComp.ThrowProjectile();
            return;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset trigger we may have activated
        animator.ResetTrigger("Attack");
    }
}
