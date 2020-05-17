using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The 'core' of the simple melee enemy. Interacts with various
/// components that are required to run the AI state machine
/// </summary>
public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private mod.StateMachine m_stateMachine;       // State machine (TODO: Replace with animator)
    [SerializeField] private SightPerception m_sightComponent;      // Our eyes of the world

    [Header("States")]
    public string m_chaseStateName = "Chase";
    public string m_postChaseState = "PostChase";

    private GameObject m_enemyToAttack = null;          // The enemy in sight and to attack
    private Coroutine m_forgetRoutine = null;           // Routine run when lost sight of the enemy (so we don't immediately stop chasing)

    /// <summary>
    /// Get the target to attack (can be null)
    /// </summary>
    public GameObject targetEnemy { get { return m_enemyToAttack; } }

    void Start()
    {
        if (m_sightComponent)
            m_sightComponent.OnPercpetionUpdated += OnSightPerceptionUpdated;
    }

    private void OnSightPerceptionUpdated(GameObject detectedObject, bool nowVisible)
    {
        // TODO: Right now we assume the only thing we will ever be able to see
        // is the player. This wouldn't work correctly if we were able to see multiple things (at the same time that is)
        if (nowVisible)
        {
            if (m_forgetRoutine != null)
            {
                StopCoroutine(m_forgetRoutine);
                m_forgetRoutine = null;
            }

            m_enemyToAttack = detectedObject;
            if (m_stateMachine)
                m_stateMachine.EnterState(m_chaseStateName);
        }
        else
        {
            m_forgetRoutine = StartCoroutine(WaitToForgetRoutine());
        }
    }

    private IEnumerator WaitToForgetRoutine()
    {
        float waitTime = 1f;
        yield return new WaitForSeconds(waitTime);

        m_enemyToAttack = null;
        if (m_stateMachine)
            m_stateMachine.EnterState(m_postChaseState);
    }
}
