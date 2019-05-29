using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max
{
	[CreateAssetMenu(fileName = "New Level Data", menuName = "CompanyName/LevelData", order = 1)]
	public class LevelData : ScriptableObject
	{
		public string name;
		public List <LevelData> dependencies;
	}
}