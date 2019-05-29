using System.Linq;
using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay.System.Components;

namespace AH.Max.Gameplay
{
	public class PlayerJump : MonoBehaviour 
	{
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float verticalJumpStrength;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float horizontalJumpStrength;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        private JumpStartedEvent jumpStartedEvent;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        private JumpForwardStartedEvent jumpForwardStartedEvent;

        private Vector3 jumpDirection = new Vector3();

        private float jumpDelta;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float jumpDeltaMultiplier;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maximumAirMovementDotProduct;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public bool playerJumped = false;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public bool playerLeftTheGround = false;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public bool shouldUseLocomotionControlInTheAir;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private State[] unavailableStates;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private State isGroundedState;

        private new Rigidbody rigidbody;
        private PlayerStateComponent playerStateComponent;
        private StateComponent stateComponent;
        private PlayerLedgeClimber playerLedgeFinder;

        void OnEnable()
        {
            if (jumpStartedEvent == null)
            {
                jumpStartedEvent = new JumpStartedEvent();
            }

            if (jumpForwardStartedEvent == null)
            {
                jumpForwardStartedEvent = new JumpForwardStartedEvent();
            }

            stateComponent = GetComponent<StateComponent>();
            rigidbody = transform.root.GetComponentInChildren<Rigidbody>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
            playerLedgeFinder = GetComponent<PlayerLedgeClimber>();

            playerLeftTheGround = false;
            playerJumped = false;
        }

        private void OnDisable()
        {
        }

        private void FixedUpdate()
        {
            if(!IsGrounded())
            {
                if(playerJumped)
                {
                    playerLeftTheGround = true;

                    if (!shouldUseLocomotionControlInTheAir)
                    {
                        jumpDelta -= (Time.deltaTime * jumpDeltaMultiplier);

                        if(jumpDelta < 0)
                        {
                            jumpDelta = 0;
                        }

                        Vector3 _direction = new Vector3(jumpDirection.x * jumpDelta, rigidbody.velocity.y, jumpDirection.z * jumpDelta);
                        rigidbody.velocity = _direction;

                        // evalutate the dot product ---- if it hits zero then you should break out of the motion
                        Vector3 _forward = transform.forward;

                        if(InputDriver.LocomotionOrientationDirection != Vector3.zero)
                        {
                            float _dot = Vector3.Dot(_forward, InputDriver.LocomotionOrientationDirection);

                            if (_dot <= maximumAirMovementDotProduct)
                            {
                                shouldUseLocomotionControlInTheAir = true;
                            }
                        }
                    }
                }

            }
            else
            {
                if(playerLeftTheGround)
                {
                    playerJumped = false;
                    playerLeftTheGround = false;
                }
            }
        }

        public void Jump()
        {
            if(ShouldJump())
            {
                bool jumpingForward = false;

                jumpDirection = InputDriver.LocomotionOrientationDirection;

                if (InputDriver.LocomotionOrientationDirection != Vector3.zero)
                {
                    Quaternion _rotation = Quaternion.LookRotation(jumpDirection);
                    transform.rotation = _rotation;
                    shouldUseLocomotionControlInTheAir = false;
                    jumpingForward = true;
                }
                else
                {
                    shouldUseLocomotionControlInTheAir = true;
                }

                playerJumped = true;
                jumpDelta = horizontalJumpStrength;

                Vector3 direction = new Vector3(0, 1, 0) * verticalJumpStrength;

                rigidbody.velocity = direction;

                if(jumpingForward)
                {
                    if(jumpForwardStartedEvent != null)
                    {
                        jumpForwardStartedEvent.Invoke();
                    }
                }
                else
                {
                    if(jumpStartedEvent != null)
                    {
                        jumpStartedEvent.Invoke();
                    }
                }
            }
        }

        private bool ShouldJump()
        {
            return !stateComponent.AnyStateTrue(unavailableStates.ToList()) && IsGrounded();
        }

        private bool IsGrounded()
        {
            return stateComponent.GetState(isGroundedState);
        }
	}
}
