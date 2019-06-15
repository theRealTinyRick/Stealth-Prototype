using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerClimbAnimationController : MonoBehaviour
{
    public const string IsClimbingAnimBool = "IsClimbingLedge";

    public const string LedgeXAnimationFloat = "LedgeX";
    public const string LedgeYAnimationFloat = "LedgeY";

    public const string LedgeMoveAnim = "LedgeMove";
    public const string WallClimbAnimation = "WallMove";

    public const string Idle = "ClimbIdle";

    public const string VERTICAL = "Vertical";
    public const string HORIZONTAL = "Horizontal";

    public const string CLIMB_UP = "ClimbUp";

    public const string CORNER_DIRECTION = "CornerDirection";
    public const string CORNER_IN = "CornerIn";
    public const string CORNER_OUT = "CornerOut";

    private Animator animator;

	private void Start ()
    {
        animator = GetComponent<Animator>();
    }
	
    public void PlayMountAnim()
    {
        animator.SetBool(IsClimbingAnimBool, true);
    }

    public void PlayLedgeMoveAnimation(Vector3 ledgePoint, float inputXValue)
    {
        Vector3 _up = transform.up;
        Vector3 _toPosition = ledgePoint - transform.position;

        float xValue = inputXValue;
        float yValue = Vector3.Dot(_up, _toPosition);

        animator.SetFloat(LedgeXAnimationFloat, xValue);
        animator.SetFloat(LedgeYAnimationFloat, yValue);

        animator.Play(LedgeMoveAnim);
    }

    /// <summary>
    /// plays an animation
    /// </summary>
    /// <param name="lastPosition"></param>
    /// <param name="newPosition"></param>
    /// <param name="inputXValue"></param>
    public void PlayMoveAnimation(Vector3 lastPosition, Vector3 newPosition, float inputXValue)
    {
        Vector3 _up = transform.up;
        Vector3 _toPosition = newPosition - lastPosition;

        float xValue = inputXValue;
        float yValue = Vector3.Dot(_up, _toPosition);

        animator.SetFloat("ClimbVertical", xValue);
        animator.SetFloat("ClimbHorizontal", yValue);

        animator.Play(WallClimbAnimation);
    }
     
    public void PlayClimbUpAnimation()
    {
        animator.SetFloat(HORIZONTAL, 0);
        animator.SetFloat(VERTICAL, 0);

        animator.SetTrigger(CLIMB_UP);
    }

    public void PlayCornerIn(float direction)
    {
        SetCornerDirection(direction);
        animator.Play(CORNER_IN);
    }

    public void PlayCornerOut(float direction)
    {
        SetCornerDirection(direction);
        animator.Play(CORNER_OUT);
    }

    private void SetCornerDirection(float direction)
    {
        if (direction < 0) direction = 0;
        animator.SetFloat(CORNER_DIRECTION, direction);
    }

    public void Dismount()
    {
        animator.SetBool(IsClimbingAnimBool, false);
    }

    public bool IsInState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
