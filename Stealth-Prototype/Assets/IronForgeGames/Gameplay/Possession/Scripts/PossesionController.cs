using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using AH.Max.System;

using AH.Max.Gameplay.Camera;

namespace AH.Max.Gameplay
{
    public class PossessionStartedEvent : UnityEngine.Events.UnityEvent<Entity>
    {
    }

    public class PossessionEndedEvent : UnityEngine.Events.UnityEvent<Entity>
    {
    }

    public class PossesionController : MonoBehaviour
    {
        public Entity playerEntity;
        public Vector3 playerPosition;
        public Quaternion playerRotation;

        public Entity target;

        public CameraManager cameraManager;
        public TargetingManager targetingManager;

        public Entity currentlyPossessedEntity;
        public bool possessing = false;

        public List <IdentityType> possessableIdentities = new List<IdentityType>();

        private void OnEnable()
        {
            targetingManager.lockedOnEvent.AddListener(SetTarget);
            targetingManager.lockedOffEvent.AddListener(RemoveTarget);
        }

        private void OnDisable()
        {
            targetingManager.lockedOnEvent.RemoveListener(SetTarget);
            targetingManager.lockedOffEvent.RemoveListener(RemoveTarget);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if (!possessing && currentlyPossessedEntity == null && target)
                {
                    StartPossessing(target);
                }
                else if(currentlyPossessedEntity != null && possessing)
                {
                    StopPossession(); 
                }
            }
        }

        private void SetTarget(Entity target)
        {
            this.target = target;
        }

        private void RemoveTarget(Entity target)
        {
            if(!possessing && currentlyPossessedEntity)
            {
                this.target = null;
            }
        }

        public void StartPossessing(Entity target)
        {
            if(!possessing && currentlyPossessedEntity == null && target)
            {
                GameObject _spawnedPossessed = SpawnManager.Instance.Spawn(GetPossessedIdentity(target.IdentityType));
                Entity _spawnedEntity = _spawnedPossessed.GetComponent<Entity>();

                if(_spawnedPossessed != null)
                {
                    playerRotation = playerEntity.transform.rotation;
                    playerPosition = playerEntity.transform.position;

                    _spawnedPossessed.transform.position = target.transform.position;
                    _spawnedPossessed.transform.rotation = target.transform.rotation;

                    cameraManager.SetCameraTarget(_spawnedPossessed.transform);
                    targetingManager.LockOff();

                    SpawnManager.Instance.Despawn(target);
                    SpawnManager.Instance.Despawn(playerEntity);

                    possessing = true;

                    if(_spawnedEntity != null)
                    {
                        currentlyPossessedEntity = _spawnedEntity;
                    }
                }
            }
        }

        public void StopPossession()
        {
            if(currentlyPossessedEntity != null && possessing)
            {
                GameObject _player = SpawnManager.Instance.Spawn(playerEntity.IdentityType);

                if (_player != null)
                {
                    _player.transform.position = playerPosition;
                    _player.transform.rotation = playerRotation;

                    cameraManager.SetCameraTarget(playerEntity.transform);

                    SpawnManager.Instance.Despawn(currentlyPossessedEntity);

                    GameObject _oldEnemy = SpawnManager.Instance.Spawn(target.IdentityType);

                    if(_oldEnemy != null)
                    {
                        _oldEnemy.transform.position = currentlyPossessedEntity.transform.position;
                        _oldEnemy.transform.rotation = currentlyPossessedEntity.transform.rotation;
                    }

                    possessing = false;
                    currentlyPossessedEntity = null;
                    target = null;
                }
            }
        }

        public IdentityType GetPossessedIdentity(IdentityType parentOfReturnValue)
        {
            return possessableIdentities.Find(_identity => _identity.parent == parentOfReturnValue);
        }
    }
}
