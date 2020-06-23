using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple pickup that will restore health to the player
/// </summary>
public class HealthPickup : PickupComponent
{
    public float m_healthToGive = 20f;      // Amount of health to give. Can be negative to apply damage

    protected override void OnPickedUp(Collider2D collision)
    {
        // Callback can happen multiple times if overlapping multiple
        // colliders at once. We disable ourselves if that happens
        if (!isActiveAndEnabled)
            return;

        HealthComponent healthComp = collision.GetComponent<HealthComponent>();
        if (!healthComp)
            return;

        // Only destroy the pickup if we have actually applied the health delta
        float delta = healthComp.RestoreHealth(m_healthToGive);
        if (Mathf.Abs(delta) > 0f)
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}
