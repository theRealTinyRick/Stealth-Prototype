using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max
{
    public class PlayerTool : Entity
    {
        [TabGroup(Tabs.Entity)]
        [SerializeField]
        public WeaponType weaponType;
    }
}
