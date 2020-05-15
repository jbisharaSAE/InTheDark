using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be added to any game object that should
/// be able to be seen by uses of the SightPerception component
/// </summary>
public class SightPerceptionSource : MonoBehaviour, ISightPerceptible
{
    public Transform GetTransform()
    {
        return transform;
    }
}
