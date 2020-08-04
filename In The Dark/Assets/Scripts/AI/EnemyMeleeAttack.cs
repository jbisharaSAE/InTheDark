using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public Vector2 m_offset = Vector2.zero;
    public float m_radius = 0.75f;
    public DamageInfo m_damage;
    public LayerMask m_attackLayers = Physics2D.AllLayers;
    [Min(0f)] public float m_cooldown = 0.75f;

    private HashSet<GameObject> m_hitObjects = null;
    private float m_cooldownStart = -1f;

    public bool isInCooldown { get { return m_cooldownStart >= 0f && Time.time < m_cooldownStart + m_cooldown; } }

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

        m_cooldownStart = Time.time;
    }

    /// <summary>
    /// Attacks once, this will stop any attack in progress
    /// </summary>
    public void AttackOnce()
    {
        StopAttack();

        m_hitObjects = new HashSet<GameObject>();
        AttackImpl();
        m_hitObjects = null;

        m_cooldownStart = Time.time;
    }

    /// <summary>
    /// Performs the actual attack by querying the world
    /// </summary>
    private void AttackImpl()
    {
        Vector2 center = GetAttackCenter();

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, m_radius, m_attackLayers);
        if (hits != null && hits.Length > 0)
            foreach (Collider2D hitCollider in hits)
            {
                // Do not hit ourselves
                GameObject hitObject = hitCollider.gameObject;
                if (hitObject == gameObject)
                    continue;

                // Check if we should ignore this object
                if (m_hitObjects != null && m_hitObjects.Contains(hitObject))
                    continue;

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
