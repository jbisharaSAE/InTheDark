using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles displaying a user prompt to the 
/// </summary>
public class UserPrompt : MonoBehaviour
{
    /// <summary>
    /// The result user has selected
    /// </summary>
    public enum UserPromptResult
    {
        Ok,
        Cancel
    };

    [SerializeField] private bool m_autoCenter = true;          // If prompt should center itself to middle of canvas

    // Event that is called once the user has made a selection
    public delegate void OnUserMadeSelection(UserPromptResult result);
    public OnUserMadeSelection OnSelectionMade;

    /// <summary>
    /// Confirms and closes with the Ok response
    /// </summary>
    public void Ok()
    {
        ConfirmAndClose(UserPromptResult.Ok);
    }

    /// <summary>
    /// Confirms and closes with the Cancel response
    /// </summary>
    public void Cancel()
    {
        ConfirmAndClose(UserPromptResult.Cancel);
    }

    /// <summary>
    /// Confirms and closes the user prompt, firing off the event.
    /// This will also destroy the prompt, removing it from screen
    /// </summary>
    /// <param name="result">Result of users choice</param>
    private void ConfirmAndClose(UserPromptResult result)
    {
        if (OnSelectionMade != null)
            OnSelectionMade.Invoke(result);

        Destroy(gameObject);
    }

    /// <summary>
    /// Instantiates and automatically adds a prompt to screen
    /// </summary>
    /// <param name="prefab">Prompt to instantiate</param>
    /// <param name="selectionCallback">Event to call upon confirmation</param>
    /// <returns>User prompt or null</returns>
    public static UserPrompt DisplayPrompt(UserPrompt prefab, OnUserMadeSelection selectionCallback)
    {
        Canvas canvas = GetPrimaryCanvas();
        if (!canvas)
            return null;

        UserPrompt newPrompt = Instantiate(prefab, canvas.transform);
        if (!newPrompt)
            return null;

        newPrompt.InitializePrompt(selectionCallback);
        return newPrompt;
    }

    /// <summary>
    /// Finds the canvas that prompts should be added to
    /// </summary>
    /// <returns>Canvas or null</returns>
    private static Canvas GetPrimaryCanvas()
    {
        return FindObjectOfType<Canvas>();
    }

    /// <summary>
    /// Initializes the prompt after having been created
    /// </summary>
    /// <param name="selectionCallback">Event to call upon confirmation</param>
    private void InitializePrompt(OnUserMadeSelection selectionCallback)
    {
        // Hook the callback
        if (selectionCallback != null)
            OnSelectionMade += selectionCallback;

        if (m_autoCenter)
        {
            RectTransform trans = transform as RectTransform;
            if (trans)
            {
                // Place in center of screen
                trans.anchorMin = new Vector2(0.5f, 0.5f);
                trans.anchorMax = new Vector2(0.5f, 0.5f);
                trans.pivot = new Vector2(0.5f, 0.5f);

                // Revert back to middle
                trans.anchoredPosition = Vector2.zero;
            }
        }
    }
}
