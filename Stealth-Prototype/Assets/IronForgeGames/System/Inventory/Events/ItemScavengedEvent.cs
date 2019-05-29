using UnityEngine.Events;
using System;

namespace AH.Max.System.Inventory
{
    [Serializable]
    public class ItemScavengedEvent : UnityEvent<ItemType>
    {
        // this is an event that we can use to respond to an item being collected
    }
}
