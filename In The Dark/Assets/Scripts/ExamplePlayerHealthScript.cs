using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePlayerHealthScript : MonoBehaviour
{
    public HealthComponent m_healthComp = null;
    public UnityEngine.UI.Text m_text = null;

    public float m_interpolateSpeed = 5f;
    private float m_interpolatedHealth = 0f;

    void Start()
    {
        if (m_healthComp)
        {
            m_interpolatedHealth = m_healthComp.health;
            m_healthComp.OnHealthChanged += OnHealthChanged;
        }

        if (m_text)
            m_text.text = string.Format("Health: {0}", Mathf.FloorToInt(m_interpolatedHealth));
    }

    void Update()
    {
        if (!m_healthComp)
            return;

        m_interpolatedHealth = Mathf.Lerp(m_interpolatedHealth, m_healthComp.health, m_interpolateSpeed * Time.deltaTime);
        // Disable ourselves once close to actual health value
        if (Mathf.Approximately(m_interpolatedHealth, m_healthComp.health))
            enabled = false;

        if (m_text)
            m_text.text = string.Format("Health: {0}", Mathf.FloorToInt(m_interpolatedHealth));
    }

    void OnHealthChanged(HealthComponent self, float newHealth, float delta)
    {
        enabled = true;
    }
}
