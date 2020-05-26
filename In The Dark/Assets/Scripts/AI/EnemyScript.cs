using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base for an enemy. This script acts as the 'Core' of an enemy
/// </summary>
public class EnemyScript : MonoBehaviour
{
    private HealthComponent m_healthComp = null;        // Enemies health component

    void Awake()
    {
        m_healthComp = GetComponent<HealthComponent>();
        if (m_healthComp)
        {
            m_healthComp.OnHealthChanged += OnHealthChanged;
            m_healthComp.OnDeath += OnDeath;
        }
    }

    protected virtual void OnHealthChanged(HealthComponent self, float newHealth, float delta)
    {
        if (newHealth > 0f)
        {

        }
    }

    protected virtual void OnDeath(HealthComponent self)
    {
        Destroy(gameObject);
    }
}
