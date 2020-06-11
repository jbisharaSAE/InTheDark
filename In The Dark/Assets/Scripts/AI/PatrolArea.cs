using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolArea : MonoBehaviour
{
    [SerializeField] private Vector2 m_patrolExtents = new Vector2(10f, 1.25f);     // Extent of the patrol area
    [SerializeField] private Vector2 m_chaseExtents = new Vector2(20f, 1f);         // Max area specified for AI using this component

    /// <summary>
    /// Quick access to the 'position' of this patrol area (center)
    /// </summary>
    public Vector2 position { get { return transform.position; } }

    /// <summary>
    /// Checks if given point is in the patrol area (only checks X axis)
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <returns>If inside this area</returns>
    public bool IsInPatrolArea(Vector2 point)
    {
        return IsInArea(point, m_patrolExtents);
    }

    public bool IsInChaseArea(Vector2 point)
    {
        return IsInArea(point, m_chaseExtents);
    }

    private bool IsInArea(Vector2 point, Vector2 areaExtent)
    {
        // TODO: Would probably want to check Y to, but for now we only need X
        Vector2 displacement = point - position;
        return Mathf.Abs(displacement.x) <= areaExtent.x;
    }

    /// <summary>
    /// Checks if some point has passed area based on a travel direction (on the X axis)
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="direction">Direction of movement (on X axis)</param>
    /// <returns>If section has been passed</returns>
    public bool HasPassedPatrolArea(Vector2 point, float direction)
    {
        return HasPassedArea(point, direction, m_patrolExtents);
    }

    public bool HasPassedChaseArea(Vector2 point, float direction)
    {
        return HasPassedArea(point, direction, m_chaseExtents);
    }

    public bool HasPassedArea(Vector2 point, float direction, Vector2 areaExtents)
    {
        if (direction > 0f)
            return point.x > (position.x + areaExtents.x);
        else if (direction < 0f)
            return point.x < (position.x - areaExtents.x);

        return IsInArea(point, areaExtents);
    }

    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, new Vector3(m_patrolExtents.x, m_patrolExtents.y, 0.1f) * 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(position, new Vector3(m_chaseExtents.x, m_chaseExtents.y, 0.1f) * 2f);
    }
    #endregion
}
