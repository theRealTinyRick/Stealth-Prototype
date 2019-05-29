using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System.Inventory
{
    public class ScavengableComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        public ItemType item;

        [TabGroup(Tabs.Properties)]
        public int amount;

        [TabGroup(Tabs.Events)]
        public ItemScavengedEvent scavengedEvent = new ItemScavengedEvent(); 

        public void AddToInventory()
        {
            if(item != null)
            {
                InventoryManager.Instance.Add(item, amount);
            }
        }
    }
}
