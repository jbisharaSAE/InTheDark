using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface required for objects to be detected by the sight system
/// </summary>
public interface ISightPerceptible
{
    /// <summary>
    /// If the object can be seen at this time. Returning false
    /// will consider the object 'invisible' to the sight perception
    /// </summary>
    bool CanBeDetected();

    /// <summary>
    /// Get the transform that needs to be visible to the perception component in
    /// order for the object implementing this interface to be seen
    /// </summary>
    Transform GetTransform();

    void OnVisibleToSightPerception(SightPerception sightPerception);
    void OnNotVisibleToSightPerception(SightPerception sightPerception);
}

/// <summary>
/// This component can be used 
/// </summary>
public class SightPerception : MonoBehaviour
{
    // TODO: Change fov to be between -1 and 1
    [SerializeField, Range(0f, 360f)] private float m_fieldOfView = 120f;       // Range of angle to check for
    [SerializeField, Range(0f, 360f)] private float m_loseFieldOfView = 150f;   // Range of angle for checking already perception objects
    [SerializeField, Min(0f)] private float m_fieldDistance = 5f;               // Max distance at which objects can be seen
    [SerializeField, Min(0f)] private float m_loseFieldDistance = 10f;          // Distance objects much reach for perception is lost
    [SerializeField] private LayerMask m_broadLayers = Physics2D.AllLayers;     // Collision layers for finding potential objects in sight
    [SerializeField] private LayerMask m_visibleLayers = Physics2D.AllLayers;   // Collision layers for checking if in direct sight

    // Event that is called when the sight perception of an object is updated
    public delegate void OnObjectPerceptionUpdated(GameObject detectedObject, bool nowVisible);
    public OnObjectPerceptionUpdated OnPercpetionUpdated;

    private HashSet<GameObject> m_objectsInSight = new HashSet<GameObject>();       // Game objects that are visible (must have collision)

    /// <summary>
    /// Get the number of objects currently percepted
    /// </summary>
    public int numSeenObjects { get { return m_objectsInSight.Count; } }

    void Update()
    {
        // We want to ignore triggers for our sight checks
        // We assume right now that any events we call, if querying, will also want to ignore them
        bool queriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        HashSet<GameObject> oldObjectsInSight = new HashSet<GameObject>(m_objectsInSight);

        Collider2D[] targets = GetPotentialObjectsInSight();
        if (targets != null)
        {
            foreach (Collider2D target in targets)
            {
                ISightPerceptible perceptable = target.GetComponent<ISightPerceptible>();
                if (perceptable == null || !perceptable.CanBeDetected())
                {
                    continue;
                }

                if (CanSeePerctableObject(perceptable, target.gameObject))
                {
                    // Returns true if being added (false if already present)
                    if (m_objectsInSight.Add(target.gameObject))
                    {
                        if (OnPercpetionUpdated != null)
                            OnPercpetionUpdated.Invoke(target.gameObject, true);

                        perceptable.OnVisibleToSightPerception(this);
                    }

                    oldObjectsInSight.Remove(target.gameObject);
                }
            }
        }

        // These objects are no longer in sight, notify listeners
        foreach (GameObject lostObject in oldObjectsInSight)
        {
            if (!m_objectsInSight.Remove(lostObject))
                continue;

            // This could arise if an object is deleted (remove from level) while on our sight (I think)
            if (lostObject == null)
                continue;
       
            if (OnPercpetionUpdated != null)
                OnPercpetionUpdated.Invoke(lostObject, false);

            // Possible that component that implemented interface is no longer present
            ISightPerceptible perceptable = lostObject.GetComponent<ISightPerceptible>();
            if (perceptable != null)
                perceptable.OnNotVisibleToSightPerception(this);
        }

        Physics2D.queriesHitTriggers = queriesHitTriggers;
    }

    void OnDestroy()
    {
        foreach (GameObject gameObject in m_objectsInSight)
        {
            if (gameObject == null)
                continue;

            // Possible that component that implemented interface is no longer present
            ISightPerceptible perceptable = gameObject.GetComponent<ISightPerceptible>();
            if (perceptable != null)
                perceptable.OnNotVisibleToSightPerception(this);
        }
    }

