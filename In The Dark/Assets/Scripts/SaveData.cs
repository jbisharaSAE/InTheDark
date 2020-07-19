using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Stats tracked per level playthrough
[System.Serializable]
public class LevelStatsData
{
    // Time it took player to complete the level
    [SerializeField]
    public float m_completionTime = float.MaxValue;
}

// Stats general from general play
[System.Serializable]
public class GeneralStatsData
{
    // Total playtime
    [SerializeField]
    public float m_playTime = 0f;

    // Amount of levels completed
    [SerializeField]
    public int m_levelsCompleted = 0;

    // Total amount of enemies killed
    [SerializeField]
    public int m_enemyKills = 0;

    // Total amount of times the player has died
    [SerializeField]
    public int m_numDeaths = 0;

    public void Combine(GeneralStatsData other)
    {
        m_playTime += other.m_playTime;
        m_levelsCompleted += other.m_levelsCompleted;
        m_enemyKills += other.m_enemyKills;
        m_numDeaths += other.m_numDeaths;
    }
}

[System.Serializable]
public class NinjasSaveData
{
    // Index of furthest campaign level unlocked
    [SerializeField]
    private int m_campaignProgressIndex = -1;
    
    // Index of current level being played
    [SerializeField]
    private int m_currentLevelIndex = -1;

    // Stats for individual levels
    [SerializeField]
    private List<LevelStatsData> m_levelStats = new List<LevelStatsData>();

    // Stats from all gameplay
    [SerializeField]
    private GeneralStatsData m_generalStats = new GeneralStatsData();

    public int campaignProgressIndex { get { return m_campaignProgressIndex; } }
    public int currentLevelIndex { get { return m_currentLevelIndex; } }
    public GeneralStatsData generalStats { get { return m_generalStats; } }

    public void Save()
    {
        string fullPath = GetFullPath();
        FileStream stream = new FileStream(fullPath, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, this);

        stream.Close();
    }

    public static NinjasSaveData Load(bool createNewIfRequired = false)
    {
        string fullPath = GetFullPath();
        if (File.Exists(fullPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullPath, FileMode.Open);

            NinjasSaveData saveData = formatter.Deserialize(stream) as NinjasSaveData;
            stream.Close();

            if (saveData.m_generalStats == null)
                saveData.m_generalStats = new GeneralStatsData();

            return saveData;
        }
        else if (createNewIfRequired)
        {
            return new NinjasSaveData();
        }

        return null;
    }

    private static string GetFullPath()
    {
        string partialPath = Application.persistentDataPath;
        return partialPath + "/save.fun";
    }

    public bool HasUnlockedLevelAtIndex(int levelIndex)
    {
        // If less than zero, means New Game is awaiting
        if (IsNewGame())
            return false;

        return levelIndex <= m_campaignProgressIndex;
    }

    public bool IsLastPlayedLevel(int levelIndex)
    {
        // If less than zero, means New Game is awaiting
        if (IsNewGame())
            return false;

        return levelIndex == m_currentLevelIndex;
    }

    public bool IsNewGame()
    {
        return m_campaignProgressIndex < 0;
    }

    public void SetLevelUnlocked(int levelIndex)
    {
        levelIndex = Mathf.Max(levelIndex, -1);
        m_campaignProgressIndex = Mathf.Max(m_campaignProgressIndex, levelIndex);
    }

    public void SetLastPlayedLevel(int levelIndex)
    {
        levelIndex = Mathf.Max(levelIndex, -1);
        m_currentLevelIndex = levelIndex;
    }

    public LevelStatsData getLevelStatsData(int levelIndex, bool createIfNeeded)
    {
        if (!createIfNeeded)
        {
            if (levelIndex < m_levelStats.Count)
                return m_levelStats[levelIndex];
            else
                return null;
        }

        // Fill up level stats, use null to signal that no actual stats exist
        while (levelIndex >= m_levelStats.Count)
            m_levelStats.Add(null);

        LevelStatsData statsData = m_levelStats[levelIndex];
        if (statsData == null)
        {
            statsData = new LevelStatsData();
            m_levelStats[levelIndex] = statsData;
        }

        return statsData;
    }
}