using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;
using AH.Max.Gameplay.System.Components;

[System.Serializable]
public class PickedUpEvent : UnityEngine.Events.UnityEvent { }

[System.Serializable]
public class DroppedEvent : UnityEngine.Events.UnityEvent { }

public class PickupBlock : MonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType playerIdentity;

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public PickedUpEvent pickedUpEvent = new PickedUpEvent();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public DroppedEvent droppedEvent = new DroppedEvent();

    private Entity playerEntity;
    private PickupComponent playerPickupComponent;

    void OnEnable()
    {
    }

    public void PickUp()
    {
        if(playerIdentity != null)
        {
            if(playerEntity == null)
            {
                playerEntity = EntityManager.GetEntity(playerIdentity);
            }

            if(playerPickupComponent == null)
            {
                playerPickupComponent = playerEntity.GetComponentInChildren<PickupComponent>();
            }

            playerPickupComponent.PickUp(this);

            pickedUpEvent.Invoke();
        }
    }

    public void Drop()
    {
        droppedEvent.Invoke();
    }
}
