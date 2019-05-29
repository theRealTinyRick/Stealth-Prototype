using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StateChangedEvent : UnityEvent<GameObject, float>
{
}
