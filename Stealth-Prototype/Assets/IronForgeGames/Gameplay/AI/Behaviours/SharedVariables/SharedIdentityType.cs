using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AH.Max.System;


namespace BehaviorDesigner.Runtime
{
    public class SharedIdentityType : SharedVariable<IdentityType>
    {
        public static implicit operator SharedIdentityType(IdentityType value) { return new SharedIdentityType { mValue = value }; }

    }
}
