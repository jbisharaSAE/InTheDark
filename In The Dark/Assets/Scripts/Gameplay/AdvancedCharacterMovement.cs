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

    protected enum WallJumpSide
    {
        None,
        Left,
        Right
    }

    [Header("Advanced")]
    [SerializeField, Min(0f)] protected float m_wallJumpPower = 12f;        // Power of jump if wall jumping

    protected WallJumpSide m_lastWallJumpSide = WallJumpSide.None;          // Which side of the character was the wall we last jumped off

    void Update()
    {
        // For now, handling inputs here
        {
            SetMoveInput(Input.GetAxis("Horizontal"));

            if (Input.GetButtonDown("Jump"))
                Jump();

            // Debug
            if (Input.GetKeyDown(KeyCode.F))
                m_rigidBody.velocity = Vector2.zero;
        }
    }

    protected override void OnLanded()
    {
        base.OnLanded();
        m_lastWallJumpSide = WallJumpSide.None;
    }

    /// <summary>
    /// Check if we can either perform a normal jump or a wall jump.
    /// Wall jumps take priority of the regular jumps
    /// </summary>
    /// <returns>If a jump was performed</returns>
    public override bool Jump()
    {
        if (m_aboutToJump)
            return false;

        // First check if we can wall jump
        if (!m_isGrounded)
        {
            if (m_lastWallJumpSide != WallJumpSide.Left && CanWallJump(WallJumpSide.Left))
            {
                // Jump to the right
                Vector2 velocity = m_rigidBody.velocity;
                velocity = new Vector2(0.77f, 0.77f) * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_aboutToJump = true;
                m_lastWallJumpSide = WallJumpSide.Left;
                return true;
            }
            else if (m_lastWallJumpSide != WallJumpSide.Right && CanWallJump(WallJumpSide.Right))
            {
                // Jump to the left
                Vector2 velocity = m_rigidBody.velocity;
                velocity = new Vector2(-0.77f, 0.77f) * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_aboutToJump = true;
                m_lastWallJumpSide = WallJumpSide.Right;
                return true;
            }
        }

        return base.Jump();
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

        Collider2D[] hits = Physics2D.OverlapBoxAll(wallCheckBounds.center, wallCheckBounds.size, 0f, m_worldLayers);
        if (hits != null && hits.Length > 0)
        {
            // Make sure we aren't detecting ourself
            foreach (Collider2D col in hits)
                if (col.gameObject != gameObject)
                    return true;
        }

        return false;
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

        Vector2 position = transform.position;
        Vector2 extents = m_collider.bounds.extents;
        float multiplier = side == WallJumpSide.Left ? -1f : 1f;

        // Position at side of collider
        position.x += (extents.x * m_collider.transform.lossyScale.x) * multiplier;

        // Push further to side to compensate for bounds horizontal size
        position.x += wallCheckExtent * multiplier;
        position.x += float.Epsilon;

        // Cut the veritcal check size off a bit (we don't want to confuse ground/roof with walls
        return new Bounds(position, new Vector3(wallCheckExtent, extents.y * 0.8f, 0f) * 2f);
    }

    #region Debug
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!m_collider)
            return;

        Bounds leftWallCheckBounds = GetWallCheckBounds(WallJumpSide.Left);
        Gizmos.DrawWireCube(leftWallCheckBounds.center, leftWallCheckBounds.size);

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
    }
}
#endif
