using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles how the players character is percepted (sight) by the AI
/// </summary>
// TODO: Rename this to something bettert
public class PlayerSightPerceptionSource : ShadowAreaListener, ISightPerceptible
{
    private short m_numSeenBy = 0;          // Number of times we have been seen

    [SerializeField] private HealthComponent m_healthComp = null;       // Owners health component

    public bool CanBeDetected()
    {
        // Ignore us if we have died
        if (m_healthComp && m_healthComp.isDead)
            return false;

        // If already seen by a enemy, we should be visible to all enemies
        return m_numSeenBy > 0 || !inShadows;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void OnVisibleToSightPerception(SightPerception sightPerception)
    {
        ++m_numSeenBy;
    }

    public void OnNotVisibleToSightPerception(SightPerception sightPerception)
    {
        --m_numSeenBy;
    }

    protected override void OnEnterShadows()
    {
        // This is testing (TODO: stuff like this we would only want to do when in the shadows while not detected)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            Color color = renderer.material.color;
            color.a = 0.7f;
            renderer.material.color = color;
        }
    }

    protected override void OnLeftShadows()
    {
        // This is testing (TODO: stuff like this we would only want to do when in the shadows while not detected)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            Color color = renderer.material.color;
            color.a = 1f;
            renderer.material.color = color;
        }
    }
}
