using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathHandler : MonoBehaviour
{
    [SerializeField] private HealthComponent m_bossHealthComp = null;
    [SerializeField] private float m_transitionAfter = 4f;

    void Start()
    {
        if (m_bossHealthComp)
            m_bossHealthComp.OnDeath += OnBossDeath;
    }

    private void OnBossDeath(HealthComponent self)
    {
        if (m_transitionAfter > 0f)
            Invoke("TransitionToLevel", m_transitionAfter);
        else
            TransitionToLevel();
    }

    private void TransitionToLevel()
    {
        GameManager.FinishCampaignLevel();
    }
}
