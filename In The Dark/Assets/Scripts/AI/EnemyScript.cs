using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base for an enemy. This script acts as the 'Core' of an enemy
/// </summary>
public class EnemyScript : MonoBehaviour
{
    /// <summary>
    /// Event that is called when an enemy dies
    /// </summary>
    /// <param name="enemy">Enemy that died</param>
    public delegate void OnEnemyDeath(EnemyScript enemy);
    public static OnEnemyDeath onEnemyDeath;

    [SerializeField] private bool m_canBeStunned = true;                    // If this enemy can be stunned while damaged
    [SerializeField] private PickupComponent m_deathDrop = null;            // Pickup to spawn upon death
    [SerializeField, Range(0f, 1f)] private float m_dropChance = 0.1f;      // Chance of dropping pick up upon death

    [Header("Components", order = 0)]
    [SerializeField] private CharacterMovement m_movementComp = null;       // Enemies movement component
    [SerializeField] private Animator m_animatorComp = null;                // Enemies animator component
    [SerializeField] private HealthComponent m_healthComp = null;           // Enemies health component

    private Coroutine m_stunRoutine = null;         // Routine handling enemies stun

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

    /// <summary>
    /// If this enemy is currently stunned
    /// </summary>
    public bool isStunned { get { return m_stunRoutine != null; } }

    protected virtual void Awake()
    {
        m_movementComp = GetComponent<CharacterMovement>();
        m_animatorComp = GetComponent<Animator>();
        m_healthComp = GetComponent<HealthComponent>();

        if (m_healthComp)
        {
            m_healthComp.OnDamaged += OnDamaged;
            m_healthComp.OnDeath += OnDeath;
        }
    }

    protected virtual void Update()
    {
        if (animatorComponent && m_movementComp)
        {
            animatorComponent.SetBool("Jumping", m_movementComp.isJumping);
            animatorComponent.SetBool("Airborne", !m_movementComp.isGrounded);
        }
    }

    protected virtual void OnDamaged(HealthComponent self, float damage, DamageInfo info, DamageEvent args)
    {
        if (info && info.stunTime > 0f)
        { 
            if (m_canBeStunned)
                Stun(info.stunTime);
        }
    }

    protected virtual void OnDeath(HealthComponent self)
    {
        TryDroppingPickup();

        if (onEnemyDeath != null)
            onEnemyDeath.Invoke(this);

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
        float failChance = m_dropChance;
        if (failChance * 100f < Random.Range(0f, 100f))
            return;

        // Spawn the pickup right where we are
        Instantiate(m_deathDrop, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Stuns this enemy for given amount of time. If already stunned,
    /// will extend the duration without resetting
    /// </summary>
    /// <param name="time">Time to stun for</param>
    public void Stun(float time)
    {
        if (time <= 0f)
            return;

        bool alreadyStunned = isStunned;

        // Manually cancel ourselves instead of calling StopStun so we don't
        // trigger OnRecoveredFromStun, we treat this as an extension by time
        if (m_stunRoutine != null)
        {
            StopCoroutine(m_stunRoutine);
            m_stunRoutine = null;
        }

        if (!alreadyStunned)
            OnStunned();

        m_stunRoutine = StartCoroutine(StunRoutine(time));
    }

    /// <summary>
    /// Manually stop enemy being stunned. This will call OnRecoverFromStunned
    /// </summary>
    protected void StopStun()
    {
        if (m_stunRoutine == null)
        {
            StopCoroutine(m_stunRoutine);
            m_stunRoutine = null;

            OnRecoveredFromStun();
        }
    }

    /// <summary>
    /// Routine that handles stun durtation
    /// </summary>
    /// <param name="time">Time to stun for</param>
    private IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        m_stunRoutine = null;

        OnRecoveredFromStun();
    }

    /// <summary>
    /// Event called when stunned. Only called once per Stun
    /// </summary>
    protected virtual void OnStunned()
    {
        m_movementComp.SetMoveInputDisabled(true);
        m_animatorComp.SetBool("Stunned", true);
    }

    /// <summary>
    /// Event called when recovered from stun. Only called once per Stun
    /// </summary>
    protected virtual void OnRecoveredFromStun()
    {
        m_animatorComp.SetBool("Stunned", false);
        m_movementComp.SetMoveInputDisabled(false);
    }
}
