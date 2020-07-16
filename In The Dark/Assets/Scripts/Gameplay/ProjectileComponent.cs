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
    public DamageInfo m_damageInfo = null;      // Info of damage to automatically apply, can be null
    public float m_lifespan = 2f;               // Lifespan of projectile, gets destroyed after this time (0 = No lifespan)
    public int m_autoDestroyAfter = 1;          // Destroy this projectile after hitting X objects (leave 0 or negative for inf)

    private Rigidbody2D m_rigidBody = null;     // Projectiles rigidbody
    private Collider2D m_collider = null;       // Projectiles collider (cached for ease of access)

    private int m_numHits = 0;                  // Number of objects that has been hit
    private bool m_pendingDestroy = false;      // If pending destroy

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
        if (m_pendingDestroy)
            return;

        // Ignore triggers
        if (collision.isTrigger)
            return;

        GameObject hitObject = collision.gameObject;

        // TODO: Would be nicer to do something like collision.GetComponent<IProjectileCollisionHandler>()
        // so objects individually could handle stuff, such as Parry
        // For now ( Ideally if (collisionHandler.HandleCollision(this)) )
        if (collision.tag == "ParryBox")
        {
            // Negate velocity, simply reflect back
            m_rigidBody.velocity *= -1f;

            // TODO: Add some effects + sound

            return;
        }

        ++m_numHits;

        // We only auto destroy after hit X amount of objects
        bool finalHit = m_autoDestroyAfter > 0 && m_numHits >= m_autoDestroyAfter;

        // TODO: This is 'hit' world objects. We could ideally just check the layer
        // but not all world collision might use it. We assume that world collision
        // just does not have a rigidbody
        if (hitObject.GetComponent<Rigidbody2D>() == null)
            finalHit = true;
        else
            OnObjectHit(hitObject, collision, finalHit);

        if (finalHit)
        {
            OnFinalHit(hitObject, collision);
            DestroySelf(false);
        }
    }

    /// <summary>
    /// Event that is called when this projectile hits an object
    /// </summary>
    /// <param name="hitObject">Game Object that was hit</param>
    /// <param name="hitCollider">Collider that was hit</param>
    /// <param name="finalHit">If this is the final hit before projectile is auto destroyed</param>
    protected virtual void OnObjectHit(GameObject hitObject, Collider2D hitCollider, bool finalHit)
    {
        HealthComponent healthComp = hitObject.GetComponent<HealthComponent>();
        if (healthComp)
            healthComp.ApplyDamage(m_damageInfo);
    }

    /// <summary>
    /// Event that is called only on the final hit before projectile auto destroys itself
    /// </summary>
    /// <param name="hitObject">Game Object that was hit</param>
    /// <param name="hitCollider">Collider that was hit</param>
    protected virtual void OnFinalHit(GameObject hitObject, Collider2D hitCollider)
    {

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
        DestroySelf(true);
    }

    /// <summary>
    /// Destroys this projectile
    /// </summary>
    /// <param name="expired">If destroying the projectile due to it expiring</param>
    protected virtual void DestroySelf(bool expired)
    {
        if (m_pendingDestroy)
            return;

        m_pendingDestroy = true;
        Destroy(gameObject);
    }
}
