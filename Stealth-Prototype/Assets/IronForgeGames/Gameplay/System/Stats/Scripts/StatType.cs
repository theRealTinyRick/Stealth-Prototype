using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System
{
	[CreateAssetMenu(fileName = "New StatType", menuName = "CompanyName/Stat", order = 1)]
	public class StatType : SerializedScriptableObject
	{
		public string name;
	}
}
