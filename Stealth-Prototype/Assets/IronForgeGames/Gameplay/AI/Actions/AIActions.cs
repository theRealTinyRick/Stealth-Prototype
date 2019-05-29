///<Summary>
///Needs to be pretty generic. Controls the player entering an action and exiting an action.
///</Summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.AI
{
    public class AIActions : MonoBehaviour
    {
        public delegate void ActionStart();
        public event ActionStart OnActionStart;

        public delegate void ActionStopped(bool interupted);
        public event ActionStopped OnActionStopped;

        [SerializeField]
        [TabGroup(Tabs.Actions)]
        private Action[] actions;
        public Action[] Actions
        {
            get { return actions; }
        }

        [SerializeField]
        [TabGroup(Tabs.Actions)]
        private Action currentAction;
        public Action CurrentAction
        {
            get { return currentAction; }
        }

        [SerializeField]
        [TabGroup(Tabs.Actions)]
        private bool runningAction = false;
        public bool RunningAction
        {
            get { return runningAction; }
        }

        [SerializeField]
        [TabGroup(Tabs.SetUp)]
        public IEnemy enemyInterface;

        [SerializeField]
        [TabGroup(Tabs.SetUp)]
        private IAIEntity aiEntityInterface;

        [HideInInspector]
        public Transform playerTransform { get; set; }

        private Coroutine runAction;

        public void StartAction(Action _action, Animator animator)
        {
            CancelAction();

            currentAction = _action;
            runAction = StartCoroutine(RunAction(_action, animator));

            if( OnActionStart != null ) 
                OnActionStart();
        }

        public void EndAction(ActionInteruptType interruptType = ActionInteruptType.None)
        {
            runningAction = false;

            if(runAction != null)
            {
                StopCoroutine(runAction);
            }

            currentAction = null;
            FireActionStoppedEvent();

            if(interruptType == ActionInteruptType.None) return;

        }

        public void IntertuptAction(ActionInteruptType interuptType /*What caused the action to end prematurly*/)
        {
            foreach (ActionInteruptType type in currentAction.PossibleInterrupts)
            {
                if(type == interuptType && type != ActionInteruptType.None)
                {
                    EndAction(type);
                    break;
                }
            }
        }
        
		public void CancelAction()
		{
            EndAction();
		}

        private void FireActionStoppedEvent()
        {
            if(OnActionStart != null)
            {
                OnActionStopped(false);
            }
        }

        ///<Summary>
        ///
        ///</Summary>
        private IEnumerator RunAction(Action _action, Animator animator)
        {
            string _animation = _action.Animation;

            runningAction = true;

            animator.Play(_animation);

            switch(_action.Type)
            {
                case ActionType.MeleeAttack:
                    while( animator.GetCurrentAnimatorStateInfo(0).IsName(_animation) )
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;

                case ActionType.RangedAttack:
                    // while( animator.GetCurrentAnimatorStateInfo(0).IsName(_animation) )
                    {
                        Debug.Log( "Ranged Attack" );
                        yield return null;
                    }
                    break;

                case ActionType.Strafe:
                    Debug.Log( "Strafe" );
                    break;
            }

            EndAction();
            yield break;
        }

        public Action FindRandomAttack()
        {
            List <Action> actionList = new List<Action>();
            
            foreach (Action action in actions)
            {
                if(action.Type == ActionType.MeleeAttack)
                {
                    actionList.Add(action);
                }    
            }

            float result = UnityEngine.Random.Range(0, actionList.Count);                      
            
            return actionList[(int)result];
        }
    }
}