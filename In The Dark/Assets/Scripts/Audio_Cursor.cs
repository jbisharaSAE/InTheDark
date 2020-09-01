using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Cursor : MonoBehaviour
{
    [SerializeField] private Texture2D mouseCursorTexture;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    private AudioSource m_audioSource;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        Cursor.SetCursor(mouseCursorTexture, Vector2.zero, CursorMode.ForceSoftware);

        DontDestroyOnLoad(gameObject);

        OnLevelWasLoaded(0);
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 0)
        {
            if(menuMusic != null)
                m_audioSource.clip = menuMusic;

            m_audioSource.Play();
        }
        else
        {
            if(gameMusic != null)
                m_audioSource.clip = gameMusic;

            m_audioSource.Play();
        }
    }


}
