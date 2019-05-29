using UnityEngine;

namespace AH.Max.System.Inventory
{
    [CreateAssetMenu(fileName = "New Invetory Category", menuName = "CompanyName/InventoryCategory", order = 1)]
    public class InventoryCategory : ScriptableObject
    {
        public ItemTypes itemType;
        public string name;
    }
}
