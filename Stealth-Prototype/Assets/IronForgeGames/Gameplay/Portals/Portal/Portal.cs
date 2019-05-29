using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;

public class Portal : MonoBehaviour
{
    public List<IdentityType> entitiesThatCanTeleport = new List<IdentityType>();
    public LayerMask layerMask;

    public PortalComponent portalComponent; // owner
    public ParticleSystem portalBlue;
    public ParticleSystem portalRed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PortalSetUp(PortalComponent portalComponent)
    {
        this.portalComponent = portalComponent;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(LayerMaskUtility.IsWithinLayerMask(layerMask, other.gameObject.layer))
        {
            Entity _entity = other.GetComponentInChildren<Entity>();
            if(_entity != null)
            {
                if(entitiesThatCanTeleport.Contains(_entity.IdentityType) || (_entity.IdentityType.parent != null && entitiesThatCanTeleport.Contains(_entity.IdentityType.parent)))
                {
                    // do thing
                    if(portalComponent != null)
                    {
                        portalComponent.TryTeleport(this, _entity);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void SetBlue()
    {
        portalRed.gameObject.SetActive(false);
        portalBlue.gameObject.SetActive(true);
    }

    public void SetRed()
    {
        portalRed.gameObject.SetActive(true);
        portalBlue.gameObject.SetActive(false);
    }
}
