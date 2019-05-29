using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System
{
	public class SpawnableComponent : MonoBehaviour 
	{	
		[SerializeField]
        [TabGroup(Tabs.Properties)]
        public Entity entity;

		[SerializeField]
        [TabGroup(Tabs.Properties)]
		public SpawnedEvent spawnedEvent = new SpawnedEvent();

		[SerializeField]
        [TabGroup(Tabs.Properties)]
		public DespawnedEvent despawnedEvent = new DespawnedEvent();

        [ShowInInspector]
        [TabGroup(Tabs.Properties)]
        private bool spawned;
        public bool Spawned 
        {
            get 
            {
                return spawned;
            }
            set 
            {
                spawned = value;
            }
        }

		private void Start()
		{
			entity = GetComponentInChildren<Entity>();
		}

		public void OnSpawn()
		{
			if(spawnedEvent != null)
			{
				spawnedEvent.Invoke();
			}

            spawned = true;
		}

		public void OnDespawn()
		{
			if(despawnedEvent != null)
			{
				despawnedEvent.Invoke();
			}

            spawned = false;
		}
	}
}