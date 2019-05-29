using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AH.Max; 
using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay.AI
{
    [RequireComponent(typeof(SphereCollider))]
    public class AIManager : MonoBehaviour
    {
        private static AIManager _instance;
        public static AIManager Instance
        {
            get { return _instance; }
        }

        [ShowInInspector]
        [TabGroup(Tabs.RuntimeInformation)]
        private List <IEnemy> enemies;
        // {
        //     get 
        //     {
        //         return EntityManager.Instance.Enemies;
        //     }
        // }

        [ShowInInspector]
        [TabGroup(Tabs.RuntimeInformation)]
        private List <IEnemy> aggressiveEnemies = new List <IEnemy> ();
        public List <IEnemy> AggressiveEnemies
        {
            get { return aggressiveEnemies; }
        } 

        [SerializeField]
        [TabGroup(Tabs.Preferences)]
        private float switchTime;

        [ShowInInspector]
        [TabGroup(Tabs.RuntimeInformation)]
        private float time = 0.0f;
        
        private Transform playerTransform;
        public Transform PlayerTransform
        {
            get
            {
                if(playerTransform == null)
                {
                    playerTransform = EntityManager.Instance.Player.transform;
                }

                return playerTransform;
            }
        }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            SetToPlayersPosition();
            UpdateActiveEnemies();
        }

        private void UpdateActiveEnemies()
        {
            time += Time.deltaTime;

            if(time >= switchTime)
            {
                time = 0.0f;

                SwitchActiveEnemies();
            }
        }

        private void SwitchActiveEnemies()
        {
            aggressiveEnemies.Clear();

            if(enemies.Count <= 0) return;

            int numberOfAggresiveEnemies = FindNumberOfEnemies();

            if(numberOfAggresiveEnemies == 1)
            {
                aggressiveEnemies.Add(enemies[0]);
                return;
            }
            else
            {
                while(aggressiveEnemies.Count < numberOfAggresiveEnemies)
                {
                    int index = Random.Range(0, enemies.Count - 1);
                    if(index < 0) index = 0;

                    if(!aggressiveEnemies.Contains(enemies[index]))
                    {
                        aggressiveEnemies.Add(enemies[index]);
                    }
                }
            }
        }

        private int FindNumberOfEnemies()
        {
            int numberOfAggresiveEnemies = enemies.Count/3;

            if(numberOfAggresiveEnemies < 1 && numberOfAggresiveEnemies > 0) return 1;
            
            return numberOfAggresiveEnemies;
        }

        private void UpdateEnemyList(IEnemy enemy, bool remove = false)
        {
            if(!remove)
            {
                if(!enemies.Contains(enemy))
                {
                    enemies.Add(enemy);
                }
            }
            else
            {
                if(enemies.Contains(enemy))
                {
                    enemies.Remove(enemy);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IEnemy _other = GetComponent<IEnemy>();

            if(_other != null)
            {
                UpdateEnemyList(_other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IEnemy _other = GetComponent<IEnemy>();

            if(_other != null)
            {
                UpdateEnemyList(_other, remove: true);
            }
        }

        //this is to set the objects position to the player's
        private void SetToPlayersPosition()
        {
            if(EntityManager.Instance.Player)
            {
                Transform _playerTransform = PlayerTransform;

                if(_playerTransform)
                {
                    transform.position = PlayerTransform.position;
                    transform.rotation = PlayerTransform.rotation;
                }
            }
        }
    }
}
