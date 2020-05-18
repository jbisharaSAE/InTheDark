using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetSelector : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;
    [SerializeField] private SightPerception m_sightPerception = null;
    [SerializeField] private float m_updateRate = 0.2f;

    private GameObject m_selectedTarget = null;

    /// <summary>
    /// Target that has been selected for enemy to attack
    /// </summary>
    public GameObject target { get { return m_selectedTarget; } }

    void Start()
    {
        if (m_sightPerception)
            m_sightPerception.OnPercpetionUpdated += OnSightPerceptionUpdated;
    }

    /// <summary>
    /// Updates the selected target, uses SightPerception.GetClosestSeenObjectTo by default
    /// </summary>
    /// <returns>Valid game object or null</returns>
    private GameObject UpdateClosetTarget()
    {
        // TODO: We might want to have an event for when targets switch and such.
        // In that case, we can call the event here when we detect a difference

        m_selectedTarget = null;
        if (m_sightPerception)
            m_selectedTarget = m_sightPerception.GetClosestSeenObjectTo(transform.position);

        return m_selectedTarget;
    }

    /// <summary>
    /// Routine for updating the selected target. Changes should be uncommon
    /// thus why we run in a routine instead of Update()
    /// </summary>
    private IEnumerator UpdateClosestTargetRoutine()
    { 
        while (m_sightPerception)
        {
            UpdateClosetTarget();
            yield return new WaitForSeconds(m_updateRate);
        }
    }

    /// <summary>
    /// Notify from sight perception that an object has either been seen or lost
    /// </summary>
    /// <param name="detectedObject">Object whose perception has been updated</param>
    /// <param name="nowVisible">If object is now visible or not</param>
    private void OnSightPerceptionUpdated(GameObject detectedObject, bool nowVisible)
    {
        // If here, we assume sight perception should be valid
        if (nowVisible)
        {
            StartCoroutine(UpdateClosestTargetRoutine());
            if (m_animator)
                m_animator.SetBool("HasTarget", true);

            UpdateClosetTarget();
        }
        // TODO: Need to do a 'time before we forget so we don't instantly forget our target'
        else if (m_sightPerception.numSeenObjects == 0)
        {
            // Assuming only the coroutine this function started is running
            StopAllCoroutines();
            if (m_animator)
                m_animator.SetBool("HasTarget", false);

            m_selectedTarget = null;
        }
    }
}
