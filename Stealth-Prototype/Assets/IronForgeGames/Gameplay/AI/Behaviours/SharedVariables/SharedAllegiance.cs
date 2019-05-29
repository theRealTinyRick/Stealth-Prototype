using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public class SharedAllegiance : SharedVariable<Allegiance>
    {
        public static implicit operator SharedAllegiance(Allegiance value) { return new SharedAllegiance { mValue = value }; }
    }
}
