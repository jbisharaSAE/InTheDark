using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteEnemyScript : EnemyScript
{
    [Header("Components", order = 0)]
    [SerializeField] private PatrolArea m_patrolArea = null;    // Area this enemy must stay within

    [Header("Brute Attributes")]
    [SerializeField] private float m_idleTime = 2f;         // How long to remain idle for
    [SerializeField] private float m_patrolSpeed = 4f;      // Speed of this enemy when patrolling
    [SerializeField] private float m_chaseSpeed = 8f;       // Speed of this enemy when chasing
    [SerializeField] private float m_attackRange = 5f;      // Attack range target must be within // TODO: Move to EnemyMeleeAttack?

    private byte m_inJumpArea = 0;

    /// <summary>
    /// This brutess patrol area/chase limits
    /// </summary>
    public PatrolArea patrolArea { get { return m_patrolArea; } }

    public bool inJumpSpot { get { return m_inJumpArea > 0; } }

    public float idleTime { get { return m_idleTime; } }
    public float attackRange { get { return m_attackRange; } }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AIJumpSpot>())
            ++m_inJumpArea;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<AIJumpSpot>())
            --m_inJumpArea;
    }

    public void OnEnterPatrol()
    {
        if (movementComponent)
        {
            movementComponent.m_orientateToMovement = true;
            movementComponent.m_walkSpeed = m_patrolSpeed;
        }
    }

    public void OnEnterChase()
    {
        if (movementComponent)
        {
            movementComponent.m_orientateToMovement = false;
            movementComponent.m_walkSpeed = m_chaseSpeed;
        }
    }
}
