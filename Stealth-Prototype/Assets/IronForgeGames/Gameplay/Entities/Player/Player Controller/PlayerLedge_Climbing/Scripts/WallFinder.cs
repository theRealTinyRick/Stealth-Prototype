using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    class WallFinder : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float distanceToCheck;
        public float DistanceToCheck { get { return distanceToCheck; } }

        /// <summary>
        /// The master bool to tell other systems if the player has hit a ledge
        /// Use this in other scripts to determine if we should apply and game logic related to elevation changes
        /// </summary>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private bool validLedge = false;
        public bool ValidLedge { get { return validLedge; } }

        /// <summary>
        /// The actual position of the ledge
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector3 ledge = new Vector3();
        public Vector3 Ledge { get { return ledge; } }

        /// <summary>
        /// The normal of the wall that we are detecting against.
        /// It should be kept in mind that if you use this yo should assume to only use this if there is a validLedge
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector3 wallNormal = new Vector3();
        public Vector3 WallNormal { get { return wallNormal; } }

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public float maxHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public float minHeight;

        /// <summary>
        /// how much does the player need to be facing the wall to climb/vault
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumAngleToWall;

        /// <summary>
        /// Is this wall on too much of a slope (hill?)
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumWallSlope;

        /// <summary>
        /// Checks the slope of the floor/ledge we are climbing up to
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumFloorSlope;

        /// <summary>
        /// The layer mask of the walls we want to detect - we will be marking these in scene
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask wallLayerMask;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private bool validateWall;

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

        private void FixedUpdate()
        {
            FindWall();
        }

        private void FindWall()
        {
            //if( playerStateManager.CurrentState == PlayerState.Traversing ) return;

            Vector3 _origin = transform.position;
            _origin.y += 0.3f;

            Vector3 _direction = transform.forward;

            RaycastHit _hit;

            Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
            if (Physics.Raycast(_origin, _direction, out _hit, distanceToCheck))
            {
                //make sure that the wall we are running into is of the appropraite layer
                if(!LayerMaskUtility.IsWithinLayerMask(wallLayerMask, _hit.transform.gameObject.layer))
                {
                    DeleteLedge();
                    return;
                }

                Vector3 _ledge = _hit.point;
                Vector3 _normal = _hit.normal;

                //if the wall is out side of a reasonable angle from the players forward vector then return
                if (!WallIsInFrontOfPlayer(_normal))
                {
                    DeleteLedge();
                    return;
                }

                //we store this distance so we can forward offset our downward ray cast and not over shoot it
                float _distanceToCheckDown = Vector3.Distance(_origin, _ledge) + 0.1f;

                if (CheckWallSlope(_hit))
                {
                    _origin = transform.position;
                    _origin.y += maxHeight;

                    Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
                    if (!Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck))
                    {
                        _origin = transform.position;

                        _origin += transform.forward * _distanceToCheckDown;
                        _origin.y += maxHeight;

                        if (Physics.Raycast(_origin, Vector3.down, out _hit, maxHeight))
                        {
                            _ledge.y = _hit.point.y;

                            // if the user decided to validate the ledge we find then make sure we are within our height params
                            if(validateWall)
                            {
                                if(!IsWithinHeightParameters(_ledge))
                                {
                                    DeleteLedge();
                                    return;
                                }
                            }

                            if (CheckFloorSlope(_hit))
                            {
                                SetLedge(_ledge, _normal);

                                return;
                            }
                        }
                    }
                }
            }

            DeleteLedge();
        }

        /// <summary>
        /// Set the ledge you want to use on other classes
        /// </summary>
        /// <param name="ledgePoint"></param>
        /// <param name="normal"></param>
        private void SetLedge(Vector3 ledgePoint, Vector3 normal)
        {
            validLedge = true;
            ledge = ledgePoint;

            wallNormal = normal;

            if (ledgeDetectedEvent != null)
            {
                ledgeDetectedEvent.Invoke();
            }
        }

        /// <summary>
        /// Get rid of the detected ledge
        /// </summary>
        private void DeleteLedge()
        {
            validLedge = false;
            ledge = Vector3.zero;

            if (noLedgeDetectedEvent != null)
            {
                noLedgeDetectedEvent.Invoke();
            }
        }

        /// <summary>
        /// Checks the slope of the wall we want to vault/ climb over- comparison against the world up vector. 
        /// Is this wall a hill???? Or is this wall at about a 90 degree ange
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool CheckWallSlope(RaycastHit hit)
        {
            float _angle = Vector3.Angle(hit.normal, Vector3.up);

            if (_angle > minimumWallSlope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the slope of the floor we are trying to mount
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
		private bool CheckFloorSlope(RaycastHit hit)
        {
            float _angle = Vector3.Angle(hit.normal, Vector3.up);

            if (_angle < minimumFloorSlope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the angle to the wall to prevent weird detections and to make sure the player is facing the wall. 
        /// </summary>
        /// <returns></returns>
        private bool WallIsInFrontOfPlayer(Vector3 wallNormal)
        {
            float _angle = Vector3.Angle(wallNormal, -transform.forward);

            return _angle < minimumAngleToWall;
        }

        private bool IsWithinHeightParameters(Vector3 ledge)
        {
            float _playerY = transform.position.y;
            float _ledgeY = ledge.y;
            float _hieghtDifference = _ledgeY - _playerY;

            return (_playerY < _ledgeY) && (_hieghtDifference > minHeight) && (_hieghtDifference < maxHeight);
        }
    }
}
