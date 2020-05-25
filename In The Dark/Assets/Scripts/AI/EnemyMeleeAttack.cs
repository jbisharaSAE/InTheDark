﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public Vector2 m_offset = Vector2.zero;
    public float m_radius = 0.75f;
    public float m_damage = 20f;
    public LayerMask m_attackLayers = Physics2D.AllLayers;

    private HashSet<GameObject> m_hitObjects = null;

    void Awake()
    {
        // We only need to Update() when our attack is active
        enabled = false;
    }

    void Update()
    {
        AttackImpl();
    }

    /// <summary>
    /// Starts (and initializes) for a new attack phase.
    /// Is possible to call again to reset the attack
    /// </summary>
    public void StartAttack()
    {
        enabled = true;
        m_hitObjects = new HashSet<GameObject>();
    }

    /// <summary>
    /// Stops the current attack in progress.
    /// </summary>
    public void StopAttack()
    {
        enabled = false;
        m_hitObjects = null;
    }

    /// <summary>
    /// Attacks once, this will stop any attack in progress
    /// </summary>
    public void AttackOnce()
    {
        StopAttack();
        AttackImpl();
    }

    /// <summary>
    /// Performs the actual attack by querying the world
    /// </summary>
    private void AttackImpl()
    {
        Vector2 center = GetAttackCenter();

        Collider2D hitCollider = Physics2D.OverlapCircle(center, m_radius, m_attackLayers);
        if (hitCollider)
        {
            // Do not hit ourselves
            GameObject hitObject = hitCollider.gameObject;
            if (hitObject == gameObject)
                return;

            // Check if we should ignore this object
            if (m_hitObjects != null && m_hitObjects.Contains(hitObject))
                return;

            // Try applying damage to it
            HealthComponent healthComp = hitObject.GetComponent<HealthComponent>();
            if (healthComp)
                healthComp.ApplyDamage(m_damage);

            if (m_hitObjects != null)
                m_hitObjects.Add(hitObject);
        }
    }

    /// <summary>
    /// Helper for getting the center of attack in world space
    /// </summary>
    /// <returns>World position of center</returns>
    private Vector2 GetAttackCenter()
    {
        return transform.TransformPoint(m_offset.x, m_offset.y, 0f);
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.TransformPoint(m_offset);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, m_radius);
    }
    #endregion
}