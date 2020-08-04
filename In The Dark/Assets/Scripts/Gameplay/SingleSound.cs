using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SingleSound : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource = null;

    public AudioClip clip { set { if (m_audioSource) m_audioSource.clip = value; } }

    void Awake()
    {
        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        if (!m_audioSource)
            m_audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        PlaySingleTime();
    }

    private void PlaySingleTime()
    {
        if (!m_audioSource.clip)
            return;

        Invoke("FinishedPlayingAudio", m_audioSource.clip.length);
    }

    private void FinishedPlayingAudio()
    {
        Destroy(gameObject);
    }

    public static void PlaySingleSound(AudioClip clip)
    {
        if (!clip)
            return;

        SingleSound singleSound = new GameObject().AddComponent<SingleSound>();
        singleSound.clip = clip;

        singleSound.PlaySingleTime();
    }
}
