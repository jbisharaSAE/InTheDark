using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mod
{
    /// <summary>
    /// A state machine responsible for managing the states of an object
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        // These two values are only used for initializing the state machine. They are not needed during runtime
        [SerializeField] private List<IStateComponent> m_states = new List<IStateComponent>();
        [SerializeField] private string m_startingState = string.Empty;

        Dictionary<string, IStateComponent> m_statesMap;        // All states present in the machine
        IStateComponent m_activeState;                          // Current active state

        void Awake()
        {
            m_statesMap = new Dictionary<string, IStateComponent>();

            // Initialize state machine lookup table
            {
                foreach (IStateComponent state in m_states)
                    AddState(state);

                // No longer need this!
                m_states = null;
            }
        }

        void Start()
        {
            IStateComponent startingState = GetState(m_startingState);
            if (!startingState && m_statesMap.Count > 0)
            {
                Debug.LogFormat("No starting state specified for {0}. Falling back to first state");

                // TODO: Maybe try using an OrderedDictionary? (Don't know how that compares to regular dictionary)
                foreach (var entry in m_statesMap)
                    startingState = entry.Value;
            }

            if (startingState)
                EnterStateInternal(startingState);

            // No longer need this! (Maybe in future though, as a fallback)
            m_startingState = null;
        }

        /// <summary>
        /// Adds a new state to this machine
        /// </summary>
        /// <param name="newState">State to add</param>
        private void AddState(IStateComponent newState)
        {
            if (!newState)
                return;

            // State with the same name might already exist, consider this new state a 'replacement' if so
            {
                IStateComponent existingState = null;
                if (m_statesMap.TryGetValue(newState.stateName, out existingState))
                {
                    Debug.LogWarningFormat(this, "State {0} is being replaced", newState.stateName);
                    RemoveState(existingState.stateName);
                }
            }

            m_statesMap.Add(newState.stateName, newState);
            newState.InitWithMachine(this);
        }

        /// <summary>
        /// Removes the state with name from the system (Does NOT destroy it)
        /// </summary>
        /// <param name="stateName">Name of state to remove</param>
        private void RemoveState(string stateName)
        {
            m_statesMap.Remove(stateName);
        }

        /// <summary>
        /// Enters a new state, exiting from the already active state
        /// </summary>
        /// <param name="stateName">Name of state to activate</param>
        public void EnterState(string stateName)
        {
            IStateComponent stateToEnter = GetState(stateName);
            if (stateToEnter)
                EnterStateInternal(stateToEnter);
        }

        /// <summary>
        /// Handles entering a new state, calling appropriate events
        /// </summary>
        /// <param name="stateToEnter">State being activated</param>
        private void EnterStateInternal(IStateComponent stateToEnter)
        {
            IStateComponent stateToExit = m_activeState;
            if (stateToExit)
            {
                stateToExit.NotifyExitState(stateToEnter);
                m_activeState = null;
            }

            m_activeState = stateToEnter;
            m_activeState.NotifyEnterState(stateToExit);
        }

        /// <summary>
        /// Get a state by name
        /// </summary>
        /// <param name="stateName">Name of the state to get</param>
        /// <returns>Valid state or null</returns>
        private IStateComponent GetState(string stateName)
        {
            IStateComponent state = null;
            if (m_statesMap.TryGetValue(stateName, out state))
                return state;

            return null;
        }
    }
}
