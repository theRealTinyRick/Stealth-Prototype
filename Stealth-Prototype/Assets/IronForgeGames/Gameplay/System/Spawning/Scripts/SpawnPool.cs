using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System
{
	public class SpawnPool : MonoBehaviour 
	{
		public IdentityType spawnPoolType;

		public List <GameObject> pool = new List <GameObject>();

		private void Start ()
		{
			Initialize();
		}

		// add this pool to the spawn manager
		public void Initialize()
		{
			// No logic here	
		}

		// remove from the spawn manager
		private void OnDestroy()
		{
			SpawnManager.Instance.spawnPools.Remove(this);
		}

		///<Summary>
		/// checks if the pool has the enity then adds it to the pool
		///</Summary>
		public void Add(GameObject entity)
		{
			if(!pool.Contains(entity))
			{
				pool.Add(entity);
			}
		}

		///<Summary>
		/// removes entity from the spawn pool
		///</Summary>
		public void Remove(GameObject entity)
		{
			if(pool.Contains(entity))
			{
				pool.Remove(entity);
			}
		}
	}
}
