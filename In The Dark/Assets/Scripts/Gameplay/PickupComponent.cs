using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component acts as the root for an object that can be picked up
/// </summary>
public class PickupComponent : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore triggers
        if (collision.isTrigger)
            return;

        // TODO: Check if player is one picking us up
        // (or better, use layers so only the player can pick this up)       

        OnPickedUp(collision);
    }

    /// <summary>
    /// This event is called whenever a valid object has picked us up
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnPickedUp(Collider2D collision)
    {

    }
}
