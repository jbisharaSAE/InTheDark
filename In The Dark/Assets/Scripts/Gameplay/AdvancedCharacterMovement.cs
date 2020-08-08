using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This component handles the movement of a character along with advanced movements
/// </summary>
public class AdvancedCharacterMovement : CharacterMovement
{
    protected readonly float wallCheckExtent = 0.1f;

    public enum WallJumpSide
    {
        None,
        Left,
        Right
    }

    [Header("Advanced (Wall Jump)")]
    [SerializeField] protected bool m_limitWallJumpSide = false;                // If wall jump side should be limited to once
    [SerializeField, Min(0f)] protected float m_wallJumpPower = 12f;            // Power of jump if wall jumping
    [SerializeField] private LayerMask m_wallJumpLayers = Physics2D.AllLayers;  // Layers of collision player can wall jump off of

    [Header("Advanced (Dash)")]
    [SerializeField, Min(0f)] protected float m_dashSpeed = 20f;                                // Speed of the dash
    [SerializeField, Min(0f)] protected float m_dashTime = 0.2f;                                // How long the dash lasts for
    [SerializeField] protected bool m_canDashInAir = true;                                      // If we can dash in the air
    [SerializeField] private List<string> m_layersToIgnoreWhenDashing = new List<string>();     // Layers to ignore when dashing
    [SerializeField] private AudioSource m_dashAudioSource = null;                              // Audio to play when dashing

    protected WallJumpSide m_lastWallJumpSide = WallJumpSide.None;          // Which side of the character was the wall we last jumped off
    protected bool m_isDashing = false;                                     // If currently dashing
    protected float m_dashDir = 0f;                                         // The direction to dash in
    protected float m_dashEnd = -1f;                                        // When dash is expected to end
    private float m_originalGravityScale = 1f;                              // Gravity scale of rigidbody before dash

    private Dictionary<int, int> m_layersDisabled = new Dictionary<int, int>();

    /// <summary>
    /// Last wall the character had jumped on
    /// </summary>
    public WallJumpSide lastWallJumpSide { get { return m_lastWallJumpSide; } }

    /// <summary>
    /// If this character is currently dashing
    /// </summary>
    public bool isDashing { get { return m_isDashing; } }

    [Header("Debug")]
    public bool m_useDebugControls = false;         // If to use debug controls

    protected override void Update()
    {
        m_maxStepHeight = 0f;

        // For now, handling inputs here
        if (m_useDebugControls)
        {
            SetMoveInput(Input.GetAxisRaw("Horizontal"));

            if (Input.GetButtonDown("Jump"))
                Jump();
            else if (Input.GetButtonUp("Jump"))
                StopJumping();

            if (Input.GetKeyDown(KeyCode.P))
                Dash();

            // Debug
            if (Input.GetKeyDown(KeyCode.F))
                m_rigidBody.velocity = Vector2.zero;
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        bool wasGrounded = isGrounded;

        base.FixedUpdate();

        if (m_isDashing)
        {
            Vector2 velocity = m_rigidBody.velocity;
            velocity.x = m_dashSpeed * m_dashDir;
            velocity.y = 0f;

            //// Keep grounded (we might be going down a small slope)
            //if (isGrounded)
            //{
            //    Vector2 traceStart = m_rigidBody.position + (velocity * Time.fixedDeltaTime);

            //    Vector2 extents = m_collider.size * 0.5f;
            //    extents *= (Vector2)transform.lossyScale;

            //    float distance = extents.y * 1.5f;

            //    traceStart.x += extents.x;

            //    RaycastHit2D result = Physics2D.Raycast(traceStart, Vector2.down, distance, m_worldLayers);
            //    if (result.point.y < traceStart.y - extents.y)
            //    {
            //        Vector2 position = m_rigidBody.position;
            //        position.y = result.point.y + extents.y;
            //        m_rigidBody.position = position;
            //    }
            //}
            
            // Finish dashing if time has elapsed
            if (Time.time >= m_dashEnd)
            {
                m_isDashing = false;
                m_customMoveMode = false;

                SetIgnoreLayers(m_layersToIgnoreWhenDashing, false);

                OnDashFinished();
                velocity.y = 0.1f;
            }

            m_rigidBody.velocity = velocity;
            CheckIfAtStep();
        }
    }

    protected override void OnLanded()
    {
        base.OnLanded();
        m_lastWallJumpSide = WallJumpSide.None;
    }

    protected virtual void OnDashFinished()
    {
        m_dashEnd = -1f;
        m_dashDir = 0f;

        m_rigidBody.gravityScale = m_originalGravityScale;
    }

    /// <summary>
    /// Check if we can either perform a normal jump or a wall jump.
    /// Wall jumps take priority of the regular jumps
    /// </summary>
    /// <returns>If a jump was performed</returns>
    public override bool Jump()
    {
        if (m_aboutToJump || m_isDashing)
            return false;

        // First check if we can wall jump
        if (!m_isGrounded)
        {
            bool allowLeftJump = !m_limitWallJumpSide || m_lastWallJumpSide != WallJumpSide.Left;
            bool allowRightJump = !m_limitWallJumpSide || m_lastWallJumpSide != WallJumpSide.Right;
            if (allowLeftJump && CanWallJump(WallJumpSide.Left))
            {
                Vector2 angle = new Vector2(0.77f, 0.77f);
                if (m_lastWallJumpSide == WallJumpSide.Left)
                    angle = new Vector2(0.175f, 0.98f);

                // Jump to the right
                Vector2 velocity = angle * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_moveInput = 0f;

                m_aboutToJump = true;
                m_lastWallJumpSide = WallJumpSide.Left;

                StartCoroutine(DisableInputAfterWallJump());
                return true;
            }
            else if (allowRightJump && CanWallJump(WallJumpSide.Right))
            {
                Vector2 angle = new Vector2(-0.77f, 0.77f);
                if (m_lastWallJumpSide == WallJumpSide.Right)
                    angle = new Vector2(-0.175f, 0.98f);

                // Jump to the left
                Vector2 velocity = angle * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_moveInput = 0f;

                m_aboutToJump = true;
                m_lastWallJumpSide = WallJumpSide.Right;
                StartCoroutine(DisableInputAfterWallJump());
                return true;
            }
        }

        return base.Jump();
    }

    /// <summary>
    /// Make this character dash once
    /// </summary>
    /// <returns>If this character is dashing</returns>
    public bool Dash()
    {
        if (CanDash())
        {
            m_isDashing = true;
            m_dashEnd = Time.time + m_dashTime;

            // Calculate which way to dash
            {
                Vector2 velocity = m_rigidBody.velocity;
                // Prioritize move input specified
                if (m_moveInput != 0f)
                {
                    m_dashDir = Mathf.Sign(m_moveInput);
                }
                // Fallback to velocity
                else if (velocity.x != 0f)
                {
                    m_dashDir = Mathf.Sign(velocity.x);
                }
                // If standing still with no input, use the objects orientation
                else
                {
                    // Using scale for now as this script will most likely only be used by the player script
                    // which uses scaling for 'flipping' the character
                    m_dashDir = Mathf.Sign(transform.lossyScale.x);
                }
            }

            SetIgnoreLayers(m_layersToIgnoreWhenDashing, true);

            m_originalGravityScale = m_rigidBody.gravityScale;
            m_rigidBody.gravityScale = 0f;

            if (m_dashAudioSource)
                m_dashAudioSource.Play();

            m_customMoveMode = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if this character can perform a wall jump (in terms of collision)
    /// </summary>
    /// <param name="side">Side of character to check</param>
    /// <returns>If able to jump off a wall</returns>
    protected bool CanWallJump(WallJumpSide side)
    {
        if (side == WallJumpSide.None)
            return false;

        Bounds wallCheckBounds = GetWallCheckBounds(side);

        Collider2D[] hits = Physics2D.OverlapBoxAll(wallCheckBounds.center, wallCheckBounds.size, 0f, m_wallJumpLayers);
        if (hits != null && hits.Length > 0)
        {
            // Make sure we aren't detecting ourself
            foreach (Collider2D col in hits)
            {
                if (col.gameObject == gameObject)
                    return false;

                return true;             
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if this character can peform a dash
    /// </summary>
    /// <returns>If able to dash</returns>
    protected bool CanDash()
    {
        // We count this as input
        if (inputDisabled)
            return false;

        if (m_isDashing)
            return false;

        if (m_aboutToJump || m_isJumping)
            return false;

        return !m_isGrounded || m_canDashInAir;
    }

    /// <summary>
    /// Get the bounds (box) used for checking if grounded
    /// </summary>
    /// <returns>Foot check bounds in world space</returns>
    protected Bounds GetWallCheckBounds(WallJumpSide side)
    {
        if (side == WallJumpSide.None)
            return new Bounds();

        if (!m_collider)
            return new Bounds(transform.position, Vector3.one);

        Vector2 position = (Vector2)transform.position + (Vector2)transform.TransformVector(m_collider.offset);
        Vector2 extents = m_collider.bounds.extents;
        float multiplier = side == WallJumpSide.Left ? -1f : 1f;

        // Position at side of collider
        position.x += (extents.x * Mathf.Abs(m_collider.transform.lossyScale.x)) * multiplier;

        // Push further to side to compensate for bounds horizontal size
        position.x += wallCheckExtent * multiplier;
        position.x += float.Epsilon;

        // Cut the veritcal check size off a bit (we don't want to confuse ground/roof with walls
        return new Bounds(position, new Vector3(wallCheckExtent, extents.y * 0.8f, 0f) * 2f);
    }

    private IEnumerator DisableInputAfterWallJump()
    {
        SetMoveInputDisabled(true);
        yield return new WaitForSeconds(0.1f);
        SetMoveInputDisabled(false);
    }

    public void SetIgnoreLayers(List<string> layers, bool ignore)
    {
        if (layers == null || layers.Count == 0)
            return;

        if (!m_collider)
            return;

        int colliderLayerId = m_collider.gameObject.layer;
        foreach (string layer in layers)
        {
            int layerId = LayerMask.NameToLayer(layer);
            if (layerId < 0)
                continue;

            int numTimesIgnored = -1;
            if (m_layersDisabled.TryGetValue(layerId, out numTimesIgnored))
            {
                if (ignore)
                    ++numTimesIgnored;
                else
                    --numTimesIgnored;

                if (numTimesIgnored > 0)
                    m_layersDisabled[layerId] = numTimesIgnored;
                else
                {
                    Physics2D.IgnoreLayerCollision(colliderLayerId, layerId, false);
                    m_layersDisabled.Remove(layerId);
                }
            }
            else if (ignore)
            {
                Physics2D.IgnoreLayerCollision(colliderLayerId, layerId);
                m_layersDisabled.Add(layerId, 1);
            }
        }
    }

    #region Debug
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!m_collider)
            return;

        Gizmos.color = Color.red;
        Bounds leftWallCheckBounds = GetWallCheckBounds(WallJumpSide.Left);
        Gizmos.DrawWireCube(leftWallCheckBounds.center, leftWallCheckBounds.size);

        Gizmos.color = Color.blue;
        Bounds rightWallCheckBounds = GetWallCheckBounds(WallJumpSide.Right);
        Gizmos.DrawWireCube(rightWallCheckBounds.center, rightWallCheckBounds.size);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(AdvancedCharacterMovement))]
public class AdvancedCharacterMovementEditor : CharacterMovementEditor
{
    protected override void PrintRuntimeValues(CharacterMovement movement)
    {
        base.PrintRuntimeValues(movement);

        AdvancedCharacterMovement advancedMovement = movement as AdvancedCharacterMovement;
        EditorGUILayout.Toggle("Is Dashing", advancedMovement.isDashing);
        EditorGUILayout.EnumPopup("Last Wall Jump Side", advancedMovement.lastWallJumpSide);
    }
}
#endif
