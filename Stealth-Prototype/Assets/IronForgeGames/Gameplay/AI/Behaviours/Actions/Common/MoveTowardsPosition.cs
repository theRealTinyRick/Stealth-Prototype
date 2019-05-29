using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Locomotion")]
    public class MoveTowardsPosition : Action
    {
        public SharedGameObject agent;

        public SharedVector3 sharedTargetPosition;

        private NavMeshAgent agentNavmeshAgent;

        public override TaskStatus OnUpdate()
        {
            if (agentNavmeshAgent == null)
            {
                agentNavmeshAgent = agent.Value.GetComponent<NavMeshAgent>();
            }

            if (agent == null || sharedTargetPosition == null || agentNavmeshAgent == null)
            {
                return TaskStatus.Failure;
            }

            agentNavmeshAgent.SetDestination(sharedTargetPosition.Value);

            agent.Value.GetComponent<Animator>().SetBool("IsMoving", true);

            return TaskStatus.Success;
        }

        public override void OnConditionalAbort()
        {
            agent.Value.GetComponent<Animator>().SetBool("IsMoving", false);
        }

    }
} 