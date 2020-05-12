using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mod
{
    /// <summary>
    /// The base for a state of a state machine
    /// </summary>
    public abstract class IStateComponent : MonoBehaviour
    {
        [SerializeField] private string m_name = string.Empty;      // Name of the state
        private StateMachine m_stateMachine = null;                 // State machine we belong to

        /// <summary>
        /// The name of this state 
        /// </summary>
        public string stateName { get { return m_name; } }

        /// <summary>
        /// The state machine this state is apart of
        /// </summary>
        public StateMachine stateMachine { get { return m_stateMachine; } }

        protected virtual void Awake()
        {
            // States never start enabled. The default state we start
            // is controlled by the state machine
            enabled = false;
        }

        /// <summary>
        /// Initializes this state with a state machine. DO NOT CALL THIS FUNCTION DIRECTLY
        /// </summary>
        /// <param name="owner">State machine owner</param>
        public void InitWithMachine(StateMachine owner)
        {
            m_stateMachine = owner;
        }

        /// <summary>
        /// Notify from state machine to enable this state. DO NOT CALL THIS FUNCTION DIRECTLY
        /// </summary>
        /// <param name="previousState">Previous state or null</param>
        public void NotifyEnterState(IStateComponent previousState)
        {
            enabled = true;
            OnEnterState(previousState);
        }

        /// <summary>
        /// Notify from state machine to disable this state. DO NOT CALL THIS FUNCTION DIRECTLY
        /// </summary>
        /// <param name="nextState">Next state or null</param>
        public void NotifyExitState(IStateComponent nextState)
        {
            OnExitState(nextState);
            enabled = false;
        }

        /// <summary>
        /// Event for when this state has just been enabled. This can be
        /// used to initialize the state on a per entry basis
        /// </summary>
        /// <param name="previousState">State that was just disabled (Can be Null)</param>
        protected virtual void OnEnterState(IStateComponent previousState)
        {

        }

        /// <summary>
        /// Event for when this state has just been disabled. This can be
        /// used to cleanup the state on a per exit basis
        /// </summary>
        /// <param name="nextState"></param>
        protected virtual void OnExitState(IStateComponent nextState)
        {

        }
    }
}
