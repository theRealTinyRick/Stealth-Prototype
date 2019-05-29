using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    public class PlayerLedgeClimber : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float distanceToCheck;
        public float DistanceToCheck { get { return distanceToCheck; } }

        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector3 wallNormal = new Vector3();
        public Vector3 WallNormal { get { return wallNormal; } }

        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector2 playerOffset = new Vector3();
        public Vector2 PlayerOffset { get { return wallNormal; } }

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxLedgeShimyHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minLedgeShimyHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxAirMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumWallSlope;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumFloorSlope;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float ledgeClimbDistance;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        [Range(0, 1)]
        private float climbSpeed;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        [Range(0, 1)]
        private float mountSpeed;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask ledgeLayerMask;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask wallLayerMask;

        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float animationMatchTargetTime;


        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float animationMatchTargetEndTime;

        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float animationMatchTargetSpeed;

        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float animationEndDelay;

        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float climbUpForwardOffset;

        [TabGroup("Climb Up Ledge")]
        [SerializeField]
        private float climbUpRightOffset;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        LedgeClimbStartEvent ledgeClimbStarted = new LedgeClimbStartEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        LedgeClimbStoppedEvent ledgeClimbStopped = new LedgeClimbStoppedEvent();

        private Vector3 ledge = new Vector3();
        public Vector3 Ledge { get { return ledge; } }

        private Vector3 climbUpPosition = new Vector3();

        private Quaternion ledgeRotation;

        private bool isClimbingUp = false;
        private bool isDismounting;
        private bool isInPosition = false;

        private bool isClimbing = false;
        public bool IsClimbing {get {return isClimbing;}}

        private bool canClimbUp {get {return IsAtNextClimbPoint();}}

        private Rigidbody _rigidbody;
        private PlayerGroundedComponent playerGroundedComponent;
        private PlayerStateComponent playerStateComponent;
        private PlayerClimbAnimationController playerLedgeAnimHook;

        private void Start()
        {
            ledge = Vector3.zero;

            _rigidbody = GetComponent<Rigidbody>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
            playerLedgeAnimHook = GetComponent<PlayerClimbAnimationController>();

            InputDriver.jumpButtonEvent.AddListener(InputResponse);
            InputDriver.jumpButtonHeldEvent.AddListener(InputResponse);
        }

        private void OnDisable()
        {
            InputDriver.jumpButtonEvent.RemoveListener(InputResponse);
            InputDriver.jumpButtonHeldEvent.RemoveListener(InputResponse);
        }

        private void FixedUpdate()
        {
            DetectLedgePoint();

            if (!isClimbingUp)
            {
                MoveToPosition();
            }

            if(isInPosition && isClimbing && playerGroundedComponent.IsGrounded && !isClimbingUp)
            {
                Dismount();
            }

            if(!InputDriver.jumpButtonIsBeingHeld)
            {
                isDismounting = false;
            }
        }

        private void InputResponse()
        {
            if(isInPosition && isClimbing)
            {
                if (InputDriver.LocomotionDirection.normalized.z > 0 && !isClimbingUp)
                {
                    StartCoroutine(ClimbupLedge());
                }
                else if (IsAtNextClimbPoint())
                {
                    //Dismount();
                }
            }
            else
            {
                if(!isClimbing && !isDismounting)
                {
                    InitClimb();
                }
            }
        }

        private void InitClimb()
        {
            if(CheckValidLedge())
            {
                isClimbing = true;
                _rigidbody.isKinematic = true;
                StartCoroutine(GetInPosition(LedgeWithPlayerOffset(ledge), wallNormal));
                playerLedgeAnimHook.PlayMountAnim();
                if (ledgeClimbStarted != null)
                {
                    ledgeClimbStarted.Invoke();
                }
            }
        }

        public void InitClimb(Vector3 ledgePoint, Vector3 normal)
        {
            SetLedge(ledgePoint, normal);

            isClimbing = true;
            _rigidbody.isKinematic = true;
            StartCoroutine(GetInPosition(LedgeWithPlayerOffset(ledge), wallNormal));
            playerLedgeAnimHook.PlayMountAnim();
            if(ledgeClimbStarted != null)
            {
                ledgeClimbStarted.Invoke();
            }
        }

        private void Dismount()
        {
            isDismounting = true;
            isClimbingUp = false;
            isClimbing = false;
            isInPosition = false;
            _rigidbody.isKinematic = false;
            ledge = Vector3.zero;
            playerLedgeAnimHook.Dismount();

            ResetRotation();
            
            if(ledgeClimbStopped != null)
            {
                ledgeClimbStopped.Invoke();
            }
        }

        private IEnumerator ClimbupLedge()
        {
            if (canClimbUp)
            {
                _rigidbody.isKinematic = true;
                isClimbingUp = true;

                playerLedgeAnimHook.PlayClimbUpAnimation();

                Animator _animator = GetComponent<Animator>();
                Vector3 position = climbUpPosition + (transform.forward * climbUpForwardOffset) + (transform.right * climbUpRightOffset);
                float _time = 0;

                while (_time < animationMatchTargetTime)
                {
                    AnimatorUtilites.MatchTarget(_animator, HumanBodyBones.RightHand, "Ledge_Hang_ToStand_Up", transform, position, transform.rotation, 0, animationMatchTargetEndTime, animationMatchTargetSpeed);
                    _time += Time.deltaTime;
                    yield return null;
                }

                playerLedgeAnimHook.Dismount();
                yield return new WaitForSeconds(animationEndDelay);

                Dismount();
                yield break;
            }
        }

        private void ResetRotation()
        {
            Quaternion _rotation = Quaternion.Euler(0, transform.root.rotation.y, 0);
        }

        public bool CheckValidLedge()
        {
            Vector3 _rayCastOrigin = transform.position;
            _rayCastOrigin.y += 1;

            Vector3 _horizontaHit = new Vector3();
            
            Vector3 _normal = new Vector3();
            RaycastHit _hitResult;

            Debug.DrawRay(_rayCastOrigin, transform.forward, Color.red, 5);
            if(Physics.Raycast(_rayCastOrigin, transform.forward, out _hitResult, 1/*, layerMask*/))
            {
                if(LayerMaskUtility.IsWithinLayerMask(ledgeLayerMask, _hitResult.collider.gameObject.layer))
                {
                    _rayCastOrigin += Vector3.up * maxMountHeight;
                    Debug.DrawRay(_rayCastOrigin, transform.forward * 2, Color.blue, 5);
                    if(!Physics.Raycast(_rayCastOrigin, transform.forward, 2))
                    {
                        _normal = _hitResult.normal;
                        _horizontaHit = _hitResult.point;

                        _rayCastOrigin = _hitResult.point + Vector3.up * maxMountHeight + transform.forward * 0.2f;

                        Debug.DrawRay(_rayCastOrigin, Vector3.down, Color.red, 5);
                        if(Physics.Raycast(_rayCastOrigin, Vector3.down, out _hitResult, 10/*, layerMask*/))
                        {
                            // float _floorHit = _hitResult.point.y;
                            // float _ledgeHeight = wallFinder.Ledge.y;

                            // if(_floorHit < _ledgeHeight)
                            // {
                            //     float _heightDifference = _ledgeHeight - _floorHit;

                            //     float _maxHeight = playerGroundedComponent.IsGrounded ? maxMountHeight : maxAirMountHeight;
                            //     float _minHeight = playerGroundedComponent.IsGrounded ? minMountHeight : 0;

                            //     if(_heightDifference > _minHeight && _heightDifference < _maxHeight)
                            //     {
                            //         return true;
                            //     }
                            // }
                            SetLedge(new Vector3(_horizontaHit.x, _hitResult.point.y, _horizontaHit.z), _normal);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private IEnumerator GetInPosition(Vector3 position, Vector3 wallNormal)
        {
            Quaternion _rotation = Quaternion.LookRotation(-wallNormal);
            while(Vector3.Distance(transform.position, position) > 0.1f || InputDriver.jumpButtonIsBeingHeld)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, 0.5f);
                transform.position = Vector3.MoveTowards(transform.position, position, mountSpeed);

                yield return new WaitForFixedUpdate();
            }

            isInPosition = true;

            yield break;
        }

       

        private bool IsAtNextClimbPoint()
        {
            if (isInPosition && isClimbing)
            {
                float _distance = Vector3.Distance(transform.position, LedgeWithPlayerOffset(ledge));
                if(_distance < 0.01f)
                {
                    if(climbUpPosition != Vector3.zero)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPlayerInTheIdleState()
        {
            return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(PlayerClimbAnimationController.Idle);
        }

        private void DetectLedgePoint()
        {
            if(isInPosition && isClimbing && InputDriver.LocomotionDirection.x != 0 && IsAtNextClimbPoint() && !isClimbingUp && IsPlayerInTheIdleState())
            {
                Vector3 _origin = transform.position;
                _origin.y += 1f;

                RaycastHit _hit;
                Debug.DrawRay(_origin, transform.forward, Color.green, 10);
                if (Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck))
                {
                    if (LayerMaskUtility.IsWithinLayerMask(ledgeLayerMask, _hit.collider.gameObject.layer))
                    {
                        Vector3 _hitAngle = -transform.forward;

                        float _distanceToCheckDown = Vector3.Distance(_origin, _hit.point) + 0.1f;

                        if (CheckWallSlope(_hit))
                        {
                            float horizontalInput = InputDriver.LocomotionDirection.normalized.x < 0 ? -1 : 1;

                            Vector3 _obsticalDirection = transform.right * horizontalInput;
                            Vector3 _targetOrigin = transform.position;

                            bool _useObstical = false;

                            if (Physics.Raycast(_origin, _obsticalDirection, out _hit, ledgeClimbDistance + 0.5f))
                            {
                                if(LayerMaskUtility.IsWithinLayerMask(wallLayerMask, _hit.collider.gameObject.layer))
                                {
                                    SwitchToWall(ledge, _hit.point, _hit.normal, horizontalInput);
                                    return;
                                }
                                else
                                {
                                    if(CheckWallSlope(_hit))
                                    {
                                        _useObstical = true;
                                    }
                                }
                            }

                            if(_useObstical)
                            {
                                _targetOrigin = _hit.point + _hit.normal;
                                _obsticalDirection = -_hit.normal;
                            }
                            else
                            {
                                _targetOrigin += transform.right * (horizontalInput * ledgeClimbDistance);
                                _obsticalDirection = transform.forward;
                            }

                            _origin = _targetOrigin;

                            if (Physics.Raycast(_origin, _obsticalDirection, out _hit, distanceToCheck + 1))
                            {
                                Vector3 _ledge = _hit.point;
                                Vector3 _normal = _hit.normal;

                                _origin = _hit.point;
                                _origin.y += maxLedgeShimyHeight;
                                _origin += -_normal * _distanceToCheckDown;

                                if (Physics.Raycast(_origin, Vector3.down, out _hit, maxLedgeShimyHeight))
                                {
                                   _ledge.y = _hit.point.y;

                                   if (CheckFloorSlope(_hit))
                                   {
                                        SetLedge(_ledge, _normal);

                                        playerLedgeAnimHook.PlayLedgeMoveAnimation(LedgeWithPlayerOffset(ledge), horizontalInput);
                                        return;
                                   }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SwitchToWall(Vector3 previousPoint, Vector3 newPoint, Vector3 normal, float horizontalInput)
        {
            PlayerWallClimber wallClimber = GetComponent<PlayerWallClimber>();
            if(wallClimber != null)
            {
                isDismounting = true;
                isClimbingUp = false;
                isClimbing = false;
                isInPosition = false;
                ledge = Vector3.zero;

                playerLedgeAnimHook.PlayMoveAnimation(previousPoint, newPoint, horizontalInput);
                playerLedgeAnimHook.Dismount();

                if (ledgeClimbStopped != null)
                {
                    ledgeClimbStopped.Invoke();
                }

                wallClimber.InitForClimb(newPoint, normal);
            }
        }

        private void SetLedge(Vector3 ledgePoint, Vector3 normal)
        {
            ledge = ledgePoint;
            wallNormal = normal;

            Quaternion _rot = Quaternion.LookRotation(-normal);
            ledgeRotation = _rot;

            climbUpPosition = ledge;
        }

        private void MoveToPosition()
        {
            if(isClimbing && isInPosition && ledge != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, LedgeWithPlayerOffset(ledge), climbSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, ledgeRotation, 0.2f);
            }
        }

        //Checks the slope of the wall we want to vault/ climb over
        private bool CheckWallSlope(RaycastHit hit)
        {
            float _angle = Vector3.Angle(hit.normal, Vector3.up);

            if (_angle >= minimumWallSlope)
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

            if (_angle < minimumFloorSlope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledge"></param>
        /// <returns></returns>
        private Vector3 LedgeWithPlayerOffset(Vector3 ledge)
        {
            ledge += WallNormal * playerOffset.x;
            ledge += Vector3.up * playerOffset.y;
            return ledge;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(climbUpPosition, 0.1f);
        }
    }
}