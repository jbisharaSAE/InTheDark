using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UserPrompt m_quitPrompt = null;        // User prompt to display when attempting to quit

    /// <summary>
    /// Resumes the current game session
    /// </summary>
    public void ResumeSession()
    {
        GameManager.Unpause();
    }

    /// <summary>
    /// Quits the current game session, but will first display user prompt to confirm
    /// </summary>
    public void QuitSession()
    {
        if (!UserPrompt.DisplayPrompt(m_quitPrompt, OnQuitSessionConfirmed))
            SceneManager.LoadScene(0);  // Assuming Main Menu is scene 0
    }

    /// <summary>
    /// Notify that user has made a selection on the prompt when attempting to quit
    /// </summary>
    /// <param name="result"></param>
    private void OnQuitSessionConfirmed(UserPrompt.UserPromptResult result)
    {
        if (result == UserPrompt.UserPromptResult.Ok)
            SceneManager.LoadScene(0);  // Assuming Main Menu is scene 0
    }
}
