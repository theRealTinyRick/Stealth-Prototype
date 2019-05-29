using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Targeting")]
    public class LineOfSight : Conditional
    {

        public SharedGameObject agent;

        public SharedGameObject sharedTarget;

        public float signedAngle;

        public float distanceOfSight;

        public LayerMask blockingLayers;

        private Vector3 yOffset = new Vector3(0,1,0);

        public override TaskStatus OnUpdate()
        {
            if(agent == null || sharedTarget.Value == null)
            {
                return TaskStatus.Failure;
            }

            Vector3 _targetDirection = sharedTarget.Value.transform.position - agent.Value.transform.position;

            float _angle = Vector3.SignedAngle(agent.Value.transform.forward, _targetDirection, Vector3.up);

            if(Physics.Raycast(agent.Value.transform.position + yOffset, agent.Value.transform.forward, distanceOfSight, blockingLayers, QueryTriggerInteraction.UseGlobal))
            {
                return TaskStatus.Failure;
            }

            if(_angle >= -signedAngle && _angle <= signedAngle )
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }


    }
}