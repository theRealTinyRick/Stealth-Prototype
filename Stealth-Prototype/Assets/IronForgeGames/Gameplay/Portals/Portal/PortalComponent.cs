using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AH.Max;

using Sirenix.OdinInspector;

public enum PortalStyle
{
    OneWay,
    TwoWay
}

public class PortalComponent : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<Entity, Vector3> entitiesBeingTransported = new Dictionary<Entity, Vector3>();

    [SerializeField]
    private PortalStyle portalStyle;

    public Portal portalOne;
    public Portal portalTwo;
    public bool isWaitingToFinish = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SpawnPortal(Vector3 position, Quaternion rotation)
    {
        Portal portal = null;

        // portal gets spawned
        portal.SetBlue();
        portal.SetRed();
        portal.PortalSetUp(this);
    }

    public void TryTeleport(Portal portal, Entity teleportee)
    {
        if (entitiesBeingTransported.ContainsKey(teleportee)) return;

        switch(portalStyle)
        {
            case PortalStyle.OneWay:
                if(portal == portalOne)
                {
                    StartTeleport(portalOne, portalTwo, teleportee);
                }
                break;
            case PortalStyle.TwoWay:
                Portal _portalTwo = null;
                if(portal == portalOne)
                {
                    _portalTwo = portalTwo;
                }
                else
                {
                    _portalTwo = portalOne;
                }

                StartTeleport(portal, _portalTwo, teleportee);
                break;
        }

    }

    private void StartTeleport(Portal startPortal, Portal endPortal, Entity teleportee)
    {
        Rigidbody _teleporteedRigidBody = teleportee.GetComponent<Rigidbody>();

        entitiesBeingTransported.Add(teleportee, _teleporteedRigidBody != null ? _teleporteedRigidBody.velocity : Vector3.zero);
        teleportee.gameObject.SetActive(false);

        teleportee.transform.position = endPortal.transform.position;

        if(endPortal.gameObject.activeInHierarchy)
        {
            FinishTeleport();
        }
        else
        {
            isWaitingToFinish = true;
        }
    }

    private void FinishTeleport()
    {
        // spawn it 
        // if it has a rigidbody - the opposit force a force to it
        foreach(Entity _entity in entitiesBeingTransported.Keys)
        {
            _entity.gameObject.SetActive(true);
            Rigidbody _entityRigidBody = _entity.GetComponent<Rigidbody>();
            if(_entityRigidBody != null)
            {
                _entityRigidBody.velocity = entitiesBeingTransported[_entity];
                _entityRigidBody.AddForce(portalTwo.transform.forward  * entitiesBeingTransported[_entity].magnitude, ForceMode.Impulse);
            }

            entitiesBeingTransported.Remove(_entity);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void WaitForSpawn()
    {
    }
}
