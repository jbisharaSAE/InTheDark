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
    [SerializeField] private GameObject m_pauseMenuPrefab = null;       // Prefab for pause menu to spawn
    [SerializeField] private GameObject m_gameOverMenuPrefab = null;    // Prefab for game over menu to spawn

    private GameObject m_instancedHUD = null;       // Instance of current HUD being displayed
    private int m_instancedHUDPrefabHash = 0;       // Hashcode of prefab used for spawning instanced HUD

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
        // Second, should handle this in player controller, not HUD
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.TogglePause();
    }

    /// <summary>
    /// Displays the pause menu
    /// </summary>
    public void DisplayPauseScreen()
    {
        DisplayHUD(m_pauseMenuPrefab);
    }

    /// <summary>
    /// Hides the pause menu, re-activating the primary Game HUD
    /// </summary>
    public void HidePauseScreen()
    {
        if (IsHUDInstanced(m_pauseMenuPrefab))
            ShowGameHUD();
    }

    /// <summary>
    /// Displays the game over menu
    /// </summary>
    public void DisplayGameOverScreen()
    {
        DisplayHUD(m_gameOverMenuPrefab);
    }

    /// <summary>
    /// Hides the game over menu, re-activating the primary Game HUD
    /// </summary>
    public void HideGameOverScreen()
    {
        if (IsHUDInstanced(m_gameOverMenuPrefab))
            ShowGameHUD();
    }

    /// <summary>
    /// Displays the HUD specified by given prefab.
    /// This will hide the primary game HUD
    /// </summary>
    /// <param name="prefab">Prefab of HUD to display</param>
    private void DisplayHUD(GameObject prefab)
    {
        if (!prefab)
            return;

        // HUD might already be instanced, if so we don't need to do anything
        if (IsHUDInstanced(prefab))
            return;

        DestroyHUD();
        SetGameHUDVisible(false);

        SpawnHUD(prefab);
    }

    /// <summary>
    /// Shows the primary game HUD, hiding any instanced HUD
    /// </summary>
    private void ShowGameHUD()
    {
        DestroyHUD();
        SetGameHUDVisible(true);
    }

    /// <summary>
    /// Spawns the HUD based on a prefab
    /// </summary>
    /// <param name="prefab">Prefab of HUD to display</param>
    /// <returns>New HUD object or null</returns>
    private GameObject SpawnHUD(GameObject prefab)
    {
        if (!prefab)
            return null;
   
        m_instancedHUD = Instantiate(prefab);
        m_instancedHUDPrefabHash = prefab.GetHashCode();

        return m_instancedHUD;
    }

    /// <summary>
    /// Destroys the current instanced HUD if it exists
    /// </summary>
    private void DestroyHUD()
    {
        if (!m_instancedHUD)
            return;

        Destroy(m_instancedHUD);

        m_instancedHUD = null;
        m_instancedHUDPrefabHash = 0;
    }

    /// <summary>
    /// If the prefab has been instanced to use as the HUD
    /// </summary>
    /// <param name="prefab">Prefab to check</param>
    /// <returns>If instance of prefab exists</returns>
    private bool IsHUDInstanced(GameObject prefab)
    {
        if (!prefab || !m_instancedHUD)
            return false;

        return m_instancedHUDPrefabHash == prefab.GetHashCode();
    }

    /// <summary>
    /// Sets the visibility of the primary game HUD
    /// </summary>
    /// <param name="visible">If HUD should be visible</param>
    private void SetGameHUDVisible(bool visible)
    {
        if (m_gameHUD)
            m_gameHUD.SetActive(visible);
    }
}
