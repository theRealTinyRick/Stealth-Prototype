using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUtilites
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"> The animator that you are calling this on </param>
    /// <param name="position"> position to move your character to </param>
    /// <param name="rotation"> rotation you what to move your character to </param>
    /// <param name="normalizedStartTime"> the normalized start time in the animation you want to   </param>
    /// <param name="normalizedStopTime"></param>
    /// <param name="positionSpeed">  </param>
    /// <param name="rotationSpeed">  </param>
    public static void MatchTarget(Animator animator, HumanBodyBones bodyPart, string animationName, Transform characterTransform, Vector3 position, Quaternion rotation, float normalizedStartTime, float normalizedStopTime, float speed)
    {
        AnimatorStateInfo _state = animator.GetCurrentAnimatorStateInfo(0);
        MatchTargetPosition(animator, bodyPart, animationName, position, normalizedStartTime, normalizedStopTime, speed);

        if(_state.IsName(animationName) && _state.normalizedTime >= normalizedStartTime && _state.normalizedTime < normalizedStopTime)
        {
            // Vector3 _rootOffset = animator.GetBoneTransform(bodyPart).position - characterTransform.position;
            // Vector3 _targetPosition = position - _rootOffset;
            // characterTransform.position = Vector3.Lerp(characterTransform.position, _targetPosition, positionSpeed);
            // characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, rotation, rotationSpeed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="bodyPart"></param>
    /// <param name="animationName"></param>
    /// <param name="position"></param>
    /// <param name="normalizedStartTime"></param>
    /// <param name="normalizedStopTime"></param>
    /// <param name="speed"></param>
    public static void MatchTargetPosition(Animator animator, HumanBodyBones bodyPart, string animationName, Vector3 position, float normalizedStartTime, float normalizedStopTime, float speed)
    {
        AnimatorStateInfo _state = animator.GetCurrentAnimatorStateInfo(0);

        if(/*_state.IsName(animationName) &&*/ _state.normalizedTime >= normalizedStartTime && _state.normalizedTime < normalizedStopTime)
        {
            Vector3 _rootOffset = animator.GetBoneTransform(bodyPart).position - animator.transform.position;
            Vector3 _targetPosition = position - _rootOffset;
            animator.transform.position = Vector3.Lerp(animator.transform.position, _targetPosition, speed);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animationName"></param>
    /// <param name="rotation"></param>
    /// <param name="normalizedStartTime"></param>
    /// <param name="normalizedStopTime"></param>
    /// <param name="speed"></param>
    public static void MatchTargetRotation(Animator animator, string animationName, Quaternion rotation, float normalizedStartTime, float normalizedStopTime, float speed)
    {
        AnimatorStateInfo _state = animator.GetCurrentAnimatorStateInfo(0);

        if(_state.IsName(animationName) && _state.normalizedTime >= normalizedStartTime && _state.normalizedTime < normalizedStopTime)
        {
            animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, rotation, speed);
        }
    }
}
