using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.System.Inventory
{
    public class InventoryManager : Singleton_SerializedMonobehaviour<InventoryManager>
    {
        /// <summary>
        /// The actual dictionary
        /// </summary>
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Dictionary<InventoryCategory, Dictionary<ItemType, uint>> Inventory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Add(ItemType item, int amount = 1)
        {
            InventoryCategory _category = GetItemCategory(item);

            if(_category != null)
            {
                Dictionary<ItemType, uint> _inventory;

                if(Inventory.TryGetValue(_category, out _inventory))
                {
                    if(_inventory.ContainsKey(item))
                    {
                        _inventory[item] += (uint)amount;
                    }
                    else
                    {
                        _inventory.Add(item, (uint)amount);
                    }
                }
            }

            Debug.LogError("The item: " + item.name + " could not be added to the Inventory");

            return false;
        }

        public bool Remove(ItemType item, int amount = 1)
        {
            InventoryCategory _category = GetItemCategory(item);

            if (_category != null)
            {
                Dictionary<ItemType, uint> _inventory;

                if (Inventory.TryGetValue(_category, out _inventory))
                {
                    if (_inventory.ContainsKey(item))
                    {
                        _inventory[item] -= (uint)amount;
                    }
                }
            }

            Debug.LogError("The item: " + item.name + " could not removed from the inventory");

            return false;
        }

        public bool Contains(ItemType item)
        {
            InventoryCategory _category = GetItemCategory(item);

            if (_category != null)
            {
                Dictionary<ItemType, uint> _inventory;

                if (Inventory.TryGetValue(_category, out _inventory))
                {
                    if (_inventory.ContainsKey(item))
                    {
                        if(_inventory[item] > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public int GetItemAmount(ItemType item)
        {
            InventoryCategory _category = GetItemCategory(item);

            if (_category != null)
            {
                Dictionary<ItemType, uint> _inventory;

                if (Inventory.TryGetValue(_category, out _inventory))
                {
                    if (_inventory.ContainsKey(item))
                    {
                        return (int)_inventory[item];
                    }
                }
            }

            return 0;
        }
           
        /// <summary>
        /// Get all possible items in the inventory of agiven type
        /// This could be used to "get all the swords" and so on
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetItemsOfType <T>() where T :  class
        {
            Debug.LogError("This method has no implementation");

            return null;
        }

        public InventoryCategory GetItemCategory(ItemType item)
        {
            foreach(InventoryCategory _category in Inventory.Keys)
            {
                if(_category.itemType == item.itemType)
                {
                    return _category;
                }
            }

            Debug.LogError("The given item: " + item.name + " did not return an Inventory Category");
            return null;
        }
    }
}
