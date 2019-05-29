using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    [Serializable]
    public class WallClimbingStartedEvent : UnityEngine.Events.UnityEvent
    {
    }

    [Serializable]
    public class WallClimbingEndedEvent : UnityEngine.Events.UnityEvent
    {
    }

    public class PlayerWallClimber : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float positionOffSet;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float offsetFromWall = 0.3f;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float climbSpeed = 3f;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public float yOffset;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public LayerMask wallLayerMask;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public LayerMask ledgeLayerMask;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public LayerMask ignoreLayer;

        //events
        [TabGroup(Tabs.Events)]
        public WallClimbingStartedEvent wallClimbingStartedEvent = new WallClimbingStartedEvent();

        [TabGroup(Tabs.Events)]
        public WallClimbingEndedEvent wallClimbingEndedEvent = new WallClimbingEndedEvent();

        private bool isLerping = false;
        private bool inPosition = false;
        private float time = 0.0f;

        private float horitontalInput;
        private float verticalInput;

        private bool hasPlayedAnim = false;
        private bool isClimbing = false;

        private Transform helper;
        private Vector3 startPos;
        private Vector3 targetPos;

        private new Rigidbody rigidbody;
        private PlayerClimbAnimationController playerClimbAnimationController;

        private void Start()
        {
            helper = new GameObject().transform;
            helper.name = "Climb Helper";

            ignoreLayer = ~ignoreLayer;

            rigidbody = GetComponent<Rigidbody>();
            playerClimbAnimationController = GetComponent<PlayerClimbAnimationController>();
        }

        /// <summary>
        /// Called from input events in the inspector
        /// </summary>
        public void Action()
        {
            if(!isClimbing)
            {
                CheckForClimb();
            }
            else if(isClimbing && inPosition && !isLerping)
            {
                Drop();
            }
        }

        private bool CheckForClimb()
        {
            RaycastHit hit;
            Vector3 origin = transform.position + (Vector3.up * 2);

            Debug.DrawRay(origin, transform.forward, Color.green, 10);
            if (Physics.Raycast(origin, transform.forward, out hit, 1, wallLayerMask))
            {
                if(LayerMaskUtility.IsWithinLayerMask(wallLayerMask, hit.collider.gameObject.layer))
                {
                    InitForClimb(hit);
                    return true;
                }
            }

            return false;
        }

        public void InitForClimb(RaycastHit hit)
        {
            InitForClimb(hit.point, hit.normal);
        }

        public void InitForClimb(Vector3 point, Vector3 normal)
        {
            isClimbing = true;
            rigidbody.isKinematic = true;
            helper.transform.rotation = Quaternion.LookRotation(-normal);
            startPos = transform.position;
            targetPos = point + (normal * offsetFromWall) - (Vector3.up * 1);
            time = 0;
            inPosition = false;

            wallClimbingStartedEvent.Invoke();

            playerClimbAnimationController.PlayMountAnim();
        }

        private void Update()
        {
            if (isClimbing)
            {
                Tick(Time.deltaTime);
            }
        }

        public void Tick(float delta)
        {
            if (!inPosition)
            {
                GetInPosition();
                return;
            }

            horitontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (!isLerping)
            {
                hasPlayedAnim = false;

                Vector3 horizontal = helper.right * horitontalInput;
                Vector3 vertical = helper.up * verticalInput;
                Vector3 moveDir = (horizontal + vertical).normalized;

                if (!playerClimbAnimationController.IsInState(PlayerClimbAnimationController.Idle) || moveDir == Vector3.zero || !CanMove(moveDir))
                {
                    return;
                }

                time = 0;
                isLerping = true;
                startPos = transform.position;
                targetPos = helper.position;

                playerClimbAnimationController.PlayMoveAnimation(startPos, targetPos, horitontalInput);
            }
            else
            {
                time += delta * climbSpeed;
                if (time > 1)
                {
                    time = 1;
                    isLerping = false;
                }

                if (!hasPlayedAnim)
                {
                    hasPlayedAnim = true;
                }

                Vector3 cp = Vector3.Lerp(startPos, targetPos, time);
                transform.position = cp;
                transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
            }
        }

        private bool CanMove(Vector3 moveDir)
        {
            Vector3 origin = transform.position;
            float dis = positionOffSet;
            Vector3 dir = moveDir;
            RaycastHit hit;

            Vector3 _wallPoint = new Vector3();
            Vector3 _wallNormal = new Vector3();

            Debug.DrawRay(origin, dir * dis, Color.red, 5);
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayer))
            {
                if(LayerMaskUtility.IsWithinLayerMask(wallLayerMask, hit.collider.gameObject.layer))
                {
                    helper.position = PosWithOffset(origin, hit.point);
                    helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
                else if(LayerMaskUtility.IsWithinLayerMask(ledgeLayerMask, hit.collider.gameObject.layer))
                {
                    _wallPoint = hit.point;
                    _wallNormal = hit.normal;

                    Vector3 _origin = hit.point + (Vector3.up * (positionOffSet * 2));
                    Vector3 _direction = -hit.normal;

                    Debug.DrawRay(_origin, _direction, Color.blue, 10);
                    if(!Physics.Raycast(_origin, _direction, 1, ignoreLayer))
                    {
                        _direction = Vector3.down;

                        Debug.DrawRay(_origin + -hit.normal * 0.1f, _direction, Color.blue, 10);
                        if(Physics.Raycast(_origin + -hit.normal * 0.1f, _direction, out hit, positionOffSet * 2, ignoreLayer))
                        {
                            if(LayerMaskUtility.IsWithinLayerMask(ledgeLayerMask, hit.collider.gameObject.layer))
                            {
                                _wallPoint.y = hit.point.y;
                                SwitchToLedge(targetPos, _wallPoint, _wallNormal);
                        
                                return false;
                            }
                        }
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }

            origin += moveDir * dis;
            dir = helper.forward;
            float dis2 = 1;

            Debug.DrawRay(origin, dir * dis2, Color.blue, 5);
            if (Physics.Raycast(origin, dir, out hit, dis2, ignoreLayer))
            {
                if (LayerMaskUtility.IsWithinLayerMask(wallLayerMask, hit.collider.gameObject.layer))
                {
                    helper.position = PosWithOffset(origin, hit.point);
                    helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Debug.Log("Look for lesdge");

                //RaycastHit _ledgeHit;
                //Vector3 _direction = Vector3.down;
                //Debug.DrawRay(origin + transform.forward, _direction, Color.green, 10);
                //if (Physics.Raycast(origin + transform.forward, _direction, out _ledgeHit, positionOffSet, ignoreLayer))
                //{
                //    Debug.Log("Hits ledge");
                //    if (LayerMaskUtility.IsWithinLayerMask(ledgeLayerMask, _ledgeHit.collider.gameObject.layer) ||
                //        LayerMaskUtility.IsWithinLayerMask(wallLayerMask, _ledgeHit.collider.gameObject.layer))
                //    {
                //        _wallPoint.y = _ledgeHit.point.y;
                //        SwitchToLedge(targetPos, _wallPoint, _wallNormal);

                //        return false;
                //    }
                //}
            }

            return false;
        }

        private void GetInPosition()
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                time = 1;
                inPosition = true;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, time * 9);
            transform.position = tp;

            tp.y = transform.position.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, Time.deltaTime * 5);
        }

        private Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }

        private Vector3 LedgeWithOffset(Vector3 ledge)
        {
            ledge += -transform.forward * 0.1f;
            ledge.y -= yOffset;
            return ledge;
        }

        private void SwitchToLedge(Vector3 previousPoint, Vector3 ledgePoint, Vector3 normal)
        {
            PlayerLedgeClimber _ledgeClimber = GetComponent<PlayerLedgeClimber>();
            if(_ledgeClimber != null)
            {
                isClimbing = false;
                inPosition = false;
                isLerping = false;

                wallClimbingEndedEvent.Invoke();

                playerClimbAnimationController.Dismount();
                playerClimbAnimationController.PlayMoveAnimation(previousPoint, ledgePoint, horitontalInput);

                _ledgeClimber.InitClimb(ledgePoint, normal);
            }
        }

        public void Drop()
        {
            rigidbody.isKinematic = false;
            isClimbing = false;
            inPosition = false;
            isLerping = false;

            wallClimbingEndedEvent.Invoke();

            playerClimbAnimationController.Dismount();
        }
    }
}

