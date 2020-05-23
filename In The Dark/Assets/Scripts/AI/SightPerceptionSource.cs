using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be added to any game object that should
/// be able to be seen by uses of the SightPerception component
/// </summary>
public class SightPerceptionSource : MonoBehaviour, ISightPerceptible
{
    public bool m_isVisible = true;     // If this object is visible now

    public bool CanBeDetected()
    {
        return isActiveAndEnabled ? m_isVisible : false;
    }

    public Transform GetTransform()
    {
        return isActiveAndEnabled ? transform : null;
    }
    public void OnVisibleToSightPerception(SightPerception sightPerception)
    {

    }

    public void OnNotVisibleToSightPerception(SightPerception sightPerception)
    {

    }
}
