using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;

[RequireComponent(typeof(Collider))]
public class DamageWithinBoundsComponent : DamageComponent
{
    [SerializeField]
    private LayerMask damageableLayers;

    [ShowInInspector]
    private List<Entity> targets = new List<Entity>();

    public void OnTriggerStay(Collider other)
    {
        if (LayerMaskUtility.IsWithinLayerMask(damageableLayers, other.gameObject.layer))
        {
            Entity _entity = other.GetComponentInChildren<Entity>();

            if (_entity != null)
            {
                if (CanDamage(_entity.IdentityType))
                {
                    if(!targets.Contains(_entity))
                    {
                        targets.Add(_entity);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (LayerMaskUtility.IsWithinLayerMask(damageableLayers, other.gameObject.layer))
        {
            Entity _entity = other.GetComponentInChildren<Entity>();

            if (_entity != null)
            {
                if (CanDamage(_entity.IdentityType))
                {
                    if (targets.Contains(_entity))
                    {
                        targets.Remove(_entity);
                    }
                }
            }
        }
    }

    [Button]
    public void DealDamage()
    {
        foreach(Entity _entity in targets)
        {
            DealDamage(_entity);
        }
    }
}
