using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be used by any object that wants
/// to be notified when it enters a shadowy area
/// </summary>
public class ShadowAreaListener : MonoBehaviour
{
    private byte m_numAreas = 0;            // Number of shadow areas we are in

    /// <summary>
    /// If this object is in the shadows
    /// </summary>
    public bool inShadows { get { return m_numAreas > 0; } }

    /// <summary>
    /// Notify from a ShadowArea component that this object has now entered the shadows
    /// </summary>
    /// <param name="shadowArea">Shadow area instigating event</param>
    public void NotifyEnterShadow(ShadowArea shadowArea)
    {
        ++m_numAreas;
        if (m_numAreas == 1)
            OnEnterShadows();
    }

    /// <summary>
    /// Notify from a ShadowArea component that this object has left the shadows
    /// </summary>
    /// <param name="shadowArea">Shadow area instigating event</param>
    public void NotifyLeftShadow(ShadowArea shadowArea)
    {
        --m_numAreas;
        if (m_numAreas == 0)
            OnLeftShadows();
    }

    /// <summary>
    /// Event that is called when entering shadows. 
    /// Not called everytime a new area is entered
    /// </summary>
    protected virtual void OnEnterShadows()
    {

    }

    /// <summary>
    /// Event that is called when exiting shadows.
    /// Not called everytime an old area is exited
    /// </summary>
    protected virtual void OnLeftShadows()
    {

    }
}
