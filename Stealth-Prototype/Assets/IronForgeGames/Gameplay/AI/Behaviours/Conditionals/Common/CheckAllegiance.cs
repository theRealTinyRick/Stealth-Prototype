using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/")]
    public class CheckAllegiance : Conditional
    {
        public SharedAllegiance allegiance;

        public Allegiance checkAllegianceAgainst;

        public override TaskStatus OnUpdate()
        {
            if(allegiance.Value == checkAllegianceAgainst)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }

}
