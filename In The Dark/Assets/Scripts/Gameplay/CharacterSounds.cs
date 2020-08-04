using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private HealthComponent m_healthComp = null;
    [SerializeField] private AudioSource m_healthChangeAudioSource = null;
    [SerializeField] private AudioClip m_healAudio = null;
    [SerializeField] private AudioClip m_hurtAudio = null;
    [SerializeField, Range(0f, 1f)] private float m_hurtPitchRange = 0.2f;

    void Awake()
    {
        if (m_healthComp)
            m_healthComp.OnHealthChanged += OnHealthChanged;
    }

    void OnDestroy()
    {
        if (m_healthComp)
            m_healthComp.OnHealthChanged -= OnHealthChanged;
    }

    void OnHealthChanged(HealthComponent self, float newHealth, float delta)
    {
        if (self.isDead)
            return;

        // Positive delta = heal, Negative delta = damage
        if (delta > 0f)
            PlayHealthChangeAudio(m_healAudio, 0f);
        else
            PlayHealthChangeAudio(m_hurtAudio, m_hurtPitchRange);
    }

    private void PlayHealthChangeAudio(AudioClip clip, float pitchRange)
    {
        if (!m_healthChangeAudioSource || !clip)
            return;

        m_healthChangeAudioSource.Stop();

        m_healthChangeAudioSource.clip = clip;
        m_healthChangeAudioSource.loop = false;

        m_healthChangeAudioSource.pitch = 1f;
        if (pitchRange > 0f)
            m_healthChangeAudioSource.pitch += Random.Range(-pitchRange, pitchRange);

        m_healthChangeAudioSource.Play();      
    }
}
