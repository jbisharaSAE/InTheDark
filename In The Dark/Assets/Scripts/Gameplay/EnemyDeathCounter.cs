using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDeathCounter : MonoBehaviour
{
    // Number of enemies to listen to before trigger counter complete event
    [SerializeField] private int m_numToListenTo = 2;

    // If specified, we will only listen for the deaths of these enemies
    [SerializeField] private List<EnemyScript> m_specificEnemies = new List<EnemyScript>();

    // If to automatically destroy the game object upon completion
    [SerializeField] private bool m_autoDestroy = true;

    // Event is called when counter is complete
    [SerializeField] private UnityEvent m_onCounterComplete;

    private int m_remainingCount = -1;

    void OnEnable()
    {
        EnemyScript.onEnemyDeath += OnEnemyDeath;
        m_remainingCount = m_numToListenTo;
    }

    void OnDisable()
    {
        m_remainingCount = -1;
        EnemyScript.onEnemyDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemyScript enemy)
    {
        if (!ShouldCountEnemy(enemy))
            return;

        --m_remainingCount;
        if (m_remainingCount == 0)
        {
            m_onCounterComplete.Invoke();

            if (m_autoDestroy)
                Destroy(gameObject);
            else
                enabled = false;
        }
    }

    private bool ShouldCountEnemy(EnemyScript enemy)
    {
        if (m_specificEnemies != null)
            if (m_specificEnemies.Count > 0 && !m_specificEnemies.Contains(enemy))
                return false;

        return true;
    }
}
