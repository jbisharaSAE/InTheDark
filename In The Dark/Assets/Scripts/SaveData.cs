using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class LevelStatsData
{
    // Time it took player to complete the level
    [SerializeField]
    public float m_completionTime = -1f;
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

    public int campaignProgressIndex { get { return m_campaignProgressIndex; } }
    public int currentLevelIndex { get { return m_currentLevelIndex; } }

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
}