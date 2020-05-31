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

    protected override void Awake()
    {
        if (!m_lookoutComp)
            m_lookoutComp = GetComponent<GuardLookoutRoutine>();

        if (!m_attackComp)
            m_attackComp = GetComponent<EnemyProjectileAttack>();

        m_sightPerception = GetComponentInChildren<SightPerception>();
    }

    public void ThrowProjectile()
    {
        if (m_attackComp)
            m_attackComp.TryThrowProjectile(GetThrowDirection());
    }

    private Vector2 GetThrowDirection()
    {
        if (m_sightPerception)
            return m_sightPerception.transform.right;
        else
            return transform.right;
    }
}
