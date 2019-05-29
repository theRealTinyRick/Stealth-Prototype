using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay
{
    [Serializable]
    public class LockedOnEvent : UnityEngine.Events.UnityEvent<Entity>
    {
    }

    [Serializable]
    public class LockedOffEvent : UnityEngine.Events.UnityEvent<Entity>
    {
    }

    public class TargetingManager : MonoBehaviour 
	{
		/// <summary>
		/// entities that can be targeted
		/// </summary>
		/// <returns></returns>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private List<Entity> entitiesToTarget = new List<Entity>();

		/// <summary>
		/// the identity types that can be targeted
		/// </summary>
		/// <returns></returns>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private List <IdentityType> targetableIdentities = new List <IdentityType>();

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask targetableLayers;

		/// <summary>
		/// the currently targeted entity
		/// </summary>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private Entity currentTarget;
        public Entity CurrentTarget 
        {
            get 
            {
                return currentTarget;
            }
        }
		
		/// <summary>
		/// the previously targeted entity
		/// </summary>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private Entity previousTarget;

		/// <summary>
		/// Is the current manager currently locked on
		/// </summary>
		[TabGroup(Tabs.Properties)]
		[ShowInInspector]
		private bool lockedOn;
		public bool LockedOn
		{
			get
			{
				return lockedOn;
			}
		}

		[TabGroup(Tabs.Properties)]
        [SerializeField]
		private Transform referenceTransform;

		[TabGroup(Tabs.Properties)]
        [SerializeField]
        public float eyeHeightOffset;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public LockedOnEvent lockedOnEvent;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public LockedOffEvent lockedOffEvent;

        public static LockedOnEvent lockedOnStaticEvent = new LockedOnEvent();
        public static LockedOffEvent lockedOffStaticEvent = new LockedOffEvent();

		private void Start()
		{
			if(referenceTransform == null)
			{
				referenceTransform = transform.root;
			}
		}

        private void OnEnable()
        {
            InputDriver.lockOnButtonEvent.AddListener(RecieveInput);
        }

        private void OnDisable()
        {
            InputDriver.lockOnButtonEvent.RemoveListener(RecieveInput);
        }

        private void RecieveInput()
        {
            if(lockedOn)
            {
                LockOff();
            }
            else
            {
                LockOn();
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                NextTargetToTheLeft();
            }
        }

		/// <summary>
		/// OnTriggerEnter is called when the Collider other enters the trigger.
		/// </summary>
		/// <param name="other">The other Collider involved in this collision.</param>
		private void OnTriggerStay(Collider other)
		{
            if(LayerMaskUtility.IsWithinLayerMask(targetableLayers, other.gameObject.layer))
            {
			    Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
			    if(_entity != null)
			    {
				    if(ValidateEntity(_entity))
				    {
					    AddEntity(_entity);
				    }
			    }
            }
		}

		/// <summary>
		/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
		/// </summary>
		/// <param name="other">The other Collider involved in this collision.</param>
		private void OnTriggerExit(Collider other)
		{
			Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
			if(_entity != null)
			{
				if(entitiesToTarget.Contains(_entity))
				{
					RemoveEntity(_entity);
				}
			}			
		}

		private void AddEntity(Entity entity)
		{
			entitiesToTarget.Add(entity);

            // subscribe to target death
        }

		private void RemoveEntity(Entity entity)
		{
            if(!entitiesToTarget.Contains(entity))
            {
                return;
            }

			if(currentTarget == entity)
			{
				NextTargetToTheRight();

                if(currentTarget == entity)
                {
                    LockOff();
                }
            }

			entitiesToTarget.Remove(entity);

            //unsubscribe  to target death
		}

		private bool ValidateEntity(Entity entity)
		{
			if(targetableIdentities.Contains(entity.IdentityType) && !entitiesToTarget.Contains(entity))
			{
				return true;
			}
			return false;
		}

        /// <summary>
        /// Gets the next target to the right of the references transform
        /// </summary>
		public void NextTargetToTheRight()
		{
            if (lockedOn && currentTarget != null)
            {
                SortEntitiesFromLeftToRight();

                uint _currentTargetIndex = (uint)entitiesToTarget.IndexOf(currentTarget);
                uint _nextTargetIndex = _currentTargetIndex + 1;

                if (_nextTargetIndex > entitiesToTarget.Count - 1)
                {
                    _nextTargetIndex = 0;
                }

                SetCurrent(entitiesToTarget[(int)_nextTargetIndex]);
            }
            else
            {
                LockOn();
            }
		}

        /// <summary>
        /// gets the next target to the left of the references transform
        /// </summary>
		public void NextTargetToTheLeft()
		{
            if (lockedOn && currentTarget != null)
            {
                SortEntitiesFromLeftToRight();

                int _currentTargetIndex = entitiesToTarget.IndexOf(currentTarget);
                int _nextTargetIndex = _currentTargetIndex - 1;

                if (_nextTargetIndex < 0)
                {
                    _nextTargetIndex = (entitiesToTarget.Count - 1);
                }

                SetCurrent(entitiesToTarget[_nextTargetIndex]);

                if(lockedOnEvent != null)
                {
                    lockedOnEvent.Invoke(entitiesToTarget[_nextTargetIndex]);
                }
            }
            else
            {
                LockOn();
            }
        }

        public void SetCurrent(Entity entity)
        {
            previousTarget = currentTarget;
            currentTarget = entity;

            // highlight shit
            if(previousTarget)
            {
                Outline _previousTargetOutline = previousTarget.GetComponentInChildren<Outline>();
                if(_previousTargetOutline)
                {
                    _previousTargetOutline.enabled = false;
                }
            }

            Outline _currentTargetOutline = currentTarget.GetComponentInChildren<Outline>();

            if(_currentTargetOutline)
            {
                _currentTargetOutline.enabled = true;
            }
        }

		public void LockOn()
		{
            if (entitiesToTarget.Count <= 0) return;

            //change the bool
            lockedOn = true;

            //find the first target
            Entity _firstTarget = FindEntityMostInFront();
            if(_firstTarget != null)
            {
                SetCurrent(_firstTarget);

                if (lockedOnEvent != null)
                {
                    lockedOnEvent.Invoke(_firstTarget);
                }

                if(lockedOnStaticEvent != null)
                {
                    lockedOnStaticEvent.Invoke(_firstTarget);
                }
            }
        }

        private Entity FindClosestEntity()
        {
            if(entitiesToTarget.Count > 0)
            {
                Entity _closestEntity = entitiesToTarget[0];

                if(entitiesToTarget.Count > 1)
                {
                    foreach (Entity _entity in entitiesToTarget)
                    {
                        if(_entity != _closestEntity)
                        {
                            float _distance_1 = Vector3Utility.DistanceSquared(_closestEntity.transform.position, referenceTransform.position);
                            float _distance_2 = Vector3Utility.DistanceSquared(_entity.transform.position, referenceTransform.position);

                            if(_distance_2 < _distance_1)
                            {
                                _closestEntity = _entity;
                            }
                        }
                    }
                }

                return _closestEntity;
            }

            return null;
        }

        /// <summary>
        /// Gets all visible entities and checks the one that has the smallest angle to the targeter
        /// </summary>
        /// <returns></returns>
        private Entity FindEntityMostInFront()
        {
            if(entitiesToTarget.Count > 0)
            {
                Entity _bestTargetEntity = entitiesToTarget[0];

                if(entitiesToTarget.Count > 1)
                {
                    //now find the smallest angled one
                    foreach(Entity _entity in entitiesToTarget)
                    {
                        Vector3 _closestDirection = _bestTargetEntity.transform.position - referenceTransform.position;
                        _closestDirection.y = referenceTransform.position.y;
                        float _angle_1 = Vector3.Angle(referenceTransform.forward, _closestDirection);

                        Vector3 _nextDirection = _entity.transform.position - referenceTransform.position;
                        _nextDirection.y = referenceTransform.position.y;
                        float _angle_2 = Vector3.Angle(referenceTransform.forward, _nextDirection);

                        if(_angle_2 < _angle_1)
                        {
                            _bestTargetEntity = _entity;
                        }
                    }
                }

                return _bestTargetEntity;
            }

            return null;
        }

        private List<Entity> GetAllVisibleEntities()
        {
            List<Entity> _visibleEntities = new List<Entity>();

            // can we see it???
            Vector3 _origin = referenceTransform.position;
            _origin.y += eyeHeightOffset;

            // is the entity visible?
            foreach (Entity _entity in entitiesToTarget)
            {
                Vector3 _direction = _entity.transform.position - referenceTransform.position;
                float _distance = Vector3.Distance(_origin, _entity.transform.position);

                RaycastHit _raycastHit;

                if (Physics.Raycast(_origin, _direction, out _raycastHit, _distance))
                {
                    Entity _hitEntity = _raycastHit.transform.root.GetComponentInChildren<Entity>();
                    if (_hitEntity)
                    {
                        if (_entity == _hitEntity)
                        {
                            _visibleEntities.Add(_entity);
                        }
                    }
                }
            }

            return _visibleEntities;
        }

		private void SortEntitiesFromLeftToRight()
		{
			Dictionary<Entity, float> entityToAngleMapper = new Dictionary<Entity, float>();

			foreach(var _entity in entitiesToTarget)
			{
				Vector3 _toVector = _entity.transform.position - referenceTransform.position;
				float _angle = Vector3.SignedAngle(referenceTransform.forward, _toVector, Vector3.up);

				entityToAngleMapper.Add(_entity, _angle);
			}			

			Dictionary<Entity, float> _sortedMapper = entityToAngleMapper.OrderBy(_entity => _entity.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            entitiesToTarget = _sortedMapper.Keys.ToList();
		}

		public void LockOff()
		{
            if(lockedOn && currentTarget)
            {
                if(lockedOffEvent != null)
                {
                    lockedOffEvent.Invoke(currentTarget);
                }

                if(lockedOffStaticEvent != null)
                {
                    lockedOffStaticEvent.Invoke(currentTarget);
                }

                lockedOn = false;
                currentTarget = null;
                previousTarget = null;

                RemoveAllHighlights();

                int _count = entitiesToTarget.Count;
                for(int _index = 0; _index < _count; _index++)
                {
                    if(entitiesToTarget.Count > _index)
                    {
                        if(entitiesToTarget[_index])
                        {
                            RemoveEntity(entitiesToTarget[_index]);
                        }
                    }
                }
            }

		}

        private void RemoveAllHighlights()
        {
            foreach(Entity _entity in entitiesToTarget)
            {
                Outline _outline = _entity.GetComponentInChildren<Outline>();
                if(_outline)
                {
                    _outline.enabled = false;
                }
            }
        }

        private void RespondToEntityDeath(Entity entity)
        {
            RemoveEntity(entity);
        }
	}
}
