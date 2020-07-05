using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private List<string> m_levelNames = new List<string>();        // List of all the levels that can be played

    private NinjasSaveData m_saveData = null;       // Loaded save data

    void OnEnable()
    {
        m_saveData = NinjasSaveData.Load();

        // No save data exist
        if (m_saveData == null)
            m_saveData = new NinjasSaveData();
    }

    void OnDisable()
    {
        m_saveData = null;
    }

    public void ContinueProgress()
    {
        if (m_saveData == null)
            return;

        // Might be dealing with a "New Game)
        string levelName = m_saveData.m_currentLevel;
        if (levelName == string.Empty)
        {
            if (m_levelNames.Count > 0)
                levelName = m_levelNames[0];
        }

        if (GameManager.OpenLevel(levelName))
        {
            m_saveData.m_currentLevel = levelName;
            m_saveData.Save();
        }
    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < m_levelNames.Count)
        {
            string levelName = m_levelNames[levelIndex];
            if (GameManager.OpenLevel(levelName))
            {
                m_saveData.m_currentLevel = levelName;
                m_saveData.Save();
            }
        }
    }
}
