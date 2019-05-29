using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class SimpleLedgeClimber : MonoBehaviour
{
    private const string ClimbAnim = "ClimbLedge";

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private LayerMask climbLayer;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float checkDistance;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float maxClimbHeight;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float minClimbHeight;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float maxCheckAngle;

    [TabGroup("Match Target")]
    [MinMaxSlider(0, 1)]
    [SerializeField]
    Vector2 matchTargetPositionTime;

    [TabGroup("Match Target")]
    [MinMaxSlider(0, 1)]
    [SerializeField]
    Vector2 matchTargetRotationTime;

    [TabGroup("Match Target")]
    [SerializeField]
    public AnimationCurve matchTargetPositionSpeed;

    [TabGroup("Match Target")]
    [SerializeField]
    public AnimationCurve matchTargetRotationSpeed;
   
    [HideInInspector]
    public bool isHoldingJumpButton = false;

    [HideInInspector]
    public bool hasValidLedge = false;

    [HideInInspector]
    public bool isClimbing = false;

    private Coroutine climbCoroutine;
    private Animator animator;

    private Vector3 ledgePoint;
    private Vector3 ledgeNormal;


    private Rigidbody rigidBody;

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        FindLedge();
        InputResponse();
    }

    public void SetInputStatus(bool value)
    {
        isHoldingJumpButton = value;
    }

    private void FindLedge()
    {
        if(!isClimbing)
        {
            Vector3 _origin = transform.position + Vector3.up;
            Vector3 _direction = transform.forward;
            Vector3 _normal = Vector3.zero;
            Vector3 _ledge = Vector3.zero;

            RaycastHit _raycastHit;

            if (Physics.Raycast(_origin, _direction, out _raycastHit, checkDistance, climbLayer))
            {
                // check for space above
                _origin = transform.position + (Vector3.up * maxClimbHeight);
                _normal = _raycastHit.normal;
                _ledge = _raycastHit.point;

                // dont record the raycast
                if (Physics.Raycast(_origin, _direction, checkDistance)) return;

                _origin = (new Vector3(_raycastHit.point.x, _origin.y, _raycastHit.point.z) + (transform.forward * 0.2f));
                _direction = Vector3.down;

                if (Physics.Raycast(_origin, _direction, out _raycastHit, checkDistance, climbLayer))
                {
                    _ledge.y = _raycastHit.point.y;

                    ledgeNormal = _normal;
                    ledgePoint = _ledge;
                    hasValidLedge = true;

                    return;
                }
            }

            ledgeNormal = _normal;
            ledgePoint = _ledge;
            hasValidLedge = false;
        }
    }

    private void InputResponse()
    {
        if(isHoldingJumpButton == true &&  isClimbing == false && climbCoroutine == null && hasValidLedge)
        {
            climbCoroutine = StartCoroutine(Climb());
        }
    }

    private IEnumerator Climb()
    {
        animator.Play(ClimbAnim);
        isClimbing = true;
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;

        yield return new WaitForEndOfFrame();

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(ClimbAnim))
        {
            float _currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            AnimatorUtilites.MatchTargetPosition(animator, HumanBodyBones.RightHand, ClimbAnim, ledgePoint, matchTargetPositionTime.x, matchTargetPositionTime.y, matchTargetPositionSpeed.Evaluate(_currentTime));
            AnimatorUtilites.MatchTargetRotation(animator, ClimbAnim,  Quaternion.LookRotation(-ledgeNormal), matchTargetRotationTime.x, matchTargetRotationTime.y, matchTargetRotationSpeed.Evaluate(_currentTime));
            yield return null;
        }

        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        isClimbing = false;
        climbCoroutine = null;
        yield break;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(ledgePoint, Vector3.one * 0.1f);
    }
}
