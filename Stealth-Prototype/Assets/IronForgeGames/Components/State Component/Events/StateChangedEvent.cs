using System;
using UnityEngine.Events;

namespace AH.Max.Gameplay.System.Components
{
    [Serializable]
    public class StateChangedEvent : UnityEvent<StateChangedEvent, State>
    {
    }
}
