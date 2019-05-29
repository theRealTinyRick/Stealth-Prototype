using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorDesigner.Runtime
{
    public class SharedUnityAction : SharedVariable<UnityAction>
    {
        public static implicit operator SharedUnityAction(UnityAction value) { return new SharedUnityAction { mValue = value }; }

    }
}