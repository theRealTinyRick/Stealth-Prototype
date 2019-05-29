using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay.AI
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public abstract class AIEntity <T> : AH.Max.Entity, IAIEntity where T : AIEntity<T>
	{
		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		protected AIStates _state = AIStates.Stationary;
		public abstract AIStates State { get; }

		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		protected ActionStates actionState;
		public abstract ActionStates ActionState { get; }
		
		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		protected AIStates defaultState;

		protected Coroutine combatPattern;

		protected AIActions aiActions;

		protected Animator animator;

		protected CapsuleCollider capsuleCollider;

		protected IEnemy enemyInterface;

		protected LayerMask playerLayer = 1 << 8; 

		[HideInInspector]
		public NavMeshAgent navMeshAgent { get; private set;}

		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		protected float attackDelay;

		[SerializeField]
		[TabGroup(Tabs.Locomotion)]
		protected float speed;
		
		[SerializeField]
		[TabGroup(Tabs.Locomotion)]
		private float followPause;

		[SerializeField]
		[Range(0, 1)]
		[TabGroup(Tabs.Locomotion)]
		private float turnSpeed;

		private  float time = 0.0f;

		///<Summary>
		///This is the function you can use to set up everything the Entity will need
		///</Summary>
		public abstract void Initialize();

		///<Summary>
		///This is the actual pattern the enemy should follow
		///</Summary>
		protected abstract void Tick();

		///<Summary>
		/////how should the enemy move?
				//Appraoch
				//strafe
				//back off
				//None
			
			//does the enemy have a clear line of attack
				// are there other enemies in the way?
				//Am I close enough
				//Is it my turn to attack
				//have I waited longe enough scince my last attack to do this. 
		///</Summary>
		protected abstract IEnumerator CombatPattern();

		protected void FindTargetPlayer( ref Transform playerVariable )
		{
			//Find the player transform in the EntityManager
			playerVariable = EntityManager.Instance.Player.transform;
		}

		protected virtual void SetUpComponents()
		{
			playerLayer = ~playerLayer;

			aiActions = GetComponent<AIActions>();
			animator = GetComponent<Animator>();
			capsuleCollider = GetComponent<CapsuleCollider>();
			navMeshAgent = GetComponent<NavMeshAgent>();
		}

		private void Start()
		{
		}

		public void Move(Vector3 _targetPosition, float range)
		{
			if(navMeshAgent)
			{
				navMeshAgent.SetDestination(_targetPosition);

				// float timePassed = Time.time - time;

				if(CheckRangeSquared(range, transform.position, _targetPosition))
				{
					navMeshAgent.isStopped = true;
					time = 0.0f;
				}
				else
				{
					time += Time.deltaTime;
								
					if(time > followPause)
					{
						navMeshAgent.isStopped = false;
					}
				}
	
				if(!navMeshAgent.isStopped)
				{
					float val = Mathf.Lerp(animator.GetFloat(AIAnimatorController.Speed), 0.5f, 0.1f);

					animator.SetFloat(AIAnimatorController.Speed, val);
					//animator.SetTrigger(AIAnimatorController.IsMoving);
				}
				else
				{
					float val = Mathf.Lerp(animator.GetFloat(AIAnimatorController.Speed), 0, 0.05f);

					if(Mathf.Approximately(val, 0))
					{
						val = 0;
					}

					animator.SetFloat(AIAnimatorController.Speed, 0);
					//animator.SetTrigger(AIAnimatorController.IsMoving);
				}
			}
			else
			{
				Debug.LogError("The nav mesh agent component is not set up. Did you call SetUpComponents()?");
			}
		}

		///<Summary>
		///This function takes into account the AIState and current action. To simply rotate towards something use ->
		///</Summary>
		public void RotateTowardsPlayer(Vector3 target, float range)
		{
			if(_state != AIStates.Aggro) return;
			if(!EvaluateCurrentActionType()) return;

			if(CheckRangeSquared(range, transform.position, target))
			{
				Vector3 _direction = target - transform.position;
				Quaternion _rotation = Quaternion.LookRotation(_direction);

				transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, turnSpeed);
			}
		}

		private bool EvaluateCurrentActionType()
		{	
			if(aiActions.CurrentAction == null) return false;
			if(aiActions.CurrentAction.Type == ActionType.MeleeAttack) return false;
			if(aiActions.CurrentAction.Type == ActionType.Buff) return false;
			if(aiActions.CurrentAction.Type == ActionType.MoveToRandomLocation) return false;
			
			return true;
		}

		//Check whether or not ther player is in front of the enemy
		protected bool CheckFieldOfView(Transform _myTransform, Vector3 _targetPosition, float _maxAngle)
		{
			float angle;

			Vector3 _toVector = _targetPosition - _myTransform.position;
			_toVector.y = _myTransform.position.y;

			angle = Vector3.Angle(_myTransform.forward, _toVector);

			if(angle <= _maxAngle)
			{
				return true;
			}

			return false;
		}

		///<Summary>
		/// This function will check the squared distance
		/// This function will also act as if the positions are on the same plane. (The y values will be zeroed out)
		///</Summary>
		protected bool CheckRangeSquared(float _range, Vector3 _myPosition, Vector3 _targetPosition)
		{
			_range *= _range;

			Vector3 _toVector = _targetPosition - _myPosition;
			_toVector.y = 0;

			float _magnitude = ( _toVector.x * _toVector.x ) + (_toVector.z * _toVector.z);

			if(_magnitude <= _range)
			{
				return true;
			}

			return false;
		}

		///<Summary>
		/// Uses a raycast to check if the AIEntity can see a target
		///</Summary>
		protected bool CheckLineOfSight(Vector3 _myPosition, Vector3 _targetPosition, LayerMask _playerLayer)
		{
			//move the positions up about 2 units to be head level
			_myPosition.y += 2;
			_targetPosition.y += 2;
			_playerLayer = ~_playerLayer;

			Vector3 _direction = _targetPosition - _myPosition;
			float _distance = Vector3.Distance(_myPosition, _targetPosition);
			RaycastHit _hit;

			if(Physics.Raycast(_myPosition, _direction, out _hit, _distance, playerLayer))
			{
				return false;
			}

			return true;
		}

		///<Summary>
		///Checks the height difference.
		///</Summary>
		protected bool CheckHeightDifference(float _range, Vector3 _myPosition, Vector3 _targetPosition)
		{
			float _topHieght = _myPosition.y > _targetPosition.y ? _myPosition.y :  _targetPosition.y;
			float _bottomPosition = _myPosition.y < _targetPosition.y ? _myPosition.y :  _targetPosition.y;

			float _difference = _topHieght - _bottomPosition;

			if(_difference <= _range)
				return true;

			return false;
		}

		///<Summary>
		/// Set the AI State
		///</Summary>
		protected void SetStateHard (AIStates state) 
		{
			_state = state;_state = state;
		}

		///<Summary>
		/// simply checks the current state against whatever you want. 
		///</Summary>
		public bool CheckState ( AIStates state )
		{
			if(_state == state)
			{
				return true;
			}
			return false;
		}

		protected bool AttackTimer()//checks if enough time has past for the enemy to attack
		{
			return false;
		}

		protected virtual bool CheckAggro()
		{
			Debug.LogWarning("You have not overridden the default aggro checker on " + typeof(T).FullName + ". Was this intentional?");
			
			if( _state != AIStates.Aggro )
			{
				//check every way the enemy should look for the player
				// if(CheckRangeSquared(aggroRange, transform.position, ))
				return true;
			}

			return false;
		}

		protected virtual void EnterAggroState()
		{
			if( !CheckState(AIStates.Aggro) && combatPattern == null )
			{	
				StartCombatPattern();
				SetStateHard(AIStates.Aggro);
			}
		}

		protected virtual void ExitAggroState()
		{
			if( CheckState(AIStates.Aggro) )
			{
				StopCombatPattern();

				SetStateHard(defaultState);
			}
		}

		protected void MainMovementLoop(float aggroRange, float meleeRange, Vector3 target)
		{
			if(_state != AIStates.Aggro) return;

			float _range = aggroRange;

			if(enemyInterface != null)
			{
				if(AIManager.Instance.AggressiveEnemies.Contains(enemyInterface))
				{
					_range = meleeRange;
				}
			}

			navMeshAgent.stoppingDistance = _range;
			navMeshAgent.speed = speed;

			Move(target, _range);
		}

		protected virtual void StartCombatPattern()
		{
			combatPattern = StartCoroutine(CombatPattern());
		}

		protected virtual void StopCombatPattern()
		{
			if(combatPattern != null)
			{
				StopCoroutine(combatPattern);
				combatPattern = null;
			}
		}
	}

	public class AIAnimatorController
	{
		public const string IsMoving = "IsMoving";
		public const string Speed = "Speed";
		public const string StrafeRight = "StrafeRight";
		public const string StrafeLeft = "StrafeLeft";
		public const string Defend = "Defend";
		public const string BackUp = "BackUp";
		public const string Dodge = "Dodge";

		public const string AttackOne = "AttackOne";
		public const string AttackTwo = "AttackTwo";
		public const string AttackThree = "AttackThree";
		public const string AttackFour = "AttackFour";
		public const string AttackFive = "AttackFive";
		public const string AttackSix = "AttackSix";

		string[] AttackAnimations = new string[] {""};
	}
}

