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
    protected readonly float floorCheckExtent = 0.075f;

    [Header("Movement")]
    [SerializeField, Min(0f)] public float m_walkSpeed = 10f;               // Normal walk speed while on the ground
    [SerializeField, Min(0f)] public float m_airSpeed = 8f;                 // Speed while in the air
    [SerializeField, Min(0f)] protected float m_maxAcceleration = 40f;      // Acceleration for reaching walk/air speed
    [SerializeField, Min(0f)] protected float m_brakeFriction = 50f;        // Friction to apply when no input has been applied
    [SerializeField, Min(0f)] protected float m_maxStepHeight = 0.2f;       // Max height character can step

    [Header("Movement (Jumping)")]
    [SerializeField, Min(0f)] public float m_jumpPower = 5f;                // Power of jump, decides velocity
    [SerializeField, Min(0f)] public float m_maxJumpHoldTime = 0.5f;        // Max amount of time jump can be held
    [SerializeField, Min(0)] public int m_maxAirJumps = 1;                  // Max number of times character can jump in air

    [Header("Components")]
    [SerializeField] protected Rigidbody2D m_rigidBody;                     // Rigidbody to move. Used to interact with world
    [SerializeField] protected CapsuleCollider2D m_collider;                // Collider that we move. Used for collision checks

    [Header("Config")]
    [SerializeField] public bool m_orientateToMovement = false;                     // If to rotate transform based on movement direction
    [SerializeField] public LayerMask m_worldLayers = Physics2D.AllLayers;          // Layers of world geoemetry

    protected float m_moveInput = 0f;           // Current input to apply next FixedUpdate()
    protected byte m_inputDisabled = 0;         // If to ignore move input (treat is as zero)
    protected bool m_isGrounded = true;         // If we are grounded (start as default)
    protected bool m_aboutToJump = false;       // If about to jumping, used to prevent jumping multiple time
    protected int m_numAirJumps = 0;            // Number of air jumps that have been done since last being grounded

    protected bool m_customMoveMode = false;    // Set to true to avoid normal fixed update calculations

    protected bool m_isJumping = false;         // If character is jumping
    protected float m_jumpHoldTime = 0f;        // Time jump has been active for

    protected Collider2D m_floorCollider;               // Floor character is standing on
    protected Vector2 m_floorLocation = Vector2.zero;   // Location floor was when last updated

    public new Rigidbody2D rigidbody2D { get { return m_rigidBody; } }
    public CapsuleCollider2D capsule { get { return m_collider; } }

    /// <summary>
    /// The velocity of the character
    /// </summary>
    public Vector2 velocity { get { return m_rigidBody.velocity; } }

    /// <summary>
    /// The bounds of the character
    /// </summary>
    public Bounds bounds { get { return capsule.bounds; } }

    /// <summary>
    /// If character is moving horizontally (not falling)
    /// </summary>
    public bool isMoving { get { return !Mathf.Approximately(velocity.x, 0.1f); } }

    /// <summary>
    /// If character is falling (y velocity is negative)
    /// </summary>
    public bool isFalling { get { return velocity.y < 0f; } }

    /// <summary>
    /// If character is about to or is jumping
    /// </summary>
    public bool isJumping { get { return m_isJumping || m_aboutToJump; } }

    /// <summary>
    /// If character is currently grounded
    /// </summary>
    public bool isGrounded { get { return m_isGrounded; } }

    /// <summary>
    /// If characters move input is disabled
    /// </summary>
    public bool inputDisabled { get { return m_inputDisabled > 0; } }

    /// <summary>
    /// Current input applied, considers if input is disabled
    /// </summary>
    public float currentInput { get { return inputDisabled ? 0f : m_moveInput; } }

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

    protected virtual void Update()
    {
        if (GameManager.isPaused)
            return;

        if (m_isJumping)
            HandleContinuedJump(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        // Certain calculations now depend on if we are on the floor
        UpdateIsGrounded(false);

        Vector2 velocity = m_rigidBody.velocity;

        float input = 0f;
        if (!m_customMoveMode)
        {
            input = currentInput;
            if (m_isGrounded && Mathf.Approximately(input, 0f))
            {
                // Apply a braking friction force to auto slow us down
                velocity.x = Mathf.Lerp(velocity.x, 0f, m_brakeFriction * Time.fixedDeltaTime);
            }
            else
            {
                float maxSpeed = GetMaxSpeed();

                velocity.x += input * m_maxAcceleration * Time.fixedDeltaTime;
                velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            }

            m_rigidBody.velocity = velocity;
            CheckIfAtStep();
        }
   
        m_aboutToJump = false;

        // Rotate ourselves if desired
        if (m_orientateToMovement)
        {
            if (input != 0f)
                transform.localEulerAngles = Helpers.FlipRotation(m_moveInput);
        } 
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
    /// Set if move input is disabled (treated as zero)
    /// </summary>
    /// <param name="disable">If to disable input</param>
    public void SetMoveInputDisabled(bool disable)
    {
        if (disable)
            ++m_inputDisabled;
        else
            --m_inputDisabled;

        if (m_inputDisabled < 0)
            m_inputDisabled = 0;
    }

    /// <summary>
    /// Have character jump once if able to. Call multiple times
    /// to increase
    /// </summary>
    /// <returns>If a jump was performed</returns>
    public virtual bool Jump()
    {
        if (CanJump())
        {
            // Consume an air jump
            if (!m_isGrounded)
                ++m_numAirJumps;

            m_aboutToJump = true;
            m_isJumping = true;
        }

        return m_isJumping;
    }

    /// <summary>
    /// Stops this character from jumping, this does not reset counters
    /// </summary>
    public virtual void StopJumping()
    {
        m_isJumping = false;
        m_jumpHoldTime = 0f;
    }

    /// <summary>
    /// If character is currently able to jump
    /// </summary>
    /// <returns>If character can jump</returns>
    public virtual bool CanJump()
    {
        // Already jumping, need to sto this jump first
        if (m_isJumping)
            return false;

        return m_isGrounded || m_numAirJumps < m_maxAirJumps;
    }

    /// <summary>
    /// Event called to handle a continued jump (if m_isJumping is true)
    /// </summary>
    /// <param name="deltaTime">Frame delta</param>
    protected virtual void HandleContinuedJump(float deltaTime)
    {
        Vector2 velocity = m_rigidBody.velocity;
        velocity.y = Mathf.Max(velocity.y, m_jumpPower);
        m_rigidBody.velocity = velocity;

        m_jumpHoldTime += Time.deltaTime;
        if (m_jumpHoldTime >= m_maxJumpHoldTime)
        {
            m_jumpHoldTime = 0f;
            m_isJumping = false;
        }
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

        Vector2 position = (Vector2)transform.position + (Vector2)transform.TransformVector(m_collider.offset);
        Vector2 extents = m_collider.size * 0.5f;
        
        // Position now at foot level
        position.y -= (extents.y * m_collider.transform.lossyScale.y);

        // Push further down to compensate for bounds vertical size
        position.y -= floorCheckExtent;
        position.y -= float.Epsilon;

        // Cut small little bit off the horizontal side so we don't check walls for being floors
        return new Bounds(position, new Vector3(extents.x * 0.9f, floorCheckExtent, 0f) * 2f);
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

    Vector2 hitSpot = Vector2.zero;

    protected void CheckIfAtStep()
    {
        if (!isGrounded || (!isMoving && currentInput != 0f))
            return;

        Vector2 velocity = m_rigidBody.velocity;
        Vector2 dir = new Vector2(Mathf.Sign(velocity.x), 0f);

        RaycastHit2D[] hits = new RaycastHit2D[1];
        int numHits = m_collider.Cast(dir, hits, Mathf.Abs(velocity.x) * Time.fixedDeltaTime);
        if (numHits > 0)
        {
            Vector2 capsuleBottom = (Vector2)transform.position - new Vector2(0f, m_collider.size.y + m_collider.offset.y);

            for (int i = 0; i < numHits; ++i)
            {
                RaycastHit2D hit = hits[i];
                hitSpot = hit.point;

                float yOffset = hit.point.y - capsuleBottom.y;
                if (yOffset <= 0f || yOffset > m_maxStepHeight)
                    continue;

                Vector2 newPosition = m_rigidBody.position + new Vector2(0f, yOffset);
                m_rigidBody.MovePosition(newPosition);
            }
        }

    }

    #region Debug
    protected virtual void OnDrawGizmos()
    {
        if (!m_collider)
            return;

        Bounds floorCheckBounds = GetFloorCheckBounds();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(floorCheckBounds.center, floorCheckBounds.size);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(hitSpot, 0.2f);
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