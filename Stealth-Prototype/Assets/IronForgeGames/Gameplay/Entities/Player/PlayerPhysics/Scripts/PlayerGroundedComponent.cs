using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerGroundedComponent : MonoBehaviour 
{
	/// <summary>
	/// Determines if the player is on the ground
	/// </summary>
    [TabGroup(Tabs.Properties)]
    [SerializeField]
	private bool isGrounded;
	public bool IsGrounded
	{
		get
		{
			return isGrounded;
		}
	}

    [TabGroup(Tabs.Properties)]
	[SerializeField]
	private Vector3[] raycastOffsets;

    [TabGroup(Tabs.Properties)]
	[SerializeField]
	[Range(0, 1)]
	private float offsetDistance;

    [TabGroup(Tabs.Properties)]
	[SerializeField]
	[Range(0, 1)]
	private float yOffset;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    [Range(0, 2)]
    private float checkDistance;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float maxFloorSlope;

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public IsOnGroundEvent isOnGroundEvent = new IsOnGroundEvent();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public IsNotOnGroundEvent isNotOnGroundEvent = new IsNotOnGroundEvent();

    public const string GroundedBool = "IsGrounded";
	
    private Animator animator;

    private void Awake()
    {
        animator = transform.root.GetComponentInChildren<Animator>();
	}

    private void Update()
    {
        UpdateAnimator();
    }

	private void FixedUpdate() 
	{
		isGrounded = Grounded();
	}

	///<Summary>
	/// Use this method to check if the chracter is reasonalbly on the ground
	///</Summary>
	public bool Grounded()
	{
		if(raycastOffsets == null)
		{
			Debug.LogError("You probably have not set up the raycast offsets on the PlayerGroundedComponent");
            if(isNotOnGroundEvent != null)
            {
                isNotOnGroundEvent.Invoke();
            }
			
			return false;
		} 

		foreach(Vector3 _position in raycastOffsets)
		{
			Vector3 _tp = transform.position + (_position * offsetDistance);
			_tp.y  = transform.position.y + yOffset;
			RaycastHit _hit;
			if(Physics.Raycast(_tp, Vector3.down, out _hit, checkDistance, PhysicsLayers.ingnorePlayerLayer))
			{
                float _floorSlope = Vector3.Angle(_hit.normal, Vector3.up);
                if(_floorSlope < maxFloorSlope)
                {
                    if(isOnGroundEvent != null)
                    {
                        isOnGroundEvent.Invoke(_hit.transform.gameObject);
                    }

				    return true;
                }
			}
		}

        if (isNotOnGroundEvent != null)
        {
            isNotOnGroundEvent.Invoke();
        }

        return false;
	}

    public void UpdateAnimator()
    {
        animator.SetBool(GroundedBool, isGrounded);
    }

	///<Summary>
	/// Draw the positions generating the raycasts
	///</Summary>
	private void OnDrawGizmos()
	{
		if(raycastOffsets == null) return;

		foreach(Vector3 _position in raycastOffsets)
		{
			Gizmos.color = Color.red;
			
			Vector3 _tp = transform.position + (_position * offsetDistance);
			_tp.y  = transform.position.y + yOffset;

			Gizmos.DrawSphere(_tp, 0.05f);
		}
	}	
}
