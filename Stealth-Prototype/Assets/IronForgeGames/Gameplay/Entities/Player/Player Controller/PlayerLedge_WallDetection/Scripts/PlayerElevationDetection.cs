using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerElevationDetection : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float distanceToCheck;		

		[HideInInspector]
		public float DistanceToCheck{ get{ return distanceToCheck; } }

		/// <summary>
		/// The master bool to tell other systems if the player has hit a ledge
        /// Use this in other scripts to determine if we should apply and game logic related to elevation changes
		/// </summary>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private bool validLedge = false;
		public bool ValidLedge{ get { return validLedge; } }	

		/// <summary>
		/// The current type of vault that we want to execute
		/// </summary>
		[SerializeField]
		private VaultType vaultType;
		public VaultType VaultType
		{
			get
			{
				return vaultType;
			}
		}	

		/// <summary>
		/// The actual position of the ledge
		/// </summary>
		/// <returns></returns>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private Vector3 ledge = new Vector3();
		public Vector3 Ledge{ get { return ledge; } }
		
		/// <summary>
		/// The normal of the wall that we are detecting against.
		/// It should be kept in mind that if you use this yo should assume to only use this if there is a validLedge
		/// </summary>
		/// <returns></returns>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private Vector3 wallNormal = new Vector3();
		public Vector3 WallNormal{ get { return wallNormal; } }

		/// <summary>
		/// The events to tell other systems if we have ledge
		/// </summary>
		/// <returns></returns>
		public LedgeDetectedEvent ledgeDetectedEvent = new LedgeDetectedEvent();
		
		/// <summary>
		/// The events to tell other systems that we do not have a ledge
		/// </summary>
		/// <returns></returns>
		public NoLedgeDetectedEvent noLedgeDetectedEvent = new NoLedgeDetectedEvent();

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		public float maxHeight;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		public float minHeight;

        /// <summary>
        /// This is the minimum angle to the wall - how much does the player need to be facing the wall to climb/vault
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumAngleToWall;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumWallSlope;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumFloorSlope;

		private LayerMask layerMask = 1 << 8;

		private void Start()
		{
			layerMask = ~layerMask;
		}

		private void FixedUpdate()
		{
			DetectElevation();
		}

		private void DetectElevation()
		{
			//if( playerStateManager.CurrentState == PlayerState.Traversing ) return;

			Vector3 _origin = transform.position;
			_origin.y += 0.3f;

			RaycastHit _hit;

			Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
			if(Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
			{
				Vector3 _ledge = _hit.point;
				Vector3 _normal = _hit.normal;

                //get the reverse of the angle we shot the ray from to get an accurate angle calculation
                Vector3 _hitAngle = -transform.forward;

                // return early if the player is not properly facing the wall
                float _angle = Vector3.Angle(_normal, _hitAngle);
                if(_angle > minimumAngleToWall)
                {
                    DeleteLedge();
                    return;
                }

				//we store this distance so we can offset our downward ray cast and not over shoot it
				float _distanceToCheckDown = Vector3.Distance(_origin, _hit.point) + 0.1f;

				if(CheckWallSlope(_hit))
				{
					_origin = transform.position;
					_origin.y += maxHeight;
					
					Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
					if(!Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
					{
						_origin = transform.position;

						//WARNING, This may cause misses
						_origin += transform.forward * _distanceToCheckDown;
						_origin.y += maxHeight;

						if(Physics.Raycast(_origin, Vector3.down, out _hit, maxHeight, layerMask))
						{
							_ledge.y = _hit.point.y;

							if(CheckFloorSlope(_hit))
							{	
								// set a target position and tell the player vault that we have a ledge to get too. We will also be showing UI with this
								SetLedge(_ledge, _normal, DetermineVaultType(_ledge));
								return;
							}
						}
					}
				}
			}

			DeleteLedge();
		}

		private VaultType DetermineVaultType(Vector3 _ledgePosition)
		{
			Vector3 _origin = _ledgePosition;
			_origin += transform.forward * distanceToCheck;
			_origin.y = transform.position.y + maxHeight;
			
			RaycastHit _raycastHit;

			if(Physics.Raycast(_origin, Vector3.down, out _raycastHit, maxHeight))
			{
				float _heightDifference = _raycastHit.point.y > transform.position.y ? _raycastHit.point.y - transform.position.y : transform.position.y - _raycastHit.point.y;
				return _heightDifference <= minHeight ? VaultType.Over : VaultType.Mount;
			}

			return VaultType.Over;
		}

        //Checks the slope of the wall we want to vault/ climb over
		private bool CheckWallSlope(RaycastHit hit)
		{
			float _angle = Vector3.Angle(hit.normal, Vector3.up);

			if(_angle > minimumWallSlope)
			{
				return true;
			}

			return false;
		}

        /// <summary>
        /// Checks the slop of the floor we are trying to mount
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
		private bool CheckFloorSlope(RaycastHit hit)
		{
			float _angle = Vector3.Angle(hit.normal, Vector3.up);

			if(_angle < minimumFloorSlope)
			{
				return true;
			}

			return false;
		} 

		private void SetLedge(Vector3 ledgePoint, Vector3 normal, VaultType type)
		{
			validLedge = true;
			ledge = ledgePoint;
			vaultType = type;

			wallNormal = normal;

			if(ledgeDetectedEvent != null)
			{
				ledgeDetectedEvent.Invoke();
			}
		}

		private void DeleteLedge()
		{
			validLedge = false;
			ledge = Vector3.zero;

			if(noLedgeDetectedEvent != null)
			{
				noLedgeDetectedEvent.Invoke();
			}
		}

		/// <summary>
		/// Draw the ledge positno if its good
		/// </summary>
		private void OnDrawGizmos()
		{
			if(validLedge)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(ledge, Vector3.one * 0.15f);
			}
		}
	}
}
