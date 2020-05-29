using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component acts as both a movement component and trigger for dealing damage
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileComponent : MonoBehaviour
{
    public float m_speed = 15f;                 // Speed of this projectile
    public float m_damage = 10f;                // Damage this projectile does upon impact
    public float m_lifespan = 2f;               // Lifespan of projectile, gets destroyed after this time (0 = No lifespan)

    private Rigidbody2D m_rigidBody = null;     // Projectiles rigidbody
    private Collider2D m_collider = null;       // Projectiles collider (cached for ease of access)

    /// <summary>
    /// The game object that spawned this projectile
    /// </summary>
    public GameObject instigator { get; set; }

    /// <summary>
    /// Set if this projectile should ignore collisions with its instigator
    /// </summary>
    public bool ignoreInstigator { set { SetIgnoreInstigator(value); } }

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.isKinematic = true;

        m_collider = GetComponent<Collider2D>();
        if (m_collider)
            m_collider.isTrigger = true;
    }

    void Start()
    {
        if (m_lifespan > 0)
            StartCoroutine(LifespanRoutine());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore triggers
        if (collision.isTrigger)
            return;

        GameObject hitObject = collision.gameObject;

        // Apply damage if we can
        {
            HealthComponent healthComp = hitObject.GetComponent<HealthComponent>();
            if (healthComp)
                healthComp.ApplyDamage(m_damage);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Helper for spawning a new projectile prefab. Will initialize projectiles rotation and movement
    /// </summary>
    /// <param name="prefab">Prefab to spawn</param>
    /// <param name="position">Position to instantiate at</param>
    /// <param name="direction">Direction to travel at</param>
    /// <returns>Valid component or null</returns>
    public static ProjectileComponent SpawnProjectile(ProjectileComponent prefab, Vector2 position, Vector2 direction)
    {
        if (!prefab)
            return null;

        ProjectileComponent projectile = Instantiate(prefab, position, Quaternion.identity);
        if (!projectile)
            return null;

        projectile.InitializeProjectile(direction.normalized);
        return projectile;
    }

    /// <summary>
    /// Initializes the projectile. This is only called from the SpawnProjectile helper
    /// </summary>
    /// <param name="direction">Normalized travel direction</param>
    protected virtual void InitializeProjectile(Vector2 direction)
    {
        m_rigidBody.velocity = direction * m_speed;

        // Rotate to face travel direction
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
    }

    /// <summary>
    /// Handles setting if collision between this projectile and instigator should be ignored
    /// </summary>
    /// <param name="ignore">If to ignore collision</param>
    private void SetIgnoreInstigator(bool ignore)
    {
        if (instigator)
        {
            Collider2D instigatorCollider = instigator.GetComponent<Collider2D>();
            if (instigatorCollider)
                Physics2D.IgnoreCollision(m_collider, instigatorCollider);
        }
    }

    /// <summary>
    /// Routine that handles this projectiles lifespan
    /// </summary>
    private IEnumerator LifespanRoutine()
    {
        yield return new WaitForSeconds(m_lifespan);
        Destroy(gameObject);
    }
}
