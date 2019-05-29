using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "CompanyName/Item", order = 1)]
    public class ItemType : ScriptableObject
    {
        public ItemTypes itemType;
        public string name;
    }
}
