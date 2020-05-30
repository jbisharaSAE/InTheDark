using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyTrack : StateMachineBehaviour
{
    private GuardEnemyScript m_scriptComp = null;
    private EnemyTargetSelector m_targetComp = null;

    // for now, handling shoot here
    float lastShotTime = float.MaxValue;
    public ProjectileComponent projectilePrefab;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_scriptComp = animator.GetComponent<GuardEnemyScript>();
        m_targetComp = animator.GetComponent<EnemyTargetSelector>();

        lastShotTime = Time.time;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_targetComp.ConditionalGetTarget())
        {
            animator.SetBool("HasTarget", false);
            return;
        }

        // TODO: Testing. Would call a function in guard selector where we pass in,
        // doing this now cause I want to go to bed and I'm tired
        SightPerception perception = animator.GetComponentInChildren<SightPerception>();
        Vector3 ang = perception.transform.eulerAngles;
        Vector2 dis = m_targetComp.target.transform.position - perception.transform.position;
        dis.Normalize();
        ang.z = Mathf.Rad2Deg * Mathf.Atan2(dis.y, dis.x);
        perception.transform.eulerAngles = ang;

        if (Time.time > (lastShotTime + 2f))
        {
            ProjectileComponent proj = ProjectileComponent.SpawnProjectile(projectilePrefab, (Vector2)perception.transform.position + dis * 0.5f, dis);
            proj.instigator = animator.gameObject;
            proj.ignoreInstigator = true;

            lastShotTime = Time.time;
            //animator.SetTrigger("Attack");
            return;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset trigger we may have activated
        animator.ResetTrigger("Attack");
    }
}
