using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A list of useful functions and constant values used throughout the code base
/// </summary>
public struct Helpers
{
    /// <summary>
    /// Get the flip rotation (in euler angles) based on a float value.
    /// Side is on the X axis and determine where we are compared to the origin
    /// </summary>
    /// <param name="side">Value to determine rotation</param>
    /// <returns>Rotation in euler angles (in world space)</returns>
    public static Vector3 FlipRotation(float side)
    {
        if (side >= 0f)
            return new Vector3(0f, 0f, 0f);
        else
            return new Vector3(0f, 180f, 0f);
    }
}

