using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UserPrompt m_quitPrompt = null;        // User prompt to display when attempting to quit

    [SerializeField] private List<GameObject> m_menus = new List<GameObject>();     // The list of menus that can be switched to
    [SerializeField] private int m_startMenuIndex = 0;                              // Index of menu to first display

    void Start()
    {
        OpenMenu(m_startMenuIndex);
    }

    /// <summary>
    /// Opens menun specified by index. If index is invalid, hides all the menus
    /// </summary>
    /// <param name="index">Index of menu to open</param>
    public void OpenMenu(int index)
    {
        // Simply interating for two reasons.
        // 1. We don't expect many elements in the list
        // 2. We want to disable the other menus so non are overlapping
        for (int i = 0; i < m_menus.Count; ++i)
        {
            GameObject menu = m_menus[i];
            if (menu)
            {
                bool enable = i == index;
                menu.SetActive(enable);
            }      
        }
    }

    /// <summary>
    /// Quits the application, but will first display a user prompt to confirm
    /// </summary>
    public void QuitGame()
    {
        if (!UserPrompt.DisplayPrompt(m_quitPrompt, OnQuitGameConfirmed))
            Application.Quit();
    }

    /// <summary>
    /// Notify that user has made a selection on the prompt
    /// </summary>
    /// <param name="result"></param>
    private void OnQuitGameConfirmed(UserPrompt.UserPromptResult result)
    {
        if (result == UserPrompt.UserPromptResult.Ok)
            Application.Quit();
    }
}
