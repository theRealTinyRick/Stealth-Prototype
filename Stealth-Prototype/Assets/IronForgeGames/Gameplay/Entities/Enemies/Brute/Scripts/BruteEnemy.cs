using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay.AI.BruteEnemy
{
	public class BruteEnemy : AIEntity<BruteEnemy>, IEnemy
	{	
		///<Summary>
		///The actual target the entity should follow
		///</Summary>
		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private Transform target;
		[HideInInspector]
		public Transform Target 
		{
			get { return target; } 
			private set { target = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private EnemyType enemyType;
		[HideInInspector]		
		public EnemyType EnemyType 
		{
			get { return enemyType; }
			private set { enemyType = value; }  
		}

		//this is a read only property. Only this class and the base class can change state
		[HideInInspector]
		public override AIStates State
		{	
			get { return _state; }
		}

		[HideInInspector]
		public override ActionStates ActionState
		{
			get { return actionState; }
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float maxFieldOfViewAngle;
		[HideInInspector]
		public float MaxFieldOfViewAngle
		{
			get { return maxFieldOfViewAngle; } 
			private set { maxFieldOfViewAngle = value; }
		}

		[SerializeField] 
		[TabGroup(Tabs.Preferences)]
		private float maxHeightDifference;
		[HideInInspector]
		public float MaxHeightDifference
		{
			get { return maxHeightDifference; }
			private set { maxHeightDifference = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float aggroRange;
		[HideInInspector]
		public float AggroRange
		{
			get { return aggroRange; }
			protected set { aggroRange = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float attackRange;
		[HideInInspector]
		public float AttackRange 
		{
			get { return attackRange; }  
			protected set { attackRange = value; }
		}

		// The current time since last attack
		private float timeSinceLastAttack;

		private void Start()
		{
			Initialize();
		}

		public override void Initialize()
		{
			// do stuff here to initialize the game
			FindTargetPlayer(ref target);

			_state = defaultState;

			SetUpComponents();

			aiActions.playerTransform = target;
			aiActions.enemyInterface = this;
			enemyInterface = this;
		}

		private void Update () 
		{
			Tick();
		}	

		protected override void Tick()
		{
			if( !EntityManager.Instance.Player ) return;

			if(CheckAggro())
			{
				EnterAggroState();
			}

			if(_state == AIStates.Aggro)
			{
				MainMovementLoop(aggroRange, attackRange, target.transform.position);
				RotateTowardsPlayer(target.position, attackRange);
			}
			
		}

		protected override bool CheckAggro()
		{
			if( CheckRangeSquared(aggroRange, transform.position, target.position) )
			{
				if( CheckFieldOfView(transform, target.position, maxFieldOfViewAngle) )
				{
					if( CheckLineOfSight(transform.position, target.position, target.gameObject.layer) )
					{
						if( CheckHeightDifference(maxHeightDifference, transform.position, target.position) )
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}

		protected override IEnumerator CombatPattern()
		{
			while(!CheckRangeSquared(attackRange, transform.position, target.position))
			{
				yield return new WaitForEndOfFrame();
			}

			aiActions.StartAction(new Action(AIAnimatorController.AttackOne, ActionType.MeleeAttack), animator);

			yield return new WaitForSeconds(attackDelay);

			while(aiActions.RunningAction)
			{
				yield return new WaitForEndOfFrame();
			}

			combatPattern = StartCoroutine(CombatPattern());
			
			yield break;
		}
	}
}
