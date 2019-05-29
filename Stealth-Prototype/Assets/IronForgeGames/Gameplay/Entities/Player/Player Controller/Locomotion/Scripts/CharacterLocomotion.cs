using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AH.Max.System;

public class CharacterLocomotion : MonoBehaviour
{
    [TabGroup(Tabs.Locomotion)]
    [SerializeField]
    private float baseSpeed;

    [TabGroup(Tabs.Locomotion)]
    [SerializeField]
    private float turnDamping;

    [HideInInspector]
    public Vector3 playerOrientationDirection = new Vector3();

    [HideInInspector]
    public Vector3 playerOrientationDirectionNotNormalized = new Vector3();

    private Transform LocomotionOrientationController;

    private Animator animator;
    private Rigidbody _rigidbody;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponentInChildren<Rigidbody>();

        if (LocomotionOrientationController == null)
        {
            LocomotionOrientationController = new GameObject().transform;
            LocomotionOrientationController.name = "Locomotion Orientation Controller";
        }
    }

    private void FixedUpdate()
    {
        Vector3 _direction = GetOrientationDirection();

        Move(_direction);
        RotatePlayer();
    }

    private void Move(Vector3 direction)
    {
        float _speed = baseSpeed;

        if (InputDriver.LocomotionDirection != Vector3.zero)
        {
            if(_rigidbody != null)
            {
                _rigidbody.velocity =
                    new Vector3((direction.x * _speed) * InputDriver.LocomotionDirection.magnitude,
                    _rigidbody.velocity.y,
                    (direction.z * _speed) * InputDriver.LocomotionDirection.magnitude);
            }
            else
            {
                transform.position +=                     
                    new Vector3((direction.x * _speed) * InputDriver.LocomotionDirection.magnitude,
                    _rigidbody.velocity.y,
                    (direction.z * _speed) * InputDriver.LocomotionDirection.magnitude);
            }
        }
    }

    private void RotatePlayer()
    {
        if (InputDriver.LocomotionDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, GetOrientationRotation(), turnDamping);
        }
    }

    private Vector3 GetOrientationDirection()
    {
        SetLocomotionOrientationControllerRotation();

        Vector3 _direction = InputDriver.LocomotionDirection;
        _direction = LocomotionOrientationController.TransformDirection(_direction).normalized;
        _direction.y = 0;

        InputDriver.LocomotionOrientationDirection = _direction;
        playerOrientationDirection = _direction;

        return _direction;
    }

    private void SetLocomotionOrientationControllerRotation()
    {
        Quaternion _targetRotation = EntityManager.Instance.GameCamera.transform.rotation;
        _targetRotation.x = 0;
        _targetRotation.z = 0;
        LocomotionOrientationController.rotation = _targetRotation;
    }

    private Quaternion GetOrientationRotation()
    {
        return Quaternion.LookRotation(GetOrientationDirection());
    }
}
