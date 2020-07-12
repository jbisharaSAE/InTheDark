using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [System.Serializable]
    class LevelData
    {
        public string m_levelName = string.Empty;
        public Button m_levelButton = null;

        public void UpdateButtonStatus(NinjasSaveData saveData)
        {
            if (!m_levelButton)
                return;

            bool unlockedLevel = saveData.m_unlockedLevels.Contains(m_levelName);
            m_levelButton.interactable = unlockedLevel;
        }
    }

    [SerializeField] private List<LevelData> m_levelsData = new List<LevelData>();          // List of all the levels that can be played

    private NinjasSaveData m_saveData = null;       // Loaded save data

    void OnEnable()
    {
        m_saveData = NinjasSaveData.Load();

        // No save data exist
        if (m_saveData == null)
        {
            // Create new save data, save first level as current and unlocked
            m_saveData = new NinjasSaveData();

            if (m_levelsData.Count > 0)
                m_saveData.m_unlockedLevels.Add(m_levelsData[0].m_levelName);

            m_saveData.Save();
        }

        for (int i = 0; i < m_levelsData.Count; ++i)
            m_levelsData[i].UpdateButtonStatus(m_saveData);
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
            if (m_levelsData.Count > 0)
                levelName = m_levelsData[0].m_levelName;
        }

        if (GameManager.OpenLevel(levelName))
        {
            m_saveData.m_currentLevel = levelName;
            m_saveData.Save();
        }
    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < m_levelsData.Count)
        {
            LevelData data = m_levelsData[levelIndex];
            if (GameManager.OpenLevel(data.m_levelName))
            {
                m_saveData.m_currentLevel = data.m_levelName;
                m_saveData.Save();
            }
        }
    }
}
