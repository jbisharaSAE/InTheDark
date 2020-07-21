using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that contains configuration for the general campaign.
/// This assumes that the config is located at /Assets/Resources/CampaignConfig.asset
/// </summary>
[CreateAssetMenu]
public class CampaignConfig : ScriptableObject
{
    public static readonly string ConfigFileName = "CampaignConfig";

    // Config loaded from resources
    private static CampaignConfig campaignConfig = null;

    // Name of all the levels. This needs to be the names of the actual level files
    [SerializeField]
    private List<string> m_levelNames = new List<string>();

    public static CampaignConfig GetConfig()
    {
        if (campaignConfig)
            return campaignConfig;

        CampaignConfig config = (CampaignConfig)Resources.Load(ConfigFileName, typeof(CampaignConfig));
        if (!config)
        {
            Debug.LogWarning("No Campaign Config file was found at \"/Assets/Resources/\". Please create new config");
            return (CampaignConfig)CreateInstance(typeof(CampaignConfig));
        }

        campaignConfig = config;
        return campaignConfig;
    }

    public static void UnloadConfig()
    {
        if (campaignConfig)
            Resources.UnloadAsset(campaignConfig);

        campaignConfig = null;
    }

    public string GetLevelName(int levelIndex)
    {
        if (m_levelNames == null)
            return string.Empty;

        if (levelIndex >= 0 && levelIndex < m_levelNames.Count)
            return m_levelNames[levelIndex];

        return string.Empty;
    }

    public int GetLevelIndex(string levelName)
    {
        if (m_levelNames == null)
            return -1;

        for (int i = 0; i < m_levelNames.Count; ++i)
            if (m_levelNames[i] == levelName)
                return i;

        return -1;
    }

    public int GetNumLevels()
    {
        if (m_levelNames != null)
            return m_levelNames.Count;

        return 0;
    }

    public bool IsValidLevelIndex(int levelIndex)
    {
        if (levelIndex < 0 || m_levelNames == null)
            return false;

        if (levelIndex >= m_levelNames.Count)
            return false;

        return m_levelNames[levelIndex] != string.Empty;
    }
}
