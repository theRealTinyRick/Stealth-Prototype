using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerStateComponent : SerializedMonoBehaviour
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private PlayerState currentState = PlayerState.Normal;
		public PlayerState CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				currentState = value;
			}
		}

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Dictionary<PlayerState, bool> states = new Dictionary<PlayerState, bool>();

		private PlayerAttackAnimationController playerAttackAnimationController;
		private PlayerGroundedComponent playerGroundedComponent;

		private void Start() 
		{
			playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
			playerGroundedComponent = GetComponent<PlayerGroundedComponent>();

            Debug.Log("Player State Component: lock the cursor");
            Cursor.lockState = CursorLockMode.Locked;
		}

		public void SetStateTrue(PlayerState state)
		{
			currentState = state;
		}

        public void SetState()
        {

        }

		public void ResetState()
		{
			currentState = PlayerState.Normal;
		}

        public bool CheckState(PlayerState state)
        {
            if(states.ContainsKey(state))
            {
                return states[state];
            }

		    return false;
        }

        /// <summary>
        /// If any of the states in the list are true then return true
        /// </summary>
        /// <param name="stateList"></param>
        /// <returns></returns>
        public bool IsInUnavailableState(List <PlayerState> stateList)
        {
            foreach(PlayerState _state in stateList)
            {
                if(states.ContainsKey(_state))
                {
                    if(states[_state])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
