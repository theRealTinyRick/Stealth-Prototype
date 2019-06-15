using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay.System.Components;

namespace AH.Max.Gameplay.Stealth
{
    [RequireComponent(typeof(PlayerLocomotion))]
    public class StealthMovementComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Preferences)]
        [SerializeField]
        private float speed;
        
        [TabGroup(Tabs.Preferences)]
        [SerializeField]
        private float rotationSpeed;

        [TabGroup(Tabs.Preferences)]
        [SerializeField]
        private State stealthState;
        
        // components
        private Animator animator;
        private PlayerLocomotion playerLocomotion;
        private StateComponent stateComponent;
        private PlayerStealthController playerStealthController;
        private new Rigidbody rigidbody;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponentInChildren<PlayerLocomotion>();
            stateComponent = GetComponentInChildren<StateComponent>();
            playerStealthController = GetComponentInChildren<PlayerStealthController>();

            rigidbody = GetComponentInChildren<Rigidbody>();
        }

       private void FixedUpdate()
       {
           Move();
       }

        private void UpdateOrientation()
        {
            Transform _orientation = playerStealthController.currentStealthObstacle.orientation;
            _orientation.position = transform.position;
        }

       private void Move()
       {
           if(IsInStealthMode())
           {
                Vector3 _moveDirection = playerLocomotion.GetOrientationDirection();
                UpdateOrientation();

                if(_moveDirection.magnitude > 0)
                {
                    Vector3 _sideA = playerStealthController.currentStealthObstacle.edgeA.position;
                    Vector3 _sideB = playerStealthController.currentStealthObstacle.edgeB.position;

                    float _angleA = Vector3.Angle(_moveDirection, (_sideA - transform.position));
                    float _angleB = Vector3.Angle(_moveDirection, (_sideB - transform.position));

                    Vector3 _actualDirection = _angleA < _angleB ? (_sideA - _sideB) : (_sideB - _sideA);

                    Vector3 _sideApproaching = _angleA < _angleB ? _sideA : _sideB;
                    Vector3 _sideBehind = _angleA > _angleB ? _sideA : _sideB;
                    
                    float _distanceBetweenSides = Vector3.Distance(_sideA, _sideB);

                    float _myDistance = Vector3.Distance(transform.position, _sideBehind);

                    if(_myDistance < _distanceBetweenSides - 0.5f)
                    {
                        rigidbody.velocity = _actualDirection.normalized * speed;
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_actualDirection), rotationSpeed) ;

                        animator.SetFloat("Vertical", rigidbody.velocity.magnitude);
                        
                        return;
                    }
                }
                
                animator.SetFloat("Vertical", 0);
           }
       }

       private bool IsInStealthMode()
       {
            return stateComponent.GetState(stealthState); 
       }
    }
}
