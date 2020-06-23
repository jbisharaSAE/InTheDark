using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component defines the routine for an enemy
/// </summary>
public class GuardLookoutRoutine : MonoBehaviour
{
    [System.Serializable]
    protected class RoutineEntry
    {
        [Range(-180f, 180f)] public float m_faceDirection = 0f;     // The direction (in degrees) to look towards
        [Min(0f)] public float m_inspectTime = 4f;                  // How long to remain inspecting this entry

        /// <summary>
        /// Helper function for converting face direction to a vector
        /// </summary>
        /// <returns>Normalized vector</returns>
        public Vector2 FaceDirectorVector()
        {
            float rad = Mathf.Deg2Rad * m_faceDirection;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    };

    [SerializeField] private List<RoutineEntry> m_routineEntries = new List<RoutineEntry>();    // All entries of the lookout routine
    [SerializeField] private int m_currentEntry = 0;                                            // Index of current entry in routine

    public Transform m_lookTransform = null;        // Transform to rotate to match face direction
    public float m_lookSpeed = 20f;                 // Speed at which to face entries face direction
    public Transform m_orientationTransform = null; // Transform to rotate to properly match face direction orientation

    private RoutineEntry m_entry = null;        // Reference to current entry
    private float m_entryElapsedTime = 0f;      // Elapsed time of current entry, manually tracked to support pausing

    void Awake()
    {
        if (!m_lookTransform)
            m_lookTransform = transform;
    }

    void Start()
    {
        if (isActiveAndEnabled)
            ResumeRoutine();
    }

    void Update()
    {
        if (m_entry == null)
        {
            PauseRoutine();
            return;
        }

        m_entryElapsedTime += Time.deltaTime;

        if (m_entryElapsedTime >= m_entry.m_inspectTime)
            IterateEntry();

        // Update the look transform to match face direction
        Transform lookTrans = m_lookTransform ? m_lookTransform : transform;
        Vector3 eulerAngles = lookTrans.eulerAngles;
        eulerAngles.x = eulerAngles.y = 0f; // Issue with Y being 180f 
        eulerAngles.z = Mathf.MoveTowardsAngle(eulerAngles.z, m_entry.m_faceDirection, m_lookSpeed);

        // Update the guards orientation
        if (m_orientationTransform && m_orientationTransform != lookTrans)
        {
            float side = Mathf.Cos(eulerAngles.z * Mathf.Deg2Rad);
            m_orientationTransform.eulerAngles = Helpers.FlipRotation(side);
        }

        // Need to do this last, we want to set this is world space after
        // having potentially flipped the orientation transform (which 
        // look trans could be a child of)
        lookTrans.eulerAngles = eulerAngles;
    }

    /// <summary>
    /// Resumes the lookout routine from when last paused
    /// </summary>
    public void ResumeRoutine()
    {
        enabled = true;

        if (m_entry == null)
        {
            m_currentEntry = Mathf.Clamp(m_currentEntry, 0, m_routineEntries.Count - 1);
            if (m_currentEntry >= m_routineEntries.Count)
            {
                PauseRoutine();
                return;
            }

            m_entry = m_routineEntries[m_currentEntry];

            // Have us immediately rotate to face desired direction
            // Update the look transform to match face direction
            Transform lookTrans = m_lookTransform ? m_lookTransform : transform;
            Vector3 eulerAngles = lookTrans.eulerAngles;
            eulerAngles.x = eulerAngles.y = 0f; // Issue with Y being 180f 
            eulerAngles.z = m_entry.m_faceDirection;

            // Update the guards orientation
            if (m_orientationTransform && m_orientationTransform != lookTrans)
            {
                float side = Mathf.Cos(eulerAngles.z * Mathf.Deg2Rad);
                m_orientationTransform.eulerAngles = Helpers.FlipRotation(side);
            }

            // Need to do this last, we want to set this is world space after
            // having potentially flipped the orientation transform (which 
            // look trans could be a child of)
            lookTrans.eulerAngles = eulerAngles;
        }
    }

    public void ResumeRoutineClosestTo(Vector2 direction)
    {
        int bestIndex = -1;
        float bestDot = -2f;

        for (int i = 0; i < m_routineEntries.Count; ++i)
        {
            RoutineEntry entry = m_routineEntries[i];

            float dot = Vector2.Dot(m_routineEntries[i].FaceDirectorVector(), direction);
            if (dot > bestDot)
            {
                bestIndex = i;
                bestDot = dot;
            }
        }

        if (bestIndex != -1)
        {
            m_entry = null;
            m_currentEntry = bestIndex;
            m_entryElapsedTime = 0f;
        }

        ResumeRoutine();
    }

    /// <summary>
    /// Pauses the current lookout routine
    /// </summary>
    public void PauseRoutine()
    {
        enabled = false;
    }

    /// <summary>
    /// Iterates to the next entry in the routine. Will handle
    /// need to loop back to the start if required
    /// </summary>
    private void IterateEntry()
    {
        if (m_routineEntries.Count == 0)
            m_currentEntry = 0;
        else
            m_currentEntry = (m_currentEntry + 1) % m_routineEntries.Count;
        
        if (m_currentEntry < m_routineEntries.Count)
        {
            m_entry = m_routineEntries[m_currentEntry];
            m_entryElapsedTime = 0f;
        }
        else
        {
            PauseRoutine();
        }
    }
}
