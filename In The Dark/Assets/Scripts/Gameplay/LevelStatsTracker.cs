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
    /// If current tracking stats of level playthrough
    /// </summary>
    public bool isTracking { get; private set; }

    private float m_timerElapsedCombined = 0f;  // Combined elapsed time taken to complete level between Start/Stop
    private float m_timerStart = -1f;           // When level timer was last enabled

    /// <summary>
    /// Current time player has played current level that counts.
    /// This is real time, as in Time.TimeScale does not effect this.
    /// This does not track when the game is paused
    /// </summary>
    public float levelPlayTime { get { return GetTotalElapsedTime(); } }

    public void StartTracking()
    {
        if (isTracking)
            return;

        m_timerStart = Time.unscaledTime;

        RegisterCallbacks();
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
    }

    private void RegisterCallbacks()
    {
        GameManager.onPaused += OnGamePaused;
    }

    private void UnregisterCallbacks()
    {
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

    private float GetTotalElapsedTime()
    {
        float time = m_timerElapsedCombined;
        if (isTracking)
            time += Time.time - m_timerStart;

        return time;
    }
}
