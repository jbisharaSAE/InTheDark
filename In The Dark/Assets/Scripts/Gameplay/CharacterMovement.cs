using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This component handles the movement of a character. It does not directly move the character
/// but rather has inputs that can be used to do so
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    protected readonly float floorCheckExtent = 0.025f;

    [Header("Movement")]
    [SerializeField, Min(0f)] public float m_walkSpeed = 10f;               // Normal walk speed while on the ground
    [SerializeField, Min(0f)] public float m_airSpeed = 8f;                 // Speed while in the air
    [SerializeField, Min(0f)] protected float m_maxAcceleration = 40f;      // Acceleration for reaching walk/air speed
    [SerializeField, Min(0f)] protected float m_brakeFriction = 50f;        // Friction to apply when no input has been applied
    [SerializeField, Min(0f)] protected float m_jumpPower = 5f;             // Power of jump, decides velocity
    [SerializeField, Min(0)] private int m_maxAirJumps = 1;                 // Max number of times character can jump in air

    [Header("Components")]
    [SerializeField] protected Rigidbody2D m_rigidBody;                     // Rigidbody to move. Used to interact with world
    [SerializeField] protected CapsuleCollider2D m_collider;                // Collider that we move. Used for collision checks

    [Header("Config")]
    [SerializeField] public bool m_orientateToMovement = false;                     // If to rotate transform based on movement direction
    [SerializeField] public LayerMask m_worldLayers = Physics2D.AllLayers;          // Layers of world geoemetry

    protected float m_moveInput = 0f;           // Current input to apply next FixedUpdate()
    protected bool m_isGrounded = true;         // If we are grounded (start as default)
    protected bool m_aboutToJump = false;       // If about to jumping, used to prevent jumping multiple time
    protected int m_numAirJumps = 0;            // Number of air jumps that have been done since last being grounded

    protected Collider2D m_floorCollider;               // Floor character is standing on
    protected Vector2 m_floorLocation = Vector2.zero;   // Location floor was when last updated

    public Rigidbody2D rigidbody2D { get { return m_rigidBody; } }
    public CapsuleCollider2D capsule { get { return m_collider; } }

    /// <summary>
    /// The velocity of the character
    /// </summary>
    public Vector3 velocity { get { return m_rigidBody.velocity; } }

    /// <summary>
    /// If character is currently grounded
    /// </summary>
    public bool isGrounded { get { return m_isGrounded; } }

    protected virtual void Awake()
    {
        if (!m_collider)
            m_collider = GetComponent<CapsuleCollider2D>();

        if (!m_rigidBody)
        {
            // Need a rigidbody to properly operate
            m_rigidBody = GetComponent<Rigidbody2D>();
            if (!m_rigidBody)
            {
                enabled = false;
                return;
            }
        }     
    }

    protected virtual void FixedUpdate()
    {
        // Certain calculations now depend on if we are on the floor
        UpdateIsGrounded(false);

        Vector2 velocity = m_rigidBody.velocity;

        if (m_isGrounded && Mathf.Approximately(m_moveInput, 0f))
        {
            // Apply a braking friction force to auto slow us down
            velocity.x = Mathf.Lerp(velocity.x, 0f, m_brakeFriction * Time.fixedDeltaTime);
        }
        else
        {
            float maxSpeed = GetMaxSpeed();

            velocity.x += m_moveInput * m_maxAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }

        m_rigidBody.velocity = velocity;
        m_aboutToJump = false;

        // Rotate ourselves if desired
        if (m_orientateToMovement)
        {
            if (m_moveInput != 0f)
                transform.localEulerAngles = Helpers.FlipRotation(m_moveInput);
        }

        // Consume input
        //m_moveInput = 0f;
    }

    /// <summary>
    /// Set the horizontal input of movement
    /// </summary>
    /// <param name="input">Amount of input to apply</param>
    public virtual void SetMoveInput(float input)
    {
        m_moveInput = input;
    }

    /// <summary>
    /// Have character jump once if able to
    /// </summary>
    /// <returns>If a jump was performed</returns>
    public virtual bool Jump()
    {
        if (CanJump())
        {
            Vector2 velocity = m_rigidBody.velocity;
            velocity.y = m_jumpPower; // Velocity change
            m_rigidBody.velocity = velocity;

            // Consume an air jump
            if (!m_isGrounded)
                ++m_numAirJumps;

            m_aboutToJump = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// If character is currently able to jump
    /// </summary>
    /// <returns>If character can jump</returns>
    public virtual bool CanJump()
    {
        if (m_aboutToJump)
            return false;

        return m_isGrounded || m_numAirJumps < m_maxAirJumps;
    }

    /// <summary>
    /// Updates the characters grounded state
    /// </summary>
    /// <param name="moveToFloor">If character should be moved to floor (if grounded)</param>
    protected void UpdateIsGrounded(bool moveToFloor)
    {
        bool wasGrounded = m_isGrounded;
        m_isGrounded = false;

        // Need the collider for proper calculations
        if (!m_collider)
            return;

        bool shouldClearVelY = false;
        if (wasGrounded && moveToFloor)
        {
            // Floor collider might be invalid if destroyed
            if (m_floorCollider)
            {
                Vector2 displacement = (Vector2)m_floorCollider.transform.position - m_floorLocation;
                if (!m_aboutToJump)
                {
                    m_rigidBody.position = m_rigidBody.position + displacement;
                    shouldClearVelY = true;
                }               
            }
        }

        m_floorCollider = null;
        m_floorLocation = Vector2.zero;

        Bounds floorCheckBounds = GetFloorCheckBounds();

        Collider2D[] hits = Physics2D.OverlapBoxAll(floorCheckBounds.center, floorCheckBounds.size, 0f, m_worldLayers);
        if (hits != null && hits.Length > 0)
        {
            // Make sure we aren't colliding with ourself
            foreach(Collider2D col in hits)
                if (col.gameObject != gameObject)
                {
                    m_isGrounded = true;
                    m_floorCollider = col;
                    m_floorLocation = col.transform.position;

                    if (shouldClearVelY)
                    {
                        // Attempt to try and stop jitterness while on moving floors
                        Vector2 velocity = m_rigidBody.velocity;
                        velocity.y = 0f;
                        m_rigidBody.velocity = velocity;
                    }
                    else if (!moveToFloor)
                    {
                        StartCoroutine(PostFixedUpdate());
                    }

                    break;
                }
        }

        if (m_isGrounded != wasGrounded)
            if (m_isGrounded)
                OnLanded();
    }

    /// <summary>
    /// Event called when landing after having been airborne
    /// </summary>
    protected virtual void OnLanded()
    {
        m_numAirJumps = 0;
    }

    /// <summary>
    /// Routine that executes after FixedUpdate is called on all game objects
    /// </summary>
    private IEnumerator PostFixedUpdate()
    {
        yield return new WaitForFixedUpdate();
        //UpdateIsGrounded(true);
    }

    /// <summary>
    /// Get the bounds (box) used for checking if grounded
    /// </summary>
    /// <returns>Foot check bounds in world space</returns>
    protected Bounds GetFloorCheckBounds()
    {
        if (!m_collider)
            return new Bounds(transform.position, Vector3.one);

        Vector2 position = transform.position;
        Vector2 extents = m_collider.size * 0.5f;
        
        // Position now at foot level
        position.y -= (extents.y * m_collider.transform.lossyScale.y);

        // Push further down to compensate for bounds vertical size
        position.y -= floorCheckExtent;
        position.y -= float.Epsilon;

        // Cut small little bit off the horizontal side so we don't check walls for being floors
        return new Bounds(position, new Vector3(extents.x * 0.95f, floorCheckExtent, 0f) * 2f);
    }

    /// <summary>
    /// Get the max horizonatal speed this character can go
    /// </summary>
    /// <returns>Max speed of character</returns>
    protected virtual float GetMaxSpeed()
    {
        if (!m_isGrounded)
            return m_airSpeed;

        return m_walkSpeed;
    }

    #region Debug
    protected virtual void OnDrawGizmos()
    {
        if (!m_collider)
            return;

        Bounds floorCheckBounds = GetFloorCheckBounds();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(floorCheckBounds.center, floorCheckBounds.size);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterMovement))]
public class CharacterMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        PrintRuntimeValuesLabel();
        PrintRuntimeValues(serializedObject.targetObject as CharacterMovement);
    }
    
    /// <summary>
    /// This function simply prints the header for the runtime values section
    /// </summary>
    private void PrintRuntimeValuesLabel()
    {
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Runtime Values");
        EditorStyles.label.fontStyle = FontStyle.Normal;
    }

    /// <summary>
    /// This function prints label of all the runtime values
    /// </summary>
    /// <param name="movement">Movement component to print</param>
    protected virtual void PrintRuntimeValues(CharacterMovement movement)
    {
        EditorGUILayout.Toggle("Is Grounded", movement.isGrounded);
    }
}
#endif