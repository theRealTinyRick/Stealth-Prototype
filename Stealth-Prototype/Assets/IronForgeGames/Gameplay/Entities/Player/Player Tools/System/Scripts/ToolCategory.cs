using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System
{
    [CreateAssetMenu(fileName = "New Tool Category", menuName = "CompanyName/Tool Category", order = 1)]
    public class ToolCategory : SerializedScriptableObject
    {
        public string name;
    }
}
