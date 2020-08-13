using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Event for when the game is paused
    /// </summary>
    /// <param name="isPaused">If game is paused/unpaused</param>
    public delegate void OnGamePaused(bool isPaused);
    public static OnGamePaused onPaused;

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
    public static bool isInputDisabled { get { return instance ? instance.m_disableInput : false; } }

    private bool m_isPaused = false;            // If game is currently paused
    private float m_prevTimeScale = 1f;         // Time scale before game was paused
    private bool m_disableInput = false;        // If input is requested to be ignored

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
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

        if (onPaused != null)
            onPaused.Invoke(true);
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

        if (onPaused != null)
            onPaused.Invoke(false);
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

    /// <summary>
    /// Opens the campaign level at specified index.
    /// </summary>
    /// <param name="levelIndex">Index of campaign level to open</param>
    /// <returns>If level was requested to load</returns>
    public static bool OpenCampaignLevel(int levelIndex)
    {
        CampaignConfig config = CampaignConfig.GetConfig();
        if (!config)
            return false;

        return OpenLevel(config.GetLevelName(levelIndex));
    }

    /// <summary>
    /// Opens a level
    /// </summary>
    /// <param name="levelName">Name of the level</param>
    /// <returns>If level was requested to load</returns>
    public static bool OpenLevel(string levelName)
    {
        if (levelName == string.Empty)
            return false;

        SceneManager.LoadScene(levelName);
        return true;
    }

    // temp here, for testing (maybe keep here)
    public static bool FinishCampaignLevel()
    {
        CampaignConfig config = CampaignConfig.GetConfig();
        if (!config)
            return false;

        string currentLevel = SceneManager.GetActiveScene().name;
        int curLevelIndex = config.GetLevelIndex(currentLevel);
        if (curLevelIndex == -1)
            return false;

        // next level
        int nextLevelIndex = curLevelIndex + 1;
        if (!config.IsValidLevelIndex(nextLevelIndex))
            // TODO: End of campaign
            return false;

        // TODO: Would need to make sure level is actually valid (we can open the scene)
        // If not, just iterate to the next level (also need to make sure that game isn't finished)

        CampaignConfig.UnloadConfig();

        // Save the game
        {
            NinjasSaveData saveData = NinjasSaveData.Load(true);

            LevelStatsTracker levelStatsTracker = LevelStatsTracker.instance;
            if (levelStatsTracker)
            {
                levelStatsTracker.StopTracking();

                // for now, just do this here
                saveData.generalStats.m_levelsCompleted++;

                saveData.generalStats.Combine(levelStatsTracker.generalStats);

                LevelStatsData levelStats = saveData.getLevelStatsData(curLevelIndex, true);
                levelStatsTracker.ReplaceIfBetterTime(levelStats);
            }

            saveData.SetLevelUnlocked(nextLevelIndex);
            saveData.SetLastPlayedLevel(nextLevelIndex);
            saveData.Save();
        }

        return OpenLevel(config.GetLevelName(nextLevelIndex));
    }

    public static bool RestartCampaignLevel(bool gameOver)
    {
        CampaignConfig config = CampaignConfig.GetConfig();
        if (!config)
            return false;

        // Make sure this is actually a campaign level
        string currentLevel = SceneManager.GetActiveScene().name;
        int curLevelIndex = config.GetLevelIndex(currentLevel);
        if (curLevelIndex == -1)
            return false;

        CampaignConfig.UnloadConfig();

        // Save the game
        {
            NinjasSaveData saveData = NinjasSaveData.Load(true);

            LevelStatsTracker levelStatsTracker = LevelStatsTracker.instance;
            if (levelStatsTracker)
            {
                levelStatsTracker.StopTracking();

                // for now, just do it here (the death)
                if (gameOver)
                    saveData.generalStats.m_numDeaths++;

                saveData.generalStats.Combine(levelStatsTracker.generalStats);
            }

            saveData.Save();
        }

        // for now
        return OpenLevel(config.GetLevelName(curLevelIndex));
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
