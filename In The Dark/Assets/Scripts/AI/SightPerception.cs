using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface required for objects to be detected by the sight system
/// </summary>
public interface ISightPerceptible
{
    /// <summary>
    /// Get the transform that needs to be visible to the perception component in
    /// order for the object implementing this interface to be seen
    /// </summary>
    Transform GetTransform();
}

/// <summary>
/// This component can be used 
/// </summary>
public class SightPerception : MonoBehaviour
{
    [SerializeField, Range(0f, 360f)] private float m_fieldOfView = 120f;       // Rangle of angle to check for
    [SerializeField, Min(0f)] private float m_fieldDistance = 5f;               // Max distance at which objects can be seen
    [SerializeField] private LayerMask m_broadLayers = Physics2D.AllLayers;     // Collision layers for finding potential objects in sight
    [SerializeField] private LayerMask m_visibleLayers = Physics2D.AllLayers;   // Collision layers for checking if in direct sight

    // Event that is called when the sight perception of an object is updated
    public delegate void OnObjectPerceptionUpdated(GameObject detectedObject, bool nowVisible);
    public OnObjectPerceptionUpdated OnPercpetionUpdated;

    private HashSet<GameObject> m_objectsInSight = new HashSet<GameObject>();       // Game objects that are visible (must have collision)

    void Update()
    {
        HashSet<GameObject> oldObjectsInSight = new HashSet<GameObject>(m_objectsInSight);

        Collider2D[] targets = GetPotentialObjectsInSight();
        if (targets != null)
        {
            foreach (Collider2D target in targets)
            {
                ISightPerceptible perceptable = target.GetComponent<ISightPerceptible>();
                if (perceptable != null && CanSeePerctableObject(perceptable))
                {
                    // Returns true if being added (false if already present)
                    if (m_objectsInSight.Add(target.gameObject))
                        if (OnPercpetionUpdated != null)
                            OnPercpetionUpdated.Invoke(target.gameObject, true);

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
        }
    }

    /// <summary>
    /// Gets all the objects that may potentially be in our sight
    /// </summary>
    /// <returns>All objects with potential to be seen</returns>
    private Collider2D[] GetPotentialObjectsInSight()
    {
        // TODO: Maybe just do what we do with the rendering, we we jump in strades (but we still
        // check if we can see the transform specified by the interface)
        return Physics2D.OverlapCircleAll(transform.position, m_fieldDistance, m_broadLayers);
    }


    private bool CanSeePerctableObject(ISightPerceptible perceptible)
    {
        Transform targetToCheck = perceptible.GetTransform();

        // No transform provided means we see it by default
        if (targetToCheck == null)
            return true;

        Vector2 origin = transform.position;
        Vector2 target = targetToCheck.position;

        // Collider we detected might be in range, but actual view transform is not
        Vector2 displacement = target - origin;
        if (displacement.sqrMagnitude > (m_fieldDistance * m_fieldDistance))
            return false;

        // Make sure view transform is actually within our field of view
        // TODO: this isn't 100% accurate and I'm not sure why
        //float dot = Vector2.Dot(displacement.normalized, transform.right);
        //if (dot < 1f - (m_fieldOfView / 360f))
        //    return false;

        // Temp solution, need to look into why the above isn't 100% accurate
        displacement.Normalize();
        float rad = Mathf.Abs(Mathf.Atan2(displacement.y, displacement.x));
        if (rad > (m_fieldOfView * 0.5f * Mathf.Deg2Rad))
            return false;

        // Make sure nothing is blocking the way
        RaycastHit2D hitResult = Physics2D.Linecast(transform.position, targetToCheck.position, m_visibleLayers);
        return hitResult.transform == targetToCheck;
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 origin = transform.position;
        float fovRad = m_fieldOfView * Mathf.Deg2Rad;
        float start = Mathf.Atan2(transform.right.y, transform.right.x) - (fovRad * 0.5f);

        // Visual representation of what we can see

        float jump = fovRad / 12f;
        for (int i = 0; i < 12; ++i)
        {
            float curAngle = start + (jump * i);
            Vector2 dir = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * m_fieldDistance;
            Vector3 startPoint = origin + new Vector3(dir.x, dir.y, 0f);

            // Draw side of cone
            if (i == 0)
                Gizmos.DrawLine(origin, startPoint);

            curAngle += jump;
            dir = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * m_fieldDistance;
            Vector3 endPoint = origin + new Vector3(dir.x, dir.y, 0f);

            // Draw size of cone
            if (i == 11)
                Gizmos.DrawLine(origin, endPoint);

            Gizmos.DrawLine(startPoint, endPoint);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            foreach (GameObject seenObject in m_objectsInSight)
                Gizmos.DrawWireSphere(seenObject.transform.position, 0.1f);
        }
    }
    #endregion
}
