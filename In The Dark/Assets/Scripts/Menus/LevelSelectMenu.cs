using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [System.Serializable]
    class LevelData
    {
        public int m_levelIndex = -1;
        public Button m_levelButton = null;

        public void UpdateButtonStatus(NinjasSaveData saveData)
        {
            if (!m_levelButton)
                return;

            bool unlockedLevel = saveData.HasUnlockedLevelAtIndex(m_levelIndex);
            m_levelButton.interactable = unlockedLevel;
        }
    }

    [SerializeField] private List<LevelData> m_levelsData = new List<LevelData>();          // List of all the levels that can be played

    private NinjasSaveData m_saveData = null;       // Loaded save data
    private CampaignConfig m_config = null;         // Loaded campaign config

    void Update()
    {
        // Cheat, to unlock all levels
        if (Input.GetKeyDown(KeyCode.M))
        {
            m_saveData.SetLevelUnlocked(m_config.GetNumLevels());
            m_saveData.Save();

            for (int i = 0; i < m_levelsData.Count; ++i)
                m_levelsData[i].UpdateButtonStatus(m_saveData);
        }
    }

    void OnEnable()
    {
        m_saveData = NinjasSaveData.Load();

        // Create new save data if none exists
        if (m_saveData == null)
            m_saveData = new NinjasSaveData();

        for (int i = 0; i < m_levelsData.Count; ++i)
            m_levelsData[i].UpdateButtonStatus(m_saveData);

        m_config = CampaignConfig.GetConfig();
    }

    void OnDisable()
    {
        CampaignConfig.UnloadConfig();

        m_saveData = null;
        m_config = null;
    }

    public void ContinueProgress()
    {
        if (m_saveData == null || m_config == null)
            return;

        // Zero as default, as 0 signals start a new game
        int levelIndex = 0;
        if (!m_saveData.IsNewGame())
            levelIndex = m_saveData.currentLevelIndex;

        // Is this level even valid?
        if (!m_config.IsValidLevelIndex(levelIndex))
            return;

        TryOpenLevelAndSave(levelIndex);
    }

    public void SelectLevel(int levelIndex)
    {
        if (m_saveData == null || m_config == null)
            return;

        // Have we unlocked this level?
        if (!m_saveData.HasUnlockedLevelAtIndex(levelIndex))
            return;

        // Is this level even valid?
        if (!m_config.IsValidLevelIndex(levelIndex))
            return;

        TryOpenLevelAndSave(levelIndex);
    }

    public bool TryOpenLevelAndSave(int levelIndex)
    {
        if (GameManager.OpenCampaignLevel(levelIndex))
        {
            m_saveData.SetLevelUnlocked(levelIndex);
            m_saveData.SetLastPlayedLevel(levelIndex);
            m_saveData.Save();

            return true;
        }

        return false;
    }
}
