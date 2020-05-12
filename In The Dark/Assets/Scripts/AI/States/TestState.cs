using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mod;

namespace mod
{
    /// <summary>
    /// This state purely exists for testing State Machine and IStateComponent.
    /// Will be removed at a later date
    /// </summary>
    public class TestState : IStateComponent
    {
        public string m_stateToSwitchTo = string.Empty;

        protected override void OnEnterState(IStateComponent previousState)
        {
            Debug.LogFormat("Entering State {0}", stateName);
            StartCoroutine(SwithState());
        }

        protected override void OnExitState(IStateComponent nextState)
        {
            Debug.LogFormat("Exiting State {0}", stateName);
        }

        private IEnumerator SwithState()
        {
            float randTime = Random.Range(1f, 3f);
            yield return new WaitForSeconds(randTime);

            stateMachine.EnterState(m_stateToSwitchTo);
        }
    }
}
