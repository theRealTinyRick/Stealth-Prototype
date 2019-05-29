using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Utility")]
    public class StopCooldown : Action
    {
        public SharedUnityEvent stopCooldownEvent;

        public override TaskStatus OnUpdate()
        {
            if(stopCooldownEvent != null)
            {
                stopCooldownEvent.Value.Invoke();

                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

    }
}