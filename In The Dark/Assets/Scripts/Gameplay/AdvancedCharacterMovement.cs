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
    [Header("Advanced")]
    public float m_wallJumpPower = 12f;         // Power of jump if wall jumping

    public override bool Jump()
    {
        if (base.Jump())
            return true;

        return TryWallJump();
    }

    private bool TryWallJump()
    {
        float checkSize = 0.1f;

        Vector2 position = transform.position;

        Vector2 size = m_collider.size;
        Vector2 extent = size * 0.5f;
        Vector2 extentHalf = extent * 0.5f;

        // Jump of left wall
        {
            Vector2 checkPos = position;
            checkPos.x -= (extent.x + checkSize);
            if (CanWallJump(checkPos, new Vector2(checkSize * 2f, size.y * 0.8f)))
            {
                // Jump right
                m_rigidBody.velocity += new Vector2(0.77f, 0.77f) * m_wallJumpPower;
                return true;
            }
        }

        // Jump of right wall
        {
            Vector2 checkPos = position;
            checkPos.x += (extent.x + checkSize);
            if (CanWallJump(checkPos, new Vector2(checkSize * 2f, size.y * 0.8f)))
            {
                // Jump left
                m_rigidBody.velocity += new Vector2(-0.77f, 0.77f) * m_wallJumpPower;
                return true;
            }
        }

        return false;
    }

    private bool CanWallJump(Vector2 position, Vector2 size)
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(position, size, 0f, m_worldLayers.value);
        if (cols != null && cols.Length > 0)
            // Make sure we are not colliding with ourselves
            foreach (Collider2D col in cols)
                if (col.gameObject != gameObject)
                    return true;

        return false;
    }

    #region Debug
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!m_collider)
            return;

        float checkSize = 0.1f;

        Vector2 position = transform.position;

        Vector2 size = m_collider.size;
        Vector2 extent = size * 0.5f;
        Vector2 extentHalf = extent * 0.5f;

        Gizmos.color = Color.green;

        // Left wall col
        {
            Vector2 checkPos = position;
            checkPos.x -= extent.x + checkSize;
            Gizmos.DrawWireCube(checkPos, new Vector2(checkSize * 2f, size.y * 0.8f));
        }

        // Right wall col
        {
            Vector2 checkPos = position;
            checkPos.x += extent.x + checkSize;
            Gizmos.DrawWireCube(checkPos, new Vector2(checkSize * 2f, size.y * 0.8f));
        }
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
