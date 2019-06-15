using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;
using AH.Max.Gameplay.System.Components;

public class PickupComponent : MonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public Vector3 objectOffset;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public LayerMask floorLayerMask;

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public PickedUpEvent pickedUpEvent = new PickedUpEvent();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public DroppedEvent droppedEvent = new DroppedEvent();

    private PickupBlock currentObjectBeingCarried = null;
    public bool isCarrying = false;

    public bool readyToDrop = false;

    void Update()
    {
        if(isCarrying && currentObjectBeingCarried != null)
        {
            Vector3 _targetPosition = transform.position;
            _targetPosition.y += objectOffset.y;
            _targetPosition += transform.forward * objectOffset.z;
            _targetPosition += transform.right * objectOffset.x;

            currentObjectBeingCarried.transform.position = _targetPosition;
            currentObjectBeingCarried.transform.rotation = transform.rotation;

            if(Input.GetKeyUp(KeyCode.F))
            {
                readyToDrop = true;
            }

            if(Input.GetKeyDown(KeyCode.F) && readyToDrop)
            {
                Drop();
            }
        }
    }

    public void PickUp(PickupBlock block)
    {
        readyToDrop = false;
        isCarrying = true;
        currentObjectBeingCarried = block;
        pickedUpEvent.Invoke();
    }

    public void Drop()
    {
        readyToDrop = false;
        if (isCarrying && currentObjectBeingCarried != null)
        {
            Vector3 _dropPosition = transform.position + transform.forward;

            Vector3 _origin = transform.position + transform.forward + (Vector3.up * 5);
            RaycastHit _hit;
            if (Physics.Raycast(_origin, Vector3.down, out _hit, 6.0f, layerMask: floorLayerMask))
            {
                _dropPosition = _hit.point;
            }

            currentObjectBeingCarried.transform.position = _dropPosition;
            currentObjectBeingCarried.Drop();

            isCarrying = false;
            currentObjectBeingCarried = null;
            droppedEvent.Invoke();
        }
    }
}

