using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is used by AI to detect if they are about to walk into a wall
/// </summary>
public class WallDetectionComponent : MonoBehaviour
{
    public LayerMask m_worldLayers = Physics2D.AllLayers;
    public float m_detectionDistance = 1.5f;
    public float m_slopeLimit = 0.5f;
     
    public bool IsAtWall(float dir)
    {
        dir = Mathf.Sin(dir);
        if (Mathf.Approximately(dir, 0f))
            return false;

        Vector2 direction = new Vector2(dir, 0f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, m_detectionDistance, m_worldLayers);
        if (hits != null && hits.Length > 0)
            foreach (RaycastHit2D hit in hits)
            {
                // Hitting ourselves, don't count as wall
                if (transform.IsChildOf(hit.transform))
                    continue;

                float dot = Vector2.Dot(hit.normal, Vector2.up);
                if (Mathf.Abs(dot) < m_slopeLimit)
                    return true;
            }

        return false;
    }
}
