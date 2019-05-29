/*
Author: Aaron Hines
Edits By:
Description: 
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace AH.Max.Gameplay.Stealth
{
    public enum StealthMode
    {
        Hidden,
        NotHidden
    }

    [Serializable]
    public class StealthModeEnteredEvent : UnityEngine.Events.UnityEvent { }
    
    [Serializable]
    public class StealthModeExitedEvent : UnityEngine.Events.UnityEvent { }

    public class PlayerStealthController : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public StealthMode stealthMode;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask stealthObstacleLayerMask;
        
        [TabGroup(Tabs.Events)]
        [SerializeField]
        public StealthModeEnteredEvent stealthModeEnteredEvent = new StealthModeEnteredEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField] 
        public StealthModeExitedEvent stealthModeExitedEvent = new StealthModeExitedEvent();

        public StealthObstacle currentStealthObstacle{get; set;}

        private bool isInStealthMode = false;

        private List <StealthObstacle> stealthObstacles = new List<StealthObstacle>();

        // components
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                if(isInStealthMode)
                {
                    ExitStealthMode();
                }
                else
                {
                    InitStealthMode();
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(stealthObstacleLayerMask, other.gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    stealthObstacles.Add(_stealthObstacle);
                }
            }    
        }
        
        public void OnTriggerExit(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(stealthObstacleLayerMask, other.gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    stealthObstacles.Remove(_stealthObstacle);
                }
            }    
        }

        public void InitStealthMode()
        {
            StealthObstacle _stealthObstacle = FindBestStealthObstacle();
            if(_stealthObstacle != null)
            {
                EnterStealthMode(_stealthObstacle);
            }
        }

        public void EnterStealthMode(StealthObstacle stealthObstacle)
        {
            // TODO: add logic to make sure that we have not already been seen by an enemy clode by!!!!
            if(stealthObstacle != null && stealthObstacle.hidingPlaces.Count > 0)
            {
                currentStealthObstacle = stealthObstacle;
                Vector3 _vector = stealthObstacle.GetClosestHidingPlace(transform.position);
                Quaternion _rotation = Quaternion.LookRotation(stealthObstacle.GetInTightestAngle(transform) - stealthObstacle.GetInWidestAngle(transform));

                if(Vector3.Distance(transform.position, _vector) < 3f)
                {
                    if(stealthModeEnteredEvent != null)
                    {
                        stealthModeEnteredEvent.Invoke();
                    }

                    isInStealthMode = true;
                    stealthMode = StealthMode.Hidden;
                    
                    StartCoroutine(GetIntoPosition(_vector, _rotation));
                    animator.SetBool("IsCrouching", true);
                }
            }
        }

        public void ExitStealthMode()
        {
            isInStealthMode = false;
            animator.SetBool("IsCrouching", false);

            if(stealthModeExitedEvent != null)
            {
                stealthModeExitedEvent.Invoke();
            }
        }
        
        IEnumerator GetIntoPosition(Vector3 position, Quaternion rotation)
        {
            while(Vector3.Distance(transform.position, position) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, position, 0.3f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.3f);
                yield return null;
            }
            yield break;
        }

        public StealthObstacle FindBestStealthObstacle()
        {
            if(stealthObstacles.Count >  0)
            {
                StealthObstacle _closest =  stealthObstacles[0];
                float _clostestDistance = Vector3.Distance(transform.position, _closest.transform.position);
                foreach(StealthObstacle _obstacle in stealthObstacles)
                {
                    float _distance = Vector3.Distance(transform.position, _obstacle.transform.position);
                    if(_distance < _clostestDistance)
                    {
                        _clostestDistance = _distance;
                        _closest = _obstacle;
                    }
                }

                return _closest;
            }
            return null;
        }
    }
}
