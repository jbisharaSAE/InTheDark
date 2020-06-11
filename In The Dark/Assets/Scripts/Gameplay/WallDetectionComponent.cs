using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component will detect if there are walls on either side that are blocking the path
/// </summary>
public class WallDetectionComponent : MonoBehaviour
{
    [System.Flags]
    private enum DetectedWalls
    {
        None = 0,
        Left = 1,
        Right = 2
    };

    [SerializeField] private Collider2D m_collider = null;

    public LayerMask m_worldLayers = Physics2D.AllLayers;
    public float m_detectionOffset = 1f;

    private DetectedWalls m_detectedWalls = DetectedWalls.None;

    public bool leftWallDetected { get { return (m_detectedWalls & DetectedWalls.Left) > 0; } }

    public bool rightWallDetected { get { return (m_detectedWalls & DetectedWalls.Right) > 0; } }

    public bool wallDetected { get { return m_detectedWalls != DetectedWalls.None; } }
     
    void Awake()
    {
        if (!m_collider)
            m_collider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        FindLeftWall();
        FindRightWall();
    }

    private void FindLeftWall()
    {
        Vector2 size = Vector2.one;
        if (m_collider != null)
            size = m_collider.bounds.size * 0.5f;

        UpdateWallDetection((Vector2)transform.position + Vector2.left * m_detectionOffset, size, DetectedWalls.Left);
    }

    private void FindRightWall()
    {
        Vector2 size = Vector2.one;
        if (m_collider != null)
            size = m_collider.bounds.size * 0.5f;

        UpdateWallDetection((Vector2)transform.position + Vector2.right * m_detectionOffset, size, DetectedWalls.Right);
    }

    private void UpdateWallDetection(Vector2 origin, Vector2 size, DetectedWalls wallToDetect)
    {
        if (DetectWall(origin, size))
            m_detectedWalls |= wallToDetect;
        else
            m_detectedWalls &= ~wallToDetect;
    }

    private bool DetectWall(Vector2 origin, Vector2 size)
    {
        Collider2D collider = Physics2D.OverlapBox(origin, size, 0f, m_worldLayers);
        if (collider != null)
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Collider2D collider = m_collider;
        if (!collider)
            collider = GetComponent<Collider2D>();

        Vector2 size = Vector2.one;
        if (collider)
            size = collider.bounds.size;

        Gizmos.color = new Color(0.4f, 1f, 0.2f);
        Gizmos.DrawWireCube(transform.position + (Vector3.left * m_detectionOffset), size);
        Gizmos.DrawWireCube(transform.position + (Vector3.right * m_detectionOffset), size);
    }
}
