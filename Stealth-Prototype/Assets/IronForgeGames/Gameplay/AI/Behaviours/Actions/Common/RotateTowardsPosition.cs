using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Locomotion")]
    public class RotateTowardsPosition : Action
    {
        public SharedGameObject agent;

        public SharedVector3 sharedTargetPosition;

        public float rotateSpeed;

        public float angleDifference;

        public bool waitTillDone;

        public override TaskStatus OnUpdate()
        {
            if (agent == null || sharedTargetPosition == null)
            {
                return TaskStatus.Failure;
            }

            Vector3 _targetDirection = sharedTargetPosition.Value - agent.Value.transform.position;

            float _angle = Vector3.SignedAngle(agent.Value.transform.forward, _targetDirection, Vector3.up);

            if (_angle >= -angleDifference && _angle <= angleDifference)
            {
                return TaskStatus.Success;
            }

            Quaternion _rotation = Quaternion.LookRotation(_targetDirection);

            transform.rotation = Quaternion.Lerp(agent.Value.transform.rotation, _rotation, rotateSpeed);

            if (waitTillDone)
            {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}
