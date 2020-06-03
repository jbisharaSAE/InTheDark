using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles the players HUD. This doesn't mean the Canvas
/// directly, but any UI widget that could be displayed
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    /// <summary>
    /// Singleton access to players HUD, may not be valid
    /// </summary>
    public static PlayerHUD instance { get; private set; }

    [SerializeField] private GameObject m_gameHUD = null;               // Object with core Game HUD
    [SerializeField] private PauseMenu m_pauseMenuPrefab = null;        // Prefab for pause menu to spawn

    private PauseMenu m_pauseMenu = null;       // Reference to instance of Pause Menu

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate PlayerHUD instantiated. This instance will not be used as singleton instance", this);
            return;
        }

        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    void Update()
    {
        // Only update if primary instance
        if (this != instance)
            return;

        // TODO: First, make an input binding
        // Second, we should handle this in a central GameManager?
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_pauseMenu)
                HidePauseScreen();
            else
                DisplayPauseScreen();
        }
    }

    public void DisplayPauseScreen()
    {
        if (m_pauseMenu || !m_pauseMenuPrefab)
            return;

        SetGameHUDVisible(false);
        m_pauseMenu = Instantiate(m_pauseMenuPrefab);

        // TODO: we should handle this in a central GameManager?
        // Scripts could then use GameManager.isPaused to check if they should update
        // We basically are 'Paused' if the menu was created (though the game should be pausable even without the HUD)
        if (m_pauseMenu)
            Time.timeScale = 0f;
    }

    public void HidePauseScreen()
    {
        if (!m_pauseMenu)
            return;

        Destroy(m_pauseMenu.gameObject);
        m_pauseMenu = null;

        SetGameHUDVisible(true);

        // TODO: we should handle this in a central GameManager?
        // Scripts could then use GameManager.isPaused to check if they should update
        Time.timeScale = 1f;
    }

    private void SetGameHUDVisible(bool visible)
    {
        if (m_gameHUD)
            m_gameHUD.SetActive(visible);
    }
}
