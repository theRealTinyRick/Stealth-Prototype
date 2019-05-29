using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay.System.Components;

namespace AH.Max.Gameplay
{
    public class PlayerLocomotionAnimationHook : MonoBehaviour
    {
        public const string LockedOn = "LockedOn";
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";

        [TabGroup("Animation")]
        [SerializeField]
        public float verticalAnimatorFloat = 0;

        [TabGroup("Animation")]
        [SerializeField]
        public float horizontalAnimatorFloat = 0;

        [TabGroup("Animation")]
        [SerializeField]
        [Range(0, 1)]
        private int lockedOnAnimatorFloat = 0; // for testing purposes

		[TabGroup("Animation")]
		[SerializeField]
		private bool ShouldLean; // determines if the player should lean while running.

        private bool isPreparing;

        private PlayerLocomotion playerLocomotion;
        private StateComponent stateComponent;
        private PlayerAttackAnimationController playerAttackAnimatorController;
        private PlayerEvade playerEvade;
        private TargetingManager targetingManager;
        private Rigidbody rigidbody;

        private Animator animator;

        [SerializeField]
        private State[] unusableStates;

		private void Start()
		{
			playerLocomotion = GetComponent<PlayerLocomotion>();
            stateComponent = GetComponent<StateComponent>();
			playerAttackAnimatorController = GetComponent<PlayerAttackAnimationController>();
			playerEvade = GetComponent<PlayerEvade>();
			animator = GetComponent<Animator>();
            targetingManager = GetComponentInChildren<TargetingManager>();
            rigidbody = GetComponent<Rigidbody>();
		}

		private void Update () 
		{
			LocomotionAnimation();
			ApplyAnimationFloats();
		}

		private void LocomotionAnimation()
		{
            if (CheckState())
            {
                Vector3 _forwardVector = transform.forward;
			    Vector3 _moveDirection = playerLocomotion.playerOrientationDirection * InputDriver.LocomotionDirection.magnitude; //multiply here so we can dampen some of the values.
			    Vector3 _crossProduct = Vector3.Cross(_forwardVector, _moveDirection);

                /*
    			    Debug.DrawRay(transform.position, _moveDirection, Color.red);
    			    Debug.DrawRay(transform.position, _forwardVector, Color.blue);
	    		    Debug.DrawRay(transform.position, _crossProduct, Color.green);
                */

			    if(!isPreparing)
			    {
                    Vector3 _velocoty = rigidbody.velocity;
                    _velocoty.y = 0;

                    verticalAnimatorFloat = (_velocoty.magnitude * Vector3.Dot(_forwardVector, _moveDirection)) / 2;

                    horizontalAnimatorFloat = ShouldLean ? _crossProduct.y : 0;
                    lockedOnAnimatorFloat = 0;
                }
                else
			    {
                    verticalAnimatorFloat = _moveDirection.magnitude * Vector3.Dot(_forwardVector, _moveDirection);
                    horizontalAnimatorFloat = _crossProduct.y;
                    lockedOnAnimatorFloat = 1;
                }

            }
		}

		private void ApplyAnimationFloats()
		{
			if(CheckState() /*&& animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion")*/)
			{
			    animator.SetFloat(LockedOn, (float)lockedOnAnimatorFloat);
		    	animator.SetFloat(Horizontal, Mathf.Lerp(animator.GetFloat(Horizontal), horizontalAnimatorFloat, 0.2f));
	    		animator.SetFloat(Vertical, Mathf.Lerp(animator.GetFloat(Vertical), verticalAnimatorFloat, 0.2f));

                return;
			}

			//animator.SetFloat(Horizontal, 0);
			//animator.SetFloat(Vertical, 0);
		}

        private bool CheckState()
        {
            return !stateComponent.AnyStateTrue(unusableStates.ToList());
        }

        public void SetIsPreparing(bool state)
        {
            isPreparing = state;
        }
    }
}
