using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton access to Game Manager. May not be valid
    /// </summary>
    public static GameManager instance { get; private set; }
    
    /// <summary>
    /// Check if the game has been paused via the game manager
    /// </summary>
    public static bool isPaused { get { return instance ? instance.m_isPaused : false; } }

    /// <summary>
    /// Check if input has been requested to be disabled
    /// </summary>
    public static bool inputDisabled { get { return instance ? instance.m_disableInput : false; } }

    private bool m_isPaused = false;            // If game is currently paused
    private float m_prevTimeScale = 1f;         // Time scale before game was paused
    private bool m_disableInput = false;        // If input is requested to be ignored

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize prime instance
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            m_prevTimeScale = 1f;
            UnpauseImpl();

            instance = null;
        }
    }

    void OnActiveSceneChanged(Scene from, Scene to)
    {
        // Make sure that game isn't paused
        UnpauseImpl();

        // Revert any changes that had been to time scale
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Pauses the game (if not already)
    /// </summary>
    static public void Pause()
    {
        if (instance)
            instance.PauseImpl();
    }

    /// <summary>
    /// Unpauses the game (if not already)
    /// </summary>
    static public void Unpause()
    {
        if (instance)
            instance.UnpauseImpl();
    }

    /// <summary>
    /// Toggles if the game should/shouldn't be paused
    /// </summary>
    static public void TogglePause()
    {
        if (isPaused)
            Unpause();
        else
            Pause();
    }

    /// <summary>
    /// Implementation for pausing the game
    /// </summary>
    private void PauseImpl()
    {
        if (m_isPaused)
            return;

        m_isPaused = true;

        // Closest thing to actual pause where nothing should be updated.
        // (We depend on scripts using GameManager.isPaused to really stop updates)
        m_prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (PlayerHUD.instance)
            PlayerHUD.instance.DisplayPauseScreen();
    }

    /// <summary>
    /// Implementation for unpausing the game
    /// </summary>
    private void UnpauseImpl()
    {
        if (!m_isPaused)
            return;

        m_isPaused = false;

        // Rever the 'pausing' of the game
        Time.timeScale = m_prevTimeScale;
        m_prevTimeScale = 1f;

        if (PlayerHUD.instance)
            PlayerHUD.instance.HidePauseScreen();
    }

    /// <summary>
    /// If game input should be disabled globally (
    /// </summary>
    /// <param name="disable">If to disable input</param>
    public static void SetInputDisabled(bool disable)
    {
        if (instance)
            instance.SetInputDisabledImpl(disable);
    }

    /// <summary>
    /// Implementation for disabling input
    /// </summary>
    private void SetInputDisabledImpl(bool disable)
    {
        m_disableInput = disable;
    }

    public static bool OpenLevel(string levelName)
    {
        if (instance)
            return instance.OpenLevelImpl(levelName);

        return false;
    }

    private bool OpenLevelImpl(string levelName)
    {
        SceneManager.LoadScene(levelName);
        return true;
    }

    /// <summary>
    /// Restarts the current session (basically reloading the current scene)
    /// </summary>
    public static void RestartSession()
    {
        if (instance)
            instance.RestartSessionImpl();
    }

    /// <summary>
    /// Implementation for restarting current session
    /// </summary>
    private void RestartSessionImpl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
