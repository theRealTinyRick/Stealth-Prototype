using UnityEngine;

namespace AH.Max.System.Inventory
{
    public class ItemConsumableComponent : MonoBehaviour
    {
        public ItemType item;
        public int amount;

        public void AddToInventory()
        {
            if (item != null)
            {
                if(amount > 0)
                {
                    InventoryManager.Instance.Remove(item, amount);
                } 
                else
                {
                    InventoryManager.Instance.Remove(item);
                }
            }
        }
    }
}
