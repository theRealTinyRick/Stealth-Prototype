using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

using AH.Max;
using AH.Max.System;

public enum Allegiance
{
    ALLY,
    PASSIVE,
    AGGRO
}

public class Enemy : Entity
{
    public BehaviorTree entityBehaviourTree;

    public IdentityType baseTarget;

    public Allegiance currentAllegiance;

    protected override void Enable()
    {
        if (entityBehaviourTree == null)
        {
            entityBehaviourTree = GetComponent<BehaviorTree>();
        }
        if (entityBehaviourTree != null)
        {
            entityBehaviourTree.GetVariable("Agent").SetValue(this.gameObject);
            //I leave shared target open ended in case in the future we would like the AI to target more than one IdentityType
            Entity _entity = EntityManager.GetEntity(baseTarget);
            if (_entity != null)
            {
                entityBehaviourTree.GetVariable("SharedTarget").SetValue(_entity.gameObject);
            }
        }
        else
        {
            Debug.LogError("No behaviour tree assigned to agent: " + this.gameObject);
        }
    }

#if UNITY_EDITOR
    private void Start()
    {
        if(entityBehaviourTree.GetVariable("SharedTarget").GetValue() == null)
        {
            entityBehaviourTree.GetVariable("SharedTarget").SetValue(EntityManager.GetEntity(baseTarget).gameObject);
        }
    }
#endif
}
