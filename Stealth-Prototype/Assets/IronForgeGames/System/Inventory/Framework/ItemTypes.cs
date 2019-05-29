using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System.Inventory
{
    public enum ItemTypes
    {
        None, /*Describes any generall item for what ever reason, also the default*/
        Weapon, /*Describes a weapon*/
        Consumable, /*Describes an item that can be consumed for an effect*/
        QuestItem, /*Describes an item that can be tied to a quest completion*/
        SingleUsable /*Describes unique items that can only be used once*/
    }
}
