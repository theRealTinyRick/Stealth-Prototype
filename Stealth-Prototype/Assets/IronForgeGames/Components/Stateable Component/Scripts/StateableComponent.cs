using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.Components.Stateable
{
    [Serializable]
    public class State
    {
        public GameObject model;
        public float value;
    }

    public class StateableComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private List<State> states = new List<State>();

        private State currentState;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private bool evaluateOnEnable;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumValue;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maximumValue;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float currentValue;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        StateChangedEvent stateChangedEvent = new StateChangedEvent();

        private void OnEnable ()
        {
            if(evaluateOnEnable)
            {
                Evaluate();
            }
        }

        public void Evaluate()
        {
            foreach(State _state in states)
            {
                if(currentValue >= _state.value)
                {
                    currentState = _state;
                    SetStateUp();
                    return;
                }
            }

            if(stateChangedEvent != null)
            {
                stateChangedEvent.Invoke(currentState.model, currentState.value);
            }
        }

        private void SetStateUp()
        {
            if(currentState == null)
            {
                return;
            }

            foreach(State _state in states)
            {
                if(_state == currentState)
                {
                    _state.model.SetActive(true);
                }
                else
                {
                    _state.model.SetActive(false);
                }
            }
        }

        public void SetValue(float value)
        {
            currentValue = value;
            Evaluate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void IncrementState()
        {
            if(states.Count <= 1)
            {
                return;
            }

            float _currentValue = currentValue;

            if(states.IndexOf(currentState) >= states.Count - 1)
            {
                _currentValue = maximumValue;

                SetValue(_currentValue);

                return;
            }
            else
            {
                _currentValue = states[states.IndexOf(currentState) + 1].value;

                SetValue(_currentValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DecrementState()
        {
            if (states.Count <= 1)
            {
                return;
            }

            float _currentValue = currentValue;

            if (states.IndexOf(currentState) <= 1)
            {
                _currentValue = minimumValue;

                SetValue(_currentValue);

                return;
            }
            else
            {
                _currentValue = states[states.IndexOf(currentState) - 1].value;

                SetValue(_currentValue);
            }
        }
    }
}
