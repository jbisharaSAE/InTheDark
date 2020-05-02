using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that will try to apply health to any object that overlaps it.
/// This component only works with trigger colliders
/// </summary>
public class HealthPickup : MonoBehaviour
{
    public float m_healthToGive = 20f;      // Amount of health to give. Can be negative to apply damage

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Callback can happen multiple times if overlapping multiple
        // colliders at once. We disable ourselves if that happens
        if (!isActiveAndEnabled)
            return;

        HealthComponent healthComp = collision.GetComponent<HealthComponent>();
        if (!healthComp)
            return;

        // Only destroy the pickup if we have actually applied the health delta
        float delta = healthComp.ApplyDeltaDirect(m_healthToGive);
        if (Mathf.Abs(delta) > 0f)
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}
