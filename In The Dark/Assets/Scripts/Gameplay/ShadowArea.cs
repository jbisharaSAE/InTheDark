using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be used to signify a shadowy area.
/// Works via using a trigger colliding and a specialized component
/// </summary>
public class ShadowArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        ShadowAreaListener listener = collision.GetComponent<ShadowAreaListener>();
        if (listener)
            listener.NotifyEnterShadow(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        ShadowAreaListener listener = collision.GetComponent<ShadowAreaListener>();
        if (listener)
            listener.NotifyLeftShadow(this); 
    }
}
