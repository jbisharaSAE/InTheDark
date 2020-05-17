using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mod;

// TODO: Update to be StateMachineBehavior
public class SimplePatrolState : IStateComponent
{
    public Transform m_patrolAreaOrigin;                            // Origin of patrol area, is center for Area Size
    public Vector2 m_patrolAreaSize = new Vector2(10f, 1f);         // Size of patrol area (Y isn't used for now)

    private CharacterMovement m_movementComp = null;            // AIs movement component
    private Vector2 m_cachedAreaOrigin = Vector2.zero;          // If no patrol origin is specified, we fall back to this
    private float m_movementInput = 0f;                         // Input for movement. +1 means right, -1 means left

    protected override void OnInitializedWithMachine()
    {
        m_movementComp = machineOwner.GetComponent<CharacterMovement>();
        m_cachedAreaOrigin = machineOwner.transform.position;
    }

    protected override void OnEnterState(IStateComponent previousState)
    {
        // TODO: Check which way we should head, for now
        m_movementInput = 1f;
    }

    protected override void OnExitState(IStateComponent nextState)
    {
        
    }

    void Update()
    {
        Transform ourTransform = m_movementComp.transform;

        // TODO: Check if enemy is in sight, if so, switch to attack state

        // TODO: Check if we are about to walk over a ledge or into a wall. Based on that, we want to jump or go back the other way

        {
            Vector2 areaOrigin = GetPatrolAreaCenter();

            // We do origin - position as we could immediately use the sign bit to get the correct movement direction
            float distanceFromPatrolArea = areaOrigin.x - ourTransform.position.x;

            if (Mathf.Abs(distanceFromPatrolArea) >= m_patrolAreaSize.x)
                m_movementInput = Mathf.Sign(distanceFromPatrolArea);
        }

        m_movementComp.SetHorizontalInput(m_movementInput);
    }

    /// <summary>
    /// Get the origin of the patrol area. Handles null check of transform component
    /// </summary>
    /// <returns>Origin of patrol area in world space</returns>
    private Vector2 GetPatrolAreaCenter()
    {
        return m_patrolAreaOrigin ? (Vector2)m_patrolAreaOrigin.position : m_cachedAreaOrigin;
    }

    void OnDrawGizmosSelected()
    {
        if (!m_patrolAreaOrigin)
            return;

        Gizmos.color = new Color(0.3f, 1f, 0f);
        Gizmos.DrawWireCube(m_patrolAreaOrigin.position, new Vector3(m_patrolAreaSize.x, m_patrolAreaSize.y, 0.1f) * 2f);
    }
}