    /// <summary>
    /// Gets all the objects that may potentially be in our sight
    /// </summary>
    /// <returns>All objects with potential to be seen</returns>
    private Collider2D[] GetPotentialObjectsInSight()
    {
        float distance = m_fieldDistance > m_loseFieldDistance ? m_fieldDistance : m_loseFieldDistance;

        // TODO: Maybe just do what we do with the rendering, we we jump in strades (but we still
        // check if we can see the transform specified by the interface)
        return Physics2D.OverlapCircleAll(transform.position, distance, m_broadLayers);
    }

    /// <summary>
    /// Checks if the game object is in our sights. Will handle if it already has been
    /// seen and should use the alternative values
    /// </summary>
    /// <param name="perceptible">Valid percitable reference</param>
    /// <param name="gameObject">Game object that owns interface</param>
    /// <returns>If object can be seen</returns>
    private bool CanSeePerctableObject(ISightPerceptible perceptible, GameObject gameObject)
    {
        Transform targetToCheck = perceptible.GetTransform();

        // No transform means object should be considered invisible
        if (targetToCheck == null)
            return false;

        Vector2 origin = transform.position;
        Vector2 target = targetToCheck.position;

        float fovToCheck = m_fieldOfView;
        float distanceToCheck = m_fieldDistance;
        if (m_objectsInSight.Contains(gameObject))
        {
            fovToCheck = m_loseFieldOfView;
            distanceToCheck = m_loseFieldDistance;
        }

        // Collider we detected might be in range, but actual view transform is not
        Vector2 displacement = target - origin;
        if (displacement.sqrMagnitude > (distanceToCheck * distanceToCheck))
            return false;

        // Make sure view transform is actually within our field of view
        float dot = Vector2.Dot(displacement.normalized, transform.right);
        if (dot < Mathf.Cos(fovToCheck * 0.5f * Mathf.Deg2Rad))
            return false;

        // Make sure nothing is blocking the way
        RaycastHit2D hitResult = Physics2D.Linecast(transform.position, targetToCheck.position, m_visibleLayers);
        return hitResult.transform == targetToCheck;
    }

    /// <summary>
    /// Get a seen game object that is closest to a point
    /// </summary>
    /// <param name="point">Point to query with</param>
    /// <returns>Valid object or null</returns>
    public GameObject GetClosestSeenObjectTo(Vector2 point)
    {
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject seenObject in m_objectsInSight)
        {
            float distanceSqr = ((Vector2)seenObject.transform.position - point).sqrMagnitude;
            if (distanceSqr < closestDistance)
            {
                closest = seenObject;
                closestDistance = distanceSqr;
            }
        }

        return closest;
    }

    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawCone(m_fieldOfView, m_fieldDistance);

        Gizmos.color = Color.magenta;
        DrawCone(m_loseFieldOfView, m_loseFieldDistance);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            foreach (GameObject seenObject in m_objectsInSight)
                Gizmos.DrawWireSphere(seenObject.transform.position, 0.1f);
        }
    }

    /// <summary>
    /// Helper for drawing a cone which represents our field of view
    /// </summary>
    /// <param name="distance">Distance of cone</param>
    private void DrawCone(float fov, float distance)
    {
        Vector3 origin = transform.position;
        float fovRad = fov * Mathf.Deg2Rad;
        float start = Mathf.Atan2(transform.right.y, transform.right.x) - (fovRad * 0.5f);

        float jump = fovRad / 12f;
        for (int i = 0; i < 12; ++i)
        {
            float curAngle = start + (jump * i);
            Vector2 dir = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * distance;
            Vector3 startPoint = origin + new Vector3(dir.x, dir.y, 0f);

            // Draw side of cone
            if (i == 0)
                Gizmos.DrawLine(origin, startPoint);

            curAngle += jump;
            dir = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * distance;
            Vector3 endPoint = origin + new Vector3(dir.x, dir.y, 0f);

            // Draw size of cone
            if (i == 11)
                Gizmos.DrawLine(origin, endPoint);

            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
    #endregion
}
