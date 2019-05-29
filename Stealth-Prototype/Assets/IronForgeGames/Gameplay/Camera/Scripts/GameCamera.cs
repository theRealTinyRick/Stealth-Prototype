using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

public class GameCamera : Entity
{
	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	private bool useCameraController;

	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	private Vector3 cameraOffsetPosition;

	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	private Vector2 cameraLookAtOffset;

	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	private float offsetDistance;

	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	[Range(0, 10)]
	private float xSensitivity;

	[TabGroup(Tabs.Preferences)]
	[SerializeField]
	[Range(0, 10)]
	private float ySensitivity;

	private Transform playerTransform;

	private Vector3 cameraPosition;

	private float currentX = 0;

	private float currentY = 0;

	public bool invertXAxis = false;
	public bool invertYAxis = false;

	private void Update () 
	{
		if(useCameraController)
		{
			playerTransform = EntityManager.Instance.Player.transform;
			if(playerTransform == null) return;

			GetInput();
		}
	}

	private void LateUpdate()
	{
		if(useCameraController)
		{
			if(playerTransform == null) return;
			ApplyMovement();
			ApplyRotation();
		}
	}
	
	private void GetInput()
	{
		currentX += InputDriver.RightInputDirection.x * xSensitivity;
		currentY += InputDriver.RightInputDirection.y * ySensitivity;
	}

	private Vector3 GetTargetPosition()
	{
		Vector3 _offset = new Vector3(0, 0, -CameraClippingOffset());
		Quaternion _rotation = Quaternion.Euler(currentY * (invertYAxis ? -1 : 1), currentX * (invertXAxis ? -1 : 1), 0);
		return (playerTransform.position + cameraOffsetPosition) +_rotation * _offset;
	}

	private Quaternion GetTargetRotation()
	{
		Vector3 _lookAtPosition = playerTransform.position;
		_lookAtPosition.y += cameraLookAtOffset.y;
		_lookAtPosition += playerTransform.right * cameraLookAtOffset.x;

		Quaternion _rotation = Quaternion.LookRotation(_lookAtPosition - transform.position);
		
		return _rotation;
	}

	private void ApplyMovement()
	{
		transform.position = GetTargetPosition();
	}

	private void ApplyRotation()
	{
		transform.rotation = GetTargetRotation();
	}

	///<Summary>
	/// fire a ray cast from the origin to the camera a detect any obstacle in between
	///</Summary>
	private float CameraClippingOffset()
	{
		Vector3 _origin = playerTransform.position + cameraOffsetPosition;
		Vector3 _direction  = transform.position - _origin;

		float _rayLength = Vector3.Distance(_origin, transform.position) + 1;
		_rayLength = Mathf.Clamp(_rayLength, 1, offsetDistance);

		RaycastHit _hit;

		if(Physics.Raycast(_origin, _direction, out _hit, _rayLength, PhysicsLayers.ingnorePlayerLayer))
		{
			return Vector3.Distance(_origin, _hit.point);
		}
		
		return offsetDistance;
	}
}
