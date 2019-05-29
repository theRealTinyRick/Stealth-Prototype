using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorDesigner.Runtime
{
    public class SharedUnityEvent : SharedVariable<UnityEvent>
    {
        public static implicit operator SharedUnityEvent(UnityEvent value) { return new SharedUnityEvent { mValue = value }; }

    }
}