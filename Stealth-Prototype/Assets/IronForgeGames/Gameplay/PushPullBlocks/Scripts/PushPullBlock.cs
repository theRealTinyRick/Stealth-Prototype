using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;
using AH.Max.Gameplay.System.Components;

public class PushPullBlock : MonoBehaviour
{
    [SerializeField]
    private IdentityType playerIdentity;

    private Entity playerEntity;
    private PushBlockComponent playerPushBlockComponent;

    private new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void InitPush()
    {
        if(playerEntity == null)
        {
            playerEntity = EntityManager.GetEntity(playerIdentity);
        }

        if(playerPushBlockComponent == null)
        {
            playerPushBlockComponent = playerEntity.GetComponentInChildren<PushBlockComponent>();
        }
        
        playerPushBlockComponent.InitPushBlock(this);
        rigidbody.isKinematic = false;
    }

    public void StopPushPull()
    {
        rigidbody.isKinematic = true;
    }
}
