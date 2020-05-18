using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolArea : MonoBehaviour
{
    [SerializeField] private Transform m_patrolAreaOrigin = null;   // Workaround
    [SerializeField] private Vector2 m_areaSize = new Vector2(10f, 1f);

    public Vector2 position { get { return m_patrolAreaOrigin ? m_patrolAreaOrigin.position : transform.position; } }

    /// <summary>
    /// Checks if given point is in the patrol area (only checks X axis)
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <returns>If inside this area</returns>
    public bool IsInArea(Vector2 point)
    {
        // TODO: Would probably want to check Y to, but for now we only need X
        Vector2 displacement = point - position;
        return Mathf.Abs(displacement.x) <= m_areaSize.x;
    }

    /// <summary>
    /// Checks if some point has passed area based on a travel direction (on the X axis)
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="direction">Direction of movement (on X axis)</param>
    /// <returns>If section has been passed</returns>
    public bool HasPassedArea(Vector2 point, float direction)
    {
        if (direction > 0f)
            return point.x > (position.x + m_areaSize.x);
        else if (direction < 0f)
            return point.x < (position.x - m_areaSize.x);

        return IsInArea(point);
    }

    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, new Vector3(m_areaSize.x, m_areaSize.y, 0.1f) * 2f);
    }
    #endregion
}
