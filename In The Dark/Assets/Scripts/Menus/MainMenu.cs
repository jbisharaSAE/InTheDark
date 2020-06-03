using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UserPrompt m_quitPrompt = null;        // User prompt to display when attempting to quit

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
