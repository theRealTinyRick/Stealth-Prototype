using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.Components
{
    public enum MoveType
    {
        Vertical,
        Horizontal,
        Freeform
    }

    public class MoveToComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public List<Transform> toPoints = new List<Transform>();

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public GameObject objectToMove;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public float speed;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public MoveType moveType;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public bool hasReachedDestination = true;

        private Transform currentPoint;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public MoveStartedEvent moveStartedEvent = new MoveStartedEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public DestinationReachedEvent destinationReachedEvent = new DestinationReachedEvent();

        public bool playOnStart;

        private Coroutine coroutine;

        private void Start()
        {
            if(playOnStart)
            {
                MoveTo(0);
            }
        }

        public void MoveTo(int index)
        {
            if (objectToMove == null)
            {
                return;
            }

            if(index > toPoints.Count - 1)
            {
                return;
            }

            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(MoveCoroutine(toPoints[index]));
        }

        public void MoveNext()
        {
            if (toPoints.Count <= 1)
            {
                // assume we are either at the closest point or have no new point to go to, so dont use logic with out cause
                return;
            }

            if (hasReachedDestination)
            {
                int _nextIndex = NextInt();

                MoveTo(_nextIndex);
            }
        }

        public int NextInt()
        {
            if(currentPoint != null)
            {
                int _result = toPoints.IndexOf(currentPoint) + 1;

                if(_result < toPoints.Count)
                {
                    return _result;
                }
            }

            return 0;
        }

        public int IndexClosestToThatIsNotCurrent()
        {
            if(toPoints.Count <= 1)
            {
                Debug.LogWarning("There are no points in the To Points list");
                return 0;
            }

            int _indexOfClosestPoint = 0;
            float _clostestDistance = Vector3.Distance(objectToMove.transform.position, toPoints[0].position);

            if(currentPoint != null)
            {
                if(toPoints[_indexOfClosestPoint] == currentPoint)
                {
                    _indexOfClosestPoint = 1;
                    _clostestDistance = Vector3.Distance(objectToMove.transform.position, toPoints[1].position);
                }
            }

            foreach(Transform _point in toPoints)
            {
                //make sure we aren't checking the first one again
                if(_point != currentPoint)
                {
                    float _distance = Vector3.Distance(objectToMove.transform.position, _point.position);

                    if (_distance < _clostestDistance)
                    {
                        _indexOfClosestPoint = toPoints.IndexOf(_point);
                        _clostestDistance = _distance;
                    }
                }
            }

            return 0;
        }

        private IEnumerator MoveCoroutine(Transform toPosition)
        {
            if(moveStartedEvent != null)
            {
                moveStartedEvent.Invoke();
            }

            hasReachedDestination = false;

            Vector3 _toPosition = toPosition.position;
            Vector3 _fromPosition = objectToMove.transform.position;

            if(moveType == MoveType.Freeform)
            {

            }
            else if(moveType == MoveType.Horizontal)
            {
                _toPosition.y = objectToMove.transform.position.y;
            }
            else
            {
                _toPosition = toPosition.position;
                _toPosition.x = objectToMove.transform.position.x;
                _toPosition.z = objectToMove.transform.position.z;
            }

            while (Vector3.Distance(objectToMove.transform.position, _toPosition) > 0.01)
            {
                Vector3 _direction = _toPosition - _fromPosition;

                objectToMove.transform.position += _direction * (speed * Time.deltaTime);

                yield return new WaitForFixedUpdate();
            }

            if(destinationReachedEvent != null)
            {
                destinationReachedEvent.Invoke();
            }

            currentPoint = toPosition;
            hasReachedDestination = true;

            yield break;
        }
    }
}
