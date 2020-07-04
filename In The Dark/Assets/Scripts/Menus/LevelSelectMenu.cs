using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private List<string> m_levelNames = new List<string>();        // List of all the levels that can be played

    public void ContinueProgress()
    {

    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < m_levelNames.Count)
        {
            string levelName = m_levelNames[levelIndex];
            GameManager.OpenLevel(levelName);
        }
    }
}
