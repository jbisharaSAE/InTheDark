using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    /// <summary>
    /// Restarts the current game session
    /// </summary>
    public void RestartSession()
    {
        GameManager.RestartSession();
    }
}
