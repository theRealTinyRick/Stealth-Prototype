using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay.System.Components;

namespace AH.Max.Gameplay
{
	public class PlayerAttackAnimationController : MonoBehaviour 
	{
		private bool isAttacking = false;
		public bool IsAttacking{ get{ return isAttacking; } }

		private List <string> queue = new List<string>();

        private int maxNumberOfClicks 
        {
            get 
            {
                if(currentAnimSet == null)
                {
                    return 0;
                }

                return currentAnimSet.Length;
            }
        }
		
		private int currentNumberOfClicks = 0;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float timeToClick;

		private float time = 0;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private State[] nonAttackableStates;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private State groundedState;

        [HideInInspector]
        public string[] swingBooleans = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};

        private string[] currentAnimSet;
        private WeaponType currentWeaponType;
        private bool hasAttacked;

        private Animator animator;
        private StateComponent stateComponent;
        private PlayerGroundedComponent playerGroundedComponent;
        private PlayerToolComponent playerToolsComponent;


        //events
        [TabGroup(Tabs.Events)]
        [SerializeField]
        public AttackStartedEvent attackStartedEvent = new AttackStartedEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public AttackEndedEvent attackEndedEvent = new AttackEndedEvent();

		private void OnEnable()
		{
			animator = GetComponent<Animator>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
            stateComponent = GetComponent<StateComponent>();
            playerToolsComponent = GetComponentInChildren<PlayerToolComponent>();
		}

		private void Update()
		{
			AttackTimer();
			CurrentlyInAttackState();
		}

        // this method simply determines if the player is still clicking. 
        // if the player keeps clicking then stops then the attacks should stop as well. 
        private void AttackTimer()
		{
            if(IsAttacking)
            {
			    if(queue.Count > 0)
			    {
				    time += Time.deltaTime;

				    if(time > timeToClick)
				    {
					    StopAttacking();
				    }
			    }
            }
		}

		public void StopAttacking()
		{
			foreach(string _animation in swingBooleans)
			{
				if(_animation != swingBooleans[0])
				{
					animator.SetBool(_animation, false);
				}
			}

			queue.Clear();
			currentNumberOfClicks = 0;
			time = 0;
			isAttacking = false;
            hasAttacked = false;
            attackEndedEvent.Invoke();
        }

        public void LightAttack()
		{
            if(currentWeaponType == null)
            {
                currentWeaponType = playerToolsComponent.currentToolType;

                if(currentWeaponType == null)
                    return; 
            }

            if(currentWeaponType.handedness == Handedness.EmptyHands)
            {
                return;
            }

            if(currentAnimSet == null)
            {
                currentAnimSet = currentWeaponType.GetAnimations();
            }

			if(EvaluateQueueConditions())
			{
                hasAttacked = true;
				isAttacking = true;

                attackStartedEvent.Invoke();

				currentNumberOfClicks ++;
				time = 0;

				int _index = currentNumberOfClicks - 1;	

				queue.Add(currentAnimSet[_index]);

				if(_index == 0)
				{
					animator.Play(currentAnimSet[0]);
				}
				else
				{
					animator.SetBool(swingBooleans[_index], true);
				}
			}
		}

		private bool EvaluateQueueConditions()
		{
            bool _inProperState = !stateComponent.AnyStateTrue(nonAttackableStates.ToList()) && stateComponent.GetState(groundedState);

            if(!playerGroundedComponent.IsGrounded)
            {
                return false;
            }

            if(!_inProperState)
            {
                return false;
            }

            if (currentNumberOfClicks < maxNumberOfClicks)
			{
				return true;
			}
			else
			{
				time = 0;
			}

            return false;
		}

		public bool CurrentlyInAttackState()
		{
            if(currentAnimSet != null)
            {
			    foreach(var _animation in currentAnimSet)
			    {
				    foreach(var thing in animator.GetCurrentAnimatorClipInfo(0))
				    {
					    if(_animation == thing.clip.name)
					    {
						    return true;
					    }
				    }
			    }
            }

            return false;
		}

        /// <summary>
        /// A response to the tool component getting a weapon
        /// </summary>
        /// <param name="weaponType"></param>
        public void OnToolEquipped(WeaponType weaponType)
        {
            currentWeaponType = weaponType;
            SetCurrentAnimations();
        }

        [Button]
        public void SetCurrentAnimations()
        {
            currentAnimSet = currentWeaponType.GetAnimations();
        }

		public void AttackEndEvent()
		{

		}
	}
}
