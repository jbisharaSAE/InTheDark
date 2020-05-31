using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileAttack : MonoBehaviour
{
    [SerializeField] private ProjectileComponent m_projectilePrefab = null;
    [SerializeField] private float m_rechargeTime = 1f;
    [SerializeField] private Vector2 m_offset = Vector2.zero;

    private float m_lastThrowTime = float.MinValue;

    /// <summary>
    /// If enemy can throw a projectile now
    /// </summary>
    public bool canThrowProjectile { get { return Time.time >= m_lastThrowTime + m_rechargeTime; } }

    /// <summary>
    /// Tries to throw a projectile. Can fail if not allowed to
    /// </summary>
    /// <param name="direction">Direction to throw projectile in</param>
    /// <returns>Projectile or null</returns>
    public ProjectileComponent TryThrowProjectile(Vector2 direction)
    {
        if (!canThrowProjectile)
            return null;

        ProjectileComponent projectile = SpawnProjectile(transform.TransformPoint(m_offset), direction);
        if (!projectile)
            return null;

        m_lastThrowTime = Time.time;
        return projectile;
    }

    /// <summary>
    /// Spawns a projectile, setting ourselves as instigator
    /// </summary>
    /// <param name="position">Position to spawn at</param>
    /// <param name="direction">Direction of projectile</param>
    /// <returns>Projectile or null</returns>
    private ProjectileComponent SpawnProjectile(Vector2 position, Vector2 direction)
    {
        ProjectileComponent projectile = ProjectileComponent.SpawnProjectile(m_projectilePrefab, position, direction);
        if (!projectile)
            return null;

        projectile.instigator = gameObject;
        projectile.ignoreInstigator = true;
        return projectile;
    }

    #region Debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.TransformPoint(m_offset), 0.25f);
    }
    #endregion
}
