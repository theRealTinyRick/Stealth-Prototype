using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace AH.Max.Gameplay.Stealth
{
    public enum StealthObsacleType
    {
        TallGrass,
        ShortWall,
        Pillar
    }

    public class StealthObstacle : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Transform[] corners;
        
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float accuracy;

        [TabGroup(Tabs.Debug)]
        [SerializeField]
                        // point    normal
        private Dictionary<Vector3, Vector3> path = new Dictionary<Vector3, Vector3>();


        // ######################################## KEEP
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Transform edgeA;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Transform edgeB;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public StealthObsacleType stealthObsacleType;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private int numberOfProbesBetweenEdges;
        
        [SerializeField]
        public List <Vector3> hidingPlaces = new List<Vector3>();

        [Button]
        public void CalculatePath()
        {
            //if(corners.Length < 2) return;
            path.Clear();

            foreach(Transform _corner in corners)
            {
                int _nextIndex = Array.IndexOf(corners, _corner) + 1;
                if(_nextIndex >= corners.Length) _nextIndex = 0;

                Vector3 _direction = (corners[_nextIndex].position - _corner.position).normalized;
                float _distance = Vector3.Distance(_corner.position, corners[_nextIndex].position);

                float _range = _distance * accuracy;        
                if(_range < 0) _range = 1;

                for(int _i = 0; _i < _range; _i++)
                {
                    path.Add(_corner.position + (_direction * (float)(_i / _range) * 100), new Vector3());
                }

                Debug.DrawLine(_corner.position, corners[_nextIndex].position, Color.green, 10);
            }
        }

        // [Button]
        public void CalculateHidingPlaces()
        {
            hidingPlaces.Clear();

            if(stealthObsacleType == StealthObsacleType.ShortWall)
            {
                Transform _edgeOne = edgeA;
                Transform _edgeTwo = edgeB;

                for(int _count = 0; _count < numberOfProbesBetweenEdges; _count ++)
                {
                    float _multiplyer = _count / (float)numberOfProbesBetweenEdges;

                    Vector3 _vector = _edgeOne.position + ( (_edgeTwo.position - _edgeOne.position) * (_multiplyer));
                    hidingPlaces.Add(_vector);
                }
            }

            if(stealthObsacleType == StealthObsacleType.Pillar)
            {
                // get a spot on all 4 sides
            }

            if(stealthObsacleType == StealthObsacleType.TallGrass)
            {
                // make the player walk towards it to hide
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            foreach(Vector3 _vector in path.Keys)
            {
                Gizmos.DrawSphere(_vector, 0.1f);
            }
         
            foreach(Vector3 _vector in hidingPlaces)
            {
                Gizmos.DrawSphere(_vector, 0.1f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(closest, Vector3.one * 0.5f);
        }

        public Vector3 GetClosestHidingPlace(Vector3 playerPostition)
        {
            if(hidingPlaces.Count > 0)
            {
                Vector3 _result = hidingPlaces[0];
                float _closest = Vector3.Distance(playerPostition, hidingPlaces[0]); 

                foreach(Vector3 _vector in hidingPlaces)
                {
                    float _distance = Vector3.Distance(playerPostition, _vector);
                    if(_distance < _closest)
                    {
                        _closest = _distance;
                        _result = _vector;
                    }
                }
                closest = _result;
                return _result;
            }
 
            return new Vector3();
        }

        Vector3 closest = new Vector3();
        
        public Vector3 GetClosestEdge(Vector3 playerPostition)
        {
            float _sideADistance = Vector3.Distance(playerPostition, edgeA.position);
            float _sideBDistance = Vector3.Distance(playerPostition, edgeB.position);

            return _sideADistance < _sideBDistance ? edgeA.position : edgeB.position; 
        }

        public Vector3 GetFarthestEdge(Vector3 playerPostition)
        {
            float _sideADistance = Vector3.Distance(playerPostition, edgeA.position);
            float _sideBDistance = Vector3.Distance(playerPostition, edgeB.position);

            return _sideADistance > _sideBDistance ? edgeA.position : edgeB.position; 
        }

        public Vector3 GetInTightestAngle(Transform playerTransform)
        {
            Vector3 _sideA = edgeA.position;
            Vector3 _sideB = edgeB.position;

            float _sideAAngle = Vector3.Angle(playerTransform.forward, (_sideA - playerTransform.position));
            float _sideBAngle = Vector3.Angle(playerTransform.forward, (_sideB - playerTransform.position));

            return _sideAAngle < _sideBAngle ? _sideA : _sideB;
        }

        public Vector3 GetInWidestAngle(Transform playerTransform)
        {
            Vector3 _sideA = edgeA.position;
            Vector3 _sideB = edgeB.position;

            float _sideAAngle = Vector3.Angle(playerTransform.forward, (_sideA - playerTransform.position));
            float _sideBAngle = Vector3.Angle(playerTransform.forward, (_sideB - playerTransform.position));

            return _sideAAngle > _sideBAngle ? _sideA : _sideB;
        }
    }
}
