using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.System.Components
{
    public class StateComponent : SerializedMonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Dictionary<State, bool> states = new Dictionary<State, bool>();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        StateChangedEvent stateChangedEvent = new StateChangedEvent();

        public bool GetState(State state)
        {
            bool _stateValue;

            if(states.TryGetValue(state, out _stateValue))
            {
                return _stateValue;
            }

            Debug.LogWarning("The state component could not find the state: " + state + ", so we are refturning false ", gameObject);
            return false;
        }

        public bool GetOrCreateState(State state, bool defaultValue = false)
        {
            bool _stateValue;

            if (states.TryGetValue(state, out _stateValue))
            {
                return _stateValue;
            }

            states.Add(state, defaultValue);
            return defaultValue;
        }

        public bool HasState(State state)
        {
            if (states.ContainsKey(state))
            {
                return true;
            }

            return false;
        }

        public bool SetState(State state, bool value)
        {
            if(states.ContainsKey(state))
            {
                states[state] = value;

                if(stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, state);
                }

                return true;
            }

            return false;
        }

        public void SetStateFalse(State state)
        {
            if (states.ContainsKey(state))
            {
                states[state] = false;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, state);
                }
            }
        }

        public void SetStateTrue(State state)
        {
            if (states.ContainsKey(state))
            {
                states[state] = true;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, state);
                }
            }
        }

        public void ReverseState(State state)
        {
            if (states.ContainsKey(state))
            {
                states[state] = !states[state];

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, state);
                }
            }
        }

        public bool AnyStateTrue()
        {
            foreach (State _state in states.Keys)
            {
                if (states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateTrue(List<State> states)
        {
            foreach(State _state in states)
            {
                if(this.states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateFalse()
        {
            foreach (State _state in states.Keys)
            {
                if (!states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateFalse(List<State> states)
        {
            foreach (State _state in states)
            {
                if (this.states.ContainsKey(_state))
                {
                    if (!this.states[_state])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

