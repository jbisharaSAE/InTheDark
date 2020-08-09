using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEnemyScript : EnemyScript
{
    [SerializeField] private GuardLookoutRoutine m_lookoutComp = null;      // Guards routine component
    [SerializeField] private EnemyProjectileAttack m_attackComp = null;     // Guards attack component

    private SightPerception m_sightPerception = null;           // The guards sight perception

    /// <summary>
    /// This guards lookout routine component
    /// </summary>
    public GuardLookoutRoutine lookoutComponent { get { return m_lookoutComp; } }

    /// <summary>
    /// This guards projectile attack component
    /// </summary>
    public EnemyProjectileAttack attackComponent { get { return m_attackComp; } }

    /// <summary>
    /// The direction this guard is looking
    /// </summary>
    public Vector2 eyeSightDirection { get { return GetEyeSightDirection(); } }

    protected override void Awake()
    {
        base.Awake();

        if (!m_lookoutComp)
            m_lookoutComp = GetComponent<GuardLookoutRoutine>();

        if (!m_attackComp)
            m_attackComp = GetComponent<EnemyProjectileAttack>();

        m_sightPerception = GetComponentInChildren<SightPerception>();
    }

    public void ThrowProjectile()
    {
        if (m_attackComp)
            m_attackComp.TryThrowProjectile(GetDirectionToTarget());
    }

    /// <summary>
    /// Get the direction the guard is currently looking
    /// </summary>
    /// <returns>Normalized vector</returns>
    private Vector2 GetEyeSightDirection()
    {
        if (m_sightPerception)
            return m_sightPerception.transform.right;
        else
            return transform.right;
    }

    private Vector2 GetDirectionToTarget()
    {
        EnemyTargetSelector selector = GetComponent<EnemyTargetSelector>();
        if (selector && selector.target)
            return (selector.target.transform.position - transform.position).normalized;

        return Vector2.right;
    }
}
