using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/")]
    public class SetAlliance : Action {

        public SharedAllegiance allegiance;

        public Allegiance setAllegianceTo;

        public override TaskStatus OnUpdate()
        {
            allegiance.Value = setAllegianceTo;

            return TaskStatus.Success;
        }

    }
}
