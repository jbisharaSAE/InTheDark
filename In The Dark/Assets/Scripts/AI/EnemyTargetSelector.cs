﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetSelector : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;
    [SerializeField] private SightPerception m_sightPerception = null;
    [SerializeField] private float m_updateRate = 0.2f;

    public bool m_focusSightOnTarget = false;

    private GameObject m_selectedTarget = null;
    private GameObject m_overrideTarget = null;
    private Coroutine m_updateTargetRoutine = null;
    private float m_originalSightRotation = 0f;

    /// <summary>
    /// Target that has been selected for enemy to attack
    /// </summary>
    public GameObject target { get { return overrideTarget ? overrideTarget : m_selectedTarget; } }

    /// <summary>
    /// Override target that takes priority over selected target
    /// </summary>
    public GameObject overrideTarget { get { return m_overrideTarget; } set { SetOverrideTarget(value); } }

    void Start()
    {
        if (m_sightPerception)
        {
            m_sightPerception.OnPercpetionUpdated += OnSightPerceptionUpdated;
            m_originalSightRotation = m_sightPerception.transform.localEulerAngles.z;
        }
    }

    void Update()
    {
        if (!m_focusSightOnTarget)
            return;

        if (!m_sightPerception || !m_selectedTarget)
            return;

        Transform sightTransform = m_sightPerception.transform;

        // Update sight component to face current target
        Vector2 dir = (m_selectedTarget.transform.position - sightTransform.position).normalized;
        sightTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
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
    /// thus why we run in a routine instead of calling this in Update()
    /// </summary>
    private IEnumerator UpdateClosestTargetRoutine()
    {
        while (m_sightPerception)
        {
            UpdateClosetTarget();
            yield return new WaitForSeconds(m_updateRate);
        }

        m_updateTargetRoutine = null;
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
            if (m_updateTargetRoutine == null)
            {
                m_updateTargetRoutine = StartCoroutine(UpdateClosestTargetRoutine());
            }

            // This most likely will not have been set
            if (m_animator)
                m_animator.SetBool("HasTarget", true);

            UpdateClosetTarget();
        }
        else if (m_sightPerception.numSeenObjects == 0)
        {
            m_selectedTarget = null;
            if (m_animator)
                m_animator.SetBool("HasTarget", m_overrideTarget != null);

            if (m_sightPerception)
                m_sightPerception.transform.localEulerAngles = new Vector3(0f, 0f, m_originalSightRotation);
        }
    }

    private void SetOverrideTarget(GameObject overrideTarget)
    {
        m_overrideTarget = overrideTarget;

        if (m_animator)
            m_animator.SetBool("HasTarget", target != null);
    }
}
