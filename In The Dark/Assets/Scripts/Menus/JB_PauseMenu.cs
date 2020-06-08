using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_PauseMenu : MonoBehaviour
{
    public GameObject pauseObj;
    public bool bToggle;

    private void Start()
    {
        pauseObj = this.gameObject;
    }

    public void TogglePauseMenu()
    {
        bToggle = !bToggle;
        if (bToggle)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        pauseObj.SetActive(bToggle);
    }
}
