/*
Author: Aaron Hines
Description: The base class for all damage dealers, this is intended to be extended to handle different siturations
*/
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;
using AH.Max.System.DamageMap;

public abstract class DamageComponent : MonoBehaviour
{
    public Entity myEntity;

    public void Start()
    {
        if(myEntity == null)
        {
            myEntity = GetComponent<Entity>();
        }
    }

    protected virtual void DealDamage(Entity target)
    {
        if(CanDamage(target.IdentityType))
        {
            HittableComponet hittableComponent = target.GetComponentInChildren<HittableComponet>();
            hittableComponent.Hit(GetDamage());
        }
    }

    protected virtual bool CanDamage(IdentityType potentialTarget)
    {
        return DamageMapDataBase.GetWhatICanDamage(myEntity.IdentityType).Contains(potentialTarget);
    }

    protected virtual float GetDamage()
    {
        // TODO: Add logic for stat modifications

        return DamageMapDataBase.GetDamageAmount(myEntity);
    }
}
