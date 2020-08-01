using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteEnemyScript : EnemyScript
{
    [Header("Components", order = 0)]
    [SerializeField] private PatrolArea m_patrolArea = null;                    // Area this enemy must stay within
    [SerializeField] private WallDetectionComponent m_wallDetection = null;     // Used for detecting obstacles
    [SerializeField] private EnemyMeleeAttack m_meleeAttack = null;             // Used for attacking

    [Header("Brute Attributes")]
    [SerializeField] private float m_idleTime = 2f;         // How long to remain idle for
    [SerializeField] private float m_patrolSpeed = 4f;      // Speed of this enemy when patrolling
    [SerializeField] private float m_chaseSpeed = 8f;       // Speed of this enemy when chasing
    [SerializeField] private float m_attackRange = 5f;      // Attack range target must be within // TODO: Move to EnemyMeleeAttack?

    private bool m_isChasing = false;
    private byte m_inJumpArea = 0;

    /// <summary>
    /// This brutess patrol area/chase limits
    /// </summary>
    public PatrolArea patrolArea
    {
        get
        {
            return m_patrolArea;
        }
        set
        {
            m_patrolArea = value;
        }
    }

    public WallDetectionComponent wallDetection { get { return m_wallDetection; } }
    public EnemyMeleeAttack meleeAttack { get { return m_meleeAttack; } }

    public bool inJumpSpot { get { return m_inJumpArea > 0; } }

    public float idleTime { get { return m_idleTime; } }
    public float attackRange { get { return m_attackRange; } }

    protected override void Awake()
    {
        base.Awake();

        if (!m_wallDetection)
            m_wallDetection = GetComponentInChildren<WallDetectionComponent>();

        if (!m_meleeAttack)
            m_meleeAttack = GetComponentInChildren<EnemyMeleeAttack>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AIJumpSpot>())
            ++m_inJumpArea;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<AIJumpSpot>())
            --m_inJumpArea;
    }

    public void OnEnterPatrol()
    {
        m_isChasing = false;

        if (movementComponent)
        {
            //movementComponent.m_orientateToMovement = true;
            movementComponent.m_walkSpeed = m_patrolSpeed;
        }

        GetComponent<EnemyTargetSelector>().m_focusSightOnTarget = false;
    }

    public void OnEnterChase()
    {
        m_isChasing = true;

        if (movementComponent)
        {
            //movementComponent.m_orientateToMovement = false;
            movementComponent.m_walkSpeed = m_chaseSpeed;
        }

        GetComponent<EnemyTargetSelector>().m_focusSightOnTarget = true;
    }

    protected override void OnDamaged(HealthComponent self, float damage, DamageInfo info, DamageEvent args)
    {
        base.OnDamaged(self, damage, info, args);

        // Still turn around if stunned
        if (self.isDead || m_isChasing)
            return;

        // Turn around to face the direction we were hit. Do this by default as it's likely we would already
        // be chasing the instigator if already facing them (thus meaning we are most likely facing away)
        if (args != null)
        {
            Vector2 displacement = args.m_hitLocation - (Vector2)transform.position;
            float desiredRot = displacement.x > 0f ? 0f : 180f;
            if (transform.eulerAngles.y != desiredRot)
                transform.eulerAngles = new Vector3(0f, desiredRot, 0f);
        }
        else
        {
            float desiredRot = transform.eulerAngles.y > 0f ? 0f : 180f;
            transform.eulerAngles = new Vector3(0f, desiredRot, 0f);
        }

        animatorComponent.SetBool("Idle", true);
        StartCoroutine(WaitTillNextTick());
    }

    IEnumerator WaitTillNextTick()
    {
        bool wasOrientateToMovement = movementComponent.m_orientateToMovement;
        movementComponent.m_orientateToMovement = false;
        yield return null;
        //movementComponent.m_orientateToMovement = wasOrientateToMovement;
    }
}
