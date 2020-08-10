using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level specific script for breaking branch after the enemy dies
/// </summary>
public class BreakBranch : MonoBehaviour
{
    [SerializeField] private EnemyScript m_enemyScript;
    [SerializeField] private AudioClip m_breakSound = null;

    void Start()
    {
        if (m_enemyScript && m_enemyScript.healthComponent)
            m_enemyScript.healthComponent.OnDeath += OnEnemyDeath;
    }

    void OnDestroy()
    {
        if (m_enemyScript && m_enemyScript.healthComponent)
            m_enemyScript.healthComponent.OnDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath(HealthComponent self)
    {
        SingleSound.PlaySingleSound(m_breakSound);

        Destroy(gameObject);
    }
}
