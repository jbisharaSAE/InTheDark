using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mod;

// TODO: Update to be StateMachineBehavior
public class SimpleChaseState : IStateComponent
{
    public float m_attackRange = 0.6f;              // Range of attack, once in range of target we switch to attack state

    public string m_attackState = "Attack";         // Name of state to enter when attacking

    private CharacterMovement m_movementComp = null;        // AIs movement component
    private GameObject m_chaseTarget = null;                // Target to chase

    protected override void OnInitializedWithMachine()
    {
        m_movementComp = machineOwner.GetComponent<CharacterMovement>();
    }

    protected override void OnEnterState(IStateComponent previousState)
    {
        MeleeEnemy enemyAI = machineOwner.GetComponent<MeleeEnemy>();
        m_chaseTarget = enemyAI.targetEnemy;
    }

    void Update()
    {
        if (!m_chaseTarget)
            return;

        Transform ourTransform = m_movementComp.transform;

        Vector2 displacement = m_chaseTarget.transform.position - ourTransform.position;
        if (displacement.sqrMagnitude < (m_attackRange * m_attackRange))
        {
            stateMachine.EnterState(m_attackState);
            return;
        }
        else if (Mathf.Abs(displacement.x) < m_attackRange)
        {
            // Enemy is above or below us
            return;
        }

        // Determine which way to move based on which side target is 
        if (displacement.x > 0f)
            m_movementComp.SetHorizontalInput(1f);
        else
            m_movementComp.SetHorizontalInput(-1f);

    }
}
