using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Tool")]

    public class UseTool : Action
    {

        public SharedGameObject agent;

        public int tool;

        private UseableComponent toolsUseable;

        private ToolsComponent agentsToolComponent;

        public override void OnStart()
        {
            if (agentsToolComponent == null)
            {
                agentsToolComponent = agent.Value.GetComponentInChildren<ToolsComponent>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (agent == null || agentsToolComponent == null)
            {
                return TaskStatus.Failure;
            }

            if (toolsUseable == null)
            {
                agentsToolComponent.UseTool(tool, true, out toolsUseable);
            }
            Debug.Log("using tool!");

            if(toolsUseable.inUse)
            {
                Debug.Log("running!");
                return TaskStatus.Running;
            }

            toolsUseable = null;
            return TaskStatus.Success;
        }
        
        public override void OnConditionalAbort()
        {
            agentsToolComponent.CancelTool();
        }

    }
}