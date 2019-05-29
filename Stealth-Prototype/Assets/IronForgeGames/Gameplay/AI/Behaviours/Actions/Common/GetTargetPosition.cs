using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Targeting")]
    public class GetTargetPosition : Action
    {
        public SharedGameObject agent;

        public SharedGameObject sharedTarget;

        public SharedVector3 sharedTargetPosition;

        public override TaskStatus OnUpdate()
        {
            if (agent == null || sharedTarget == null)
            {
                return TaskStatus.Failure;
            }

            sharedTargetPosition.Value = sharedTarget.Value.transform.position;

            return TaskStatus.Success;

        }

    }
}