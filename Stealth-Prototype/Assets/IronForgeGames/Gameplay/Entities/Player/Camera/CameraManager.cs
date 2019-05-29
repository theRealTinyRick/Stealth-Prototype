using System;

using UnityEngine;

using Sirenix.OdinInspector;

using Cinemachine;

namespace AH.Max.Gameplay.Camera
{
    public enum UpdateMode : int
    {
        Normal, 
        Fixed, 
        Late
    }

    [Serializable]
    public class StateData
    {
        public Vector3 positionOffset;
        public Vector3 lookAtOffset;
        public float positionDamping;
        public float recenterDelay;
        public float recenterTime;
    }

    public class CameraManager : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public UpdateMode updateMode;
        
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private CinemachineFreeLook cm_cameraController;

        private Transform previousCameraTarget;
        private Transform currentCameraTarget;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Transform cameraFollow;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Transform cameraLookAt;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private TargetingManager targetingManager;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private PlayerLedgeClimber playerLedgeFinder;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Entity entity;

        [TabGroup("Normal")]
        [SerializeField]
        private StateData normalStateData;

        [TabGroup("Locked On")]
        [SerializeField]
        private StateData lockedOnStateData;

        [TabGroup("Climbing")]
        [SerializeField]
        private StateData climbingStateData;

        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";

        private void Start()
        {
            SetCameraTarget(AH.Max.System.EntityManager.Instance.Player.transform);

            if (entity == null)
            {
                entity = transform.root.GetComponentInChildren<Entity>();
            }

            if (targetingManager == null)
            {
                targetingManager = transform.root.GetComponentInChildren<TargetingManager>();
            }
        }

        public void LateUpdate()
        {
            Tick();
        }

        private void Tick()
        {
            if(cameraFollow != null)
            {
                if(targetingManager.LockedOn)
                {
                    if(targetingManager.CurrentTarget != null)
                    {
                        cm_cameraController.m_XAxis.m_InputAxisName = "";
                        cm_cameraController.m_YAxis.m_InputAxisName = "";

                        cm_cameraController.m_XAxis.m_InputAxisValue = 0;
                        cm_cameraController.m_YAxis.m_InputAxisValue = 0;

                        cm_cameraController.m_RecenterToTargetHeading.m_enabled = true;
                        cm_cameraController.m_YAxisRecentering.m_enabled = true;

                        Vector3 _targetDirection = targetingManager.CurrentTarget.transform.position - currentCameraTarget.position;
                        Quaternion _targetRotation = Quaternion.LookRotation(_targetDirection);

                        cm_cameraController.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

                        //_targetRotation.x = 0;
                        //_targetRotation.z = 0;

                        cameraFollow.position = currentCameraTarget.position;
                        cameraLookAt.position = currentCameraTarget.position + (Vector3.up * 2);

                        cameraFollow.rotation = _targetRotation;

                        cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = lockedOnStateData.recenterDelay;
                        cm_cameraController.m_YAxisRecentering.m_WaitTime = lockedOnStateData.recenterDelay;

                        cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = lockedOnStateData.recenterTime;
                        cm_cameraController.m_YAxisRecentering.m_RecenteringTime = lockedOnStateData.recenterTime;

                        return;
                    }
                }
                /*
                else if(playerLedgeFinder.IsClimbing)
                {
                    cm_cameraController.m_XAxis.m_InputAxisName = MouseX;
                    cm_cameraController.m_YAxis.m_InputAxisName = MouseY;

                    //cameraFollow.position = Vector3.Lerp(cameraFollow.position, transform.position + climbingStateData.positionOffset, climbingStateData.positionDamping);
                    // cameraFollow.rotation = transform.rotation;

                    // cameraLookAt.position = Vector3.Lerp(cameraLookAt.position, transform.position + climbingStateData.lookAtOffset, climbingStateData.positionDamping);

                    cm_cameraController.m_RecenterToTargetHeading.m_enabled = true;
                    cm_cameraController.m_YAxisRecentering.m_enabled = true;

                    cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = climbingStateData.recenterDelay;
                    cm_cameraController.m_YAxisRecentering.m_WaitTime = climbingStateData.recenterDelay;

                    cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = climbingStateData.recenterTime;
                    cm_cameraController.m_YAxisRecentering.m_RecenteringTime = climbingStateData.recenterTime;

                    return;
                }
                */
                // reset the data

                cameraFollow.position = currentCameraTarget.position;
                cameraLookAt.position = currentCameraTarget.position + (Vector3.up * 2);

                cm_cameraController.m_XAxis.m_InputAxisName = MouseX;
                cm_cameraController.m_YAxis.m_InputAxisName = MouseY;

                cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = 3f;
                cm_cameraController.m_YAxisRecentering.m_WaitTime = 3f;

                cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = 2f;
                cm_cameraController.m_YAxisRecentering.m_RecenteringTime = 2f;

                cm_cameraController.m_RecenterToTargetHeading.m_enabled = false;
                cm_cameraController.m_YAxisRecentering.m_enabled = false;
                cm_cameraController.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

            }

        }

        public void UpdateCameraTargetPosition()
        {
            cameraFollow.position = currentCameraTarget.position;

            if (cameraFollow != null && targetingManager.LockedOn && targetingManager.CurrentTarget != null)
            {
                cameraFollow.rotation = currentCameraTarget.rotation;
            }
        }

        /// <summary>
        /// this method is used the 
        /// </summary>
        public void SetCameraTarget(Transform newTarget)
        {
            if(currentCameraTarget != null)
            {
                previousCameraTarget = currentCameraTarget;
            }

            currentCameraTarget = newTarget;
        }

        public void RevertCameraTarget()
        {

        }

        public void SetCameraMode()
        {

        }

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.black;
            //Gizmos.DrawWireCube(cameraFollow.position, Vector3.one * 0.2f);

            //Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(cameraLookAt.position, Vector3.one * 0.3f);
        }

    }
}

