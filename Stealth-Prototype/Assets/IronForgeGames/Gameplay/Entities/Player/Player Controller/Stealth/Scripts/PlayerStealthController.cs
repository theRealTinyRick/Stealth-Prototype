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

using AH.Max.Gameplay.System.Components;

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
        private LayerMask obstacleLayer;
        
        [TabGroup(Tabs.Events)]
        [SerializeField]
        public StealthModeEnteredEvent stealthModeEnteredEvent = new StealthModeEnteredEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField] 
        public StealthModeExitedEvent stealthModeExitedEvent = new StealthModeExitedEvent();

        public StealthObstacle currentStealthObstacle{get; set;}

        private bool isInStealthMode = false;
        public bool IsInStealthMode
        {
            get
            {
                return isInStealthMode;
            }
        }

        private bool isPeeking = false;
        public bool IsPeeking
        {
            get
            {
                return isPeeking;
            }
        }

        private List <StealthObstacle> stealthObstacles = new List<StealthObstacle>();

        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private List<State> statesCannotEnterStealth = new List<State>();

        // components
        private Animator animator;
        private StateComponent stateComponent;

        void Start()
        {
            animator = GetComponent<Animator>();
            stateComponent = GetComponentInChildren<StateComponent>();
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



            if(Input.GetKeyDown(KeyCode.F))
            {
                Peek();
            }
            
            if(Input.GetKeyUp(KeyCode.F))
            {
                StopPeeking();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(obstacleLayer, other.gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    if(!stealthObstacles.Contains(_stealthObstacle))
                    {
                        stealthObstacles.Add(_stealthObstacle);
                    }
                }
            }    
        }
        
        public void OnTriggerExit(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(obstacleLayer, other.gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    stealthObstacles.Remove(_stealthObstacle);
                }
            }    
        }

        private bool CanEnterStealth()
        {
            return !stateComponent.AnyStateTrue(statesCannotEnterStealth);
        }

        public void InitStealthMode()
        {
            if(CanEnterStealth())
            {
                StealthObstacle _stealthObstacle = FindBestStealthObstacle();
                if(_stealthObstacle != null)
                {
                    EnterStealthMode(_stealthObstacle);
                }
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
            isPeeking = false;
            animator.SetBool("IsCrouching", false);

            if(stealthModeExitedEvent != null)
            {
                stealthModeExitedEvent.Invoke();
            }
        }

        public void Peek()
        {
            if(isInStealthMode /* && is the right kind of stealth*/ &&  CanPeek())
            {
                isPeeking = true;
            }
        }

        public void StopPeeking()
        {
            if(isInStealthMode)
            {
                isPeeking = false;
            }
        }

        private bool CanPeek()
        {
            if(currentStealthObstacle != null)
            {

                float _distance = Vector3Utility.PlanarDistance(transform.position, currentStealthObstacle.GetClosestEdge(transform.position));
                if (_distance < 0.72f)
                {
                    return true;
                }
            }
            return false;
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

        public Vector3 GetPeekPosition (float offsetAmount)
        {
            Vector3 _farthest = currentStealthObstacle.GetFarthestEdge(transform.position);
            Vector3 _closest = currentStealthObstacle.GetClosestEdge(transform.position);

            Debug.DrawRay(transform.position, _closest - _farthest, Color.blue, 100);

            return transform.position + ((_closest - _farthest).normalized * offsetAmount);
        }
    }
}
