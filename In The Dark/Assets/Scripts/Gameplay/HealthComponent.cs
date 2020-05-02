using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A health component. Can be used for more than just health (e.g. armor)
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float m_health = 100f;         // Health of object
    [SerializeField] private float m_maxHealth = 100f;      // Max health of object
    [SerializeField] private bool m_invincible = false;     // If object cannot be damaged (invincible)

    // Event that is called when ever this object loses/restores health
    public delegate void OnHealthChangedEvent(HealthComponent self, float newHealth, float delta);
    public OnHealthChangedEvent onHealthChanged;

    // Event that is called upon death of the object. This will get called after onHealthChanged
    public delegate void OnDeathEvent(HealthComponent self);
    public OnDeathEvent onDeath;

    /// <summary>
    /// If this object is dead (has no health)
    /// </summary>
    public bool isDead { get { return m_health <= 0f; } }

    /// <summary>
    /// If this object is invincible
    /// </summary>
    public bool isInvincible { get { return m_invincible; } set { m_invincible = value; } }

    void Start()
    {
        // Clamp health as it is
        m_health = Mathf.Min(m_health, m_maxHealth);        
    }

    /// <summary>
    /// Restores health to this object. Will do nothing if dead
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    /// <returns>Amount of health restored</returns>
    public float RestoreHealth(float amount)
    {
        if (amount > 0)
            return ApplyHealthDelta(amount);

        return 0f;
    }

    /// <summary>
    /// Applies damage to this object. Will do nothing if dead
    /// </summary>
    /// <param name="amount">Amount of damage to apply</param>
    /// <returns>Amount of damage applied</returns>
    public float ApplyDamage(float amount)
    {
        if (amount > 0)
            return Mathf.Abs(ApplyHealthDelta(-amount));

        return 0f;
    }

    /// <summary>
    /// Applies an amount of health directly. Delta can be negative (if applying negative amount)
    /// </summary>
    /// <param name="amount">Amount of delta</param>
    /// <returns>Delta of change applied</returns>
    public float ApplyDeltaDirect(float amount)
    {
        return ApplyHealthDelta(amount);
    }

    /// <summary>
    /// Applies a delta of health to this object
    /// </summary>
    /// <param name="delta">Delta to apply. Positive to restore, negative to damage</param>
    /// <returns>Delta that was applied to health</returns>
    private float ApplyHealthDelta(float delta)
    {
        if (isDead || Mathf.Approximately(delta, 0f))
            return 0f;

        // Stop here if recieving damage but we are invincible
        if (m_invincible && delta < 0f)
            return 0f;

        float OldHealth = m_health;
        m_health = Mathf.Clamp(OldHealth + delta, 0f, m_maxHealth);

        // Update delta based on how much health has actually changed
        // (Healing will be positive, damage will be negative)
        delta = m_health - OldHealth;

        if (onHealthChanged != null)
            onHealthChanged.Invoke(this, m_health, delta);

        if (isDead)
            if (onDeath != null)
                onDeath.Invoke(this);

        return delta;
    }
}
