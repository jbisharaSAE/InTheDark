using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidBody;           // Rigid body to move
    [SerializeField] private Transform[] m_waypoints;           // Waypoints of path
    [SerializeField] private float m_speed = 5f;                // Speed of movement
    [SerializeField] private bool m_reverse = false;            // If to traverse waypoints in reverse order

    private int m_curWaypoint = -1;             // Index of waypoint to travel from

    /// <summary>
    /// If platform has potential waypoints to travel to. This does not
    /// check if the transform references are valid, only if elements 'exist'
    /// </summary>
    public bool HasWaypoints { get { return m_waypoints != null && m_waypoints.Length > 0; } }

    void Awake()
    {
        if (!m_rigidBody)
            m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (HasWaypoints)
            m_curWaypoint = 0;
    }

    void FixedUpdate()
    {
        if (!HasWaypoints)
            return;

        // TODO: This does not handle reaching more than one waypoint per update, this
        // could be put into a recursive function or a while loop but for testing it just does max
        // one at a time

        int nextWaypoint = GetNextWaypointIndex(m_curWaypoint);
        Transform target = GetWaypoint(nextWaypoint);
        if (!target)
            return;

        float step = m_speed * Time.fixedDeltaTime;

        Vector2 currentPosition = m_rigidBody.position;
        Vector2 targetPosition = target.position;
        Vector2 displacement = targetPosition - currentPosition;
        float distance = displacement.magnitude;

        Vector2 newPosition = currentPosition;

        if (step <= distance)
        {
            newPosition += displacement.normalized * step;
        }
        else
        {
            m_curWaypoint = nextWaypoint;

            nextWaypoint = GetNextWaypointIndex(m_curWaypoint);
            target = GetWaypoint(nextWaypoint);
            if (!target)
                return;

            currentPosition = targetPosition;
            targetPosition = target.position;
            displacement = targetPosition - currentPosition;

            newPosition += displacement.normalized * (step - distance);
        }

        m_rigidBody.MovePosition(newPosition);
    }

    /// <summary>
    /// Get the transform of a waypoint
    /// </summary>
    /// <param name="index">Index of waypoint</param>
    /// <returns>Valid transform or null</returns>
    private Transform GetWaypoint(int index)
    {
        if (!HasWaypoints)
            return null;

        if (index >= 0 && index < m_waypoints.Length)
            return m_waypoints[index];

        return null;
    }

    /// <summary>
    /// Get index of waypoint to travel to, this handles if travelling in reverse
    /// </summary>
    /// <param name="index">Index of source waypoint</param>
    /// <returns>Valid index or -1</returns>
    private int GetNextWaypointIndex(int index)
    {
        if (!HasWaypoints)
            return -1;

        if (m_reverse)
        {
            if (index <= 0)
                return m_waypoints.Length - 1;
            else
                return index - 1;
        }
        else
        {
            if (index >= (m_waypoints.Length - 1))
                return 0;
            else
                return index + 1;
        }
    }
}
