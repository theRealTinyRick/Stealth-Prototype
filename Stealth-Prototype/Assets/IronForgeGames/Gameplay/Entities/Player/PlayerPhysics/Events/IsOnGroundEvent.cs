using System;
using UnityEngine;

/// <summary>
/// An event inetended to pass across the object the player is on
/// </summary>
[Serializable]
public class IsOnGroundEvent : UnityEngine.Events.UnityEvent<GameObject>
{
}