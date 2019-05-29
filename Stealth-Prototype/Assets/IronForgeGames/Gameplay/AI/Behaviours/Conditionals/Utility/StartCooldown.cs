using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Utility")]
    public class StartCooldown : Action
    {

        public SharedUnityEvent cooldownEvent;


        public override TaskStatus OnUpdate()
        {
            if (cooldownEvent != null)
            {
                cooldownEvent.Value.Invoke();

                return TaskStatus.Success;
            }
            return TaskStatus.Failure;

        }

    }
}