using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Targeting")]
    public class TargetInRange : Action
    {
        public SharedGameObject agent;

        public SharedGameObject sharedtarget;

        public SharedFloat range;

        public override TaskStatus OnUpdate()
        {
            if (agent == null || sharedtarget == null)
            {
                return TaskStatus.Failure;
            }

            if(Vector3.Distance(agent.Value.transform.position, sharedtarget.Value.transform.position) <= range.Value)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;

        }

    }
}