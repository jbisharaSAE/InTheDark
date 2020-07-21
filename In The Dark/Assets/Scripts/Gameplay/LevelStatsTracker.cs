using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component tracks the stats for a level. In order to track stats,
/// StartTracking() must be called, use StopTracking() to stop tracking
/// </summary>
public class LevelStatsTracker : MonoBehaviour
{
    /// <summary>
    /// Singleton access to LevelStatsTracker. May not be valid
    /// </summary>
    public static LevelStatsTracker instance { get; private set; }

    [SerializeField] private bool autoStartTracking = false;        // If to automatically start tracking on Start()

    /// <summary>
    /// If current tracking stats of level playthrough
    /// </summary>
    public bool isTracking { get; private set; }

    private float m_timerElapsedCombined = 0f;  // Combined elapsed time taken to complete level between Start/Stop
    private float m_timerStart = -1f;           // When level timer was last enabled

    private GeneralStatsData m_statsData = new GeneralStatsData();      // General stats collected (not including time)

    /// <summary>
    /// Current time player has played current level that counts.
    /// This is real time, as in Time.TimeScale does not effect this.
    /// This does not track when the game is paused
    /// </summary>
    public float levelPlayTime { get { return GetTotalElapsedTime(); } }

    /// <summary>
    /// General stats that have been collected this playthrough
    /// </summary>
    public GeneralStatsData generalStats { get { return m_statsData; } }

    void Awake()
    {
        if (instance != null)
        {
            autoStartTracking = false;

            Destroy(this);
            return;
        }

        instance = this;
    }

    void Start()
    {
        if (autoStartTracking)
            StartTracking();
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void StartTracking()
    {
        if (isTracking)
            return;

        m_timerStart = Time.unscaledTime;

        RegisterCallbacks();

        isTracking = true;
    }

    public void StopTracking()
    {
        if (!isTracking)
            return;

        UnregisterCallbacks();

        // Check timer start here, this prevents time being added twice
        // if pausing the game then calling stop tracking
        if (m_timerStart >= 0f)
        {
            m_timerElapsedCombined += Time.unscaledTime - m_timerStart;
            m_timerStart = -1f;
        }

        m_statsData.m_playTime = m_timerElapsedCombined;

        isTracking = false;
    }

    private void RegisterCallbacks()
    {
        GameManager.onPaused += OnGamePaused;
        EnemyScript.onEnemyDeath += OnEnemyDeath;
    }

    private void UnregisterCallbacks()
    {
        EnemyScript.onEnemyDeath -= OnEnemyDeath;
        GameManager.onPaused -= OnGamePaused;
    }

    private void OnGamePaused(bool paused)
    {
        if (!isTracking)
            return;

        if (paused)
        {
            m_timerElapsedCombined += Time.unscaledTime - m_timerStart;
            m_timerStart = -1f;
        }
        else
        {
            m_timerStart = Time.unscaledTime;
        }
    }

    private void OnEnemyDeath(EnemyScript enemy)
    {
        m_statsData.m_enemyKills++;
    }

    private float GetTotalElapsedTime()
    {
        float time = m_timerElapsedCombined;
        if (isTracking)
            time += Time.time - m_timerStart;

        return time;
    }

    public void ReplaceIfBetterTime(LevelStatsData dataToReplace)
    {
        float time = GetTotalElapsedTime();
        if (time < dataToReplace.m_completionTime)
        {
            dataToReplace.m_completionTime = time;
        }
    }
}
