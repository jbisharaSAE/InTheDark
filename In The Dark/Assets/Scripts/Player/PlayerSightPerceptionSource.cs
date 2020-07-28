using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles how the players character is percepted (sight) by the AI
/// </summary>
public class PlayerSightPerceptionSource : ShadowAreaListener, ISightPerceptible
{
    [SerializeField] private List<string> m_layersToIgnoreWhenHideen = new List<string>();      // Layers to ignore when hidden

    private bool m_cachedHidden = false;    // If currently hidden
    private short m_numSeenBy = 0;          // Number of times we have been seen
    private bool m_canGoInvis = false;      // If can go hidden (invis) at this time

    [SerializeField] private HealthComponent m_healthComp = null;                   // Owners health component
    [SerializeField] private AdvancedCharacterMovement m_movementComp = null;       // Owners movement component

    void Awake()
    {
        if (!m_healthComp)
            m_healthComp = GetComponent<HealthComponent>();

        if (!m_movementComp)
            m_movementComp = GetComponent<AdvancedCharacterMovement>();
    }

    void Update()
    {
        if (GameManager.isPaused)
            return;

        UpdateInvisState();
    }

    public bool CanBeDetected()
    {
        // Ignore us if we have died
        if (m_healthComp && m_healthComp.isDead)
            return false;

        return !m_cachedHidden;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void OnVisibleToSightPerception(SightPerception sightPerception)
    {
        ++m_numSeenBy;
        UpdateInvisState();
    }

    public void OnNotVisibleToSightPerception(SightPerception sightPerception)
    {
        --m_numSeenBy;
        UpdateInvisState();
    }

    protected override void OnEnterShadows()
    {
        UpdateInvisState();
    }

    protected override void OnLeftShadows()
    {
        UpdateInvisState();
    }

    private void UpdateInvisState()
    {
        if (m_healthComp && m_healthComp.isDead)
        {
            m_canGoInvis = false;
            m_cachedHidden = false;
            return;
        }

        bool canGoInvis = CheckInvisState();
        if (canGoInvis != m_canGoInvis)
        {
            if (canGoInvis)
                OnBecomeHidden();
            else
                OnBecomeVisible();

            m_canGoInvis = canGoInvis;
        }

        m_cachedHidden = canGoInvis;
    }

    private bool CheckInvisState()
    {
        // Fully visible while dashing
        if (m_movementComp && m_movementComp.isDashing)
            return false;

        // If already seen by a enemy, we should be visible to all enemies
        if (m_numSeenBy > 0)
            return false;

        return inShadows;
    }

    protected virtual void OnBecomeHidden()
    {
        UpdateLayerCollisions(true);
    }

    protected virtual void OnBecomeVisible()
    {
        UpdateLayerCollisions(false);
    }

    private void UpdateLayerCollisions(bool ignore)
    {
        foreach (string layerName in m_layersToIgnoreWhenHideen)
        {
            int layerId = LayerMask.NameToLayer(layerName);
            if (layerId >= 0)
                Physics2D.IgnoreLayerCollision(gameObject.layer, layerId, ignore);
        }
    }
}
