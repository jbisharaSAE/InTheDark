using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base for an enemy. This script acts as the 'Core' of an enemy
/// </summary>
public class EnemyScript : MonoBehaviour
{
    [SerializeField] private PickupComponent m_deathDrop = null;            // Pickup to spawn upon death
    [SerializeField, Range(0f, 1f)] private float m_dropChance = 0.1f;      // Chance of dropping pick up upon death

    [Header("Components", order = 0)]
    [SerializeField] private CharacterMovement m_movementComp = null;       // Enemies movement component
    [SerializeField] private Animator m_animatorComp = null;                // Enemies animator component
    [SerializeField] private HealthComponent m_healthComp = null;           // Enemies health component

    /// <summary>
    /// This enemies character movement component
    /// </summary>
    public CharacterMovement movementComponent { get { return m_movementComp; } }

    /// <summary>
    /// This enemies animator component
    /// </summary>
    public Animator animatorComponent { get { return m_animatorComp; } }

    /// <summary>
    /// This enemies health component
    /// </summary>
    public HealthComponent healthComponent { get { return m_healthComp; } }

    protected virtual void Awake()
    {
        if (!m_movementComp)
            m_movementComp = GetComponent<CharacterMovement>();

        if (!m_animatorComp)
            m_animatorComp = GetComponent<Animator>();

        if (!m_healthComp)
            m_healthComp = GetComponent<HealthComponent>();

        if (m_healthComp)
        {
            m_healthComp.OnHealthChanged += OnHealthChanged;
            m_healthComp.OnDeath += OnDeath;
        }
    }

    protected virtual void Update()
    {
        animatorComponent.SetBool("Jumping", m_movementComp.isJumping);
        animatorComponent.SetBool("Airborne", !m_movementComp.isGrounded);
    }

    protected virtual void OnHealthChanged(HealthComponent self, float newHealth, float delta)
    {
        if (newHealth > 0f)
        {

        }
    }

    protected virtual void OnDeath(HealthComponent self)
    {
        TryDroppingPickup();
        Destroy(gameObject);
    }

    /// <summary>
    /// Attempts to drop a pickup right where we are
    /// </summary>
    private void TryDroppingPickup()
    {
        if (!m_deathDrop)
            return;

        // Test our luck
        float failChance = 1f - m_dropChance;
        if (failChance * 10f < Random.Range(0f, 100f))
            return;

        // Spawn the pickup right where we are
        Instantiate(m_deathDrop, transform.position, Quaternion.identity);
    }
}
