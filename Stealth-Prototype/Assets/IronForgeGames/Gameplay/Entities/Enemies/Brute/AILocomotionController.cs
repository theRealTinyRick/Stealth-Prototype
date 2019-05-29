using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[RequireComponent(typeof(Animator))]
public class AILocomotionController : MonoBehaviour 
{
	public const string Focused = "Focused";
	public const string Horizontal = "Horizontal";
	public const string Vertical = "Vertical";

	[TabGroup("Animation")]
	[SerializeField]
	private float verticalAnimatorFloat = 0;

	[TabGroup("Animation")]
	[SerializeField]
	private float horizontalAnimatorFloat = 0;

	[TabGroup("Animation")]
	[SerializeField]
	[Range(0, 1)]
	private int focusedFloat = 0; // for testing purposes

	private Vector3 forwardDirection;
	private Vector3 moveDirection;

	private Animator animator;

	void Start () 
	{
		if(animator == null)
		{
			animator = transform.root.GetComponentInChildren<Animator>();
		}
	}
	
	///<Summary>
	/// Call this method in Update on the agent to update the locomotion animatior
	///</Summary>
	///<param name="forwardDirection">The forward vector of the agent Use transform.forward</param> 
	///<param name="moveDirection">The direction the agent is moving Use NavMeshAgent.velocity</param> 
	///<param name="focusedTarget">Is the agent currently supposed to be looking at a target like the player or another focused point?</param>  
	public void UpdateLocomotionAnimation(Vector3 forwardDirection, Vector3 moveDirection, bool focusedTarget)
	{
		Vector3 _crossProduct = Vector3.Cross(forwardDirection, moveDirection);
		float _dotProduct = Vector3.Dot(forwardDirection, moveDirection);

		if(focusedTarget == true)
		{
			verticalAnimatorFloat = moveDirection.magnitude * _dotProduct ;
			horizontalAnimatorFloat = _crossProduct.y;
			focusedFloat = 1;
		}
		else
		{
			verticalAnimatorFloat = moveDirection.magnitude;
			horizontalAnimatorFloat = _crossProduct.y;
			focusedFloat = 0;
		}

		this.forwardDirection = forwardDirection;
		this.moveDirection = moveDirection;

		ApplyAnimationFloat();
	}

	private void ApplyAnimationFloat()
	{
		if(animator != null)
		{
			animator.SetFloat(Focused, focusedFloat);
			animator.SetFloat(Vertical, verticalAnimatorFloat);
			animator.SetFloat(Horizontal, horizontalAnimatorFloat);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, forwardDirection);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, moveDirection);
	}
}
