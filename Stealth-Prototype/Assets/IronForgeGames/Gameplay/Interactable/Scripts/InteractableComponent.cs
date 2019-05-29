using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;

public class InteractableComponent : SerializedMonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List <IdentityType> interactableIdentities;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private LayerMask layers;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List <Interaction> interactions;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List<Interaction> validInteractions = new List<Interaction>();

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private Outline outlineComponent;

    public GameObject objectActingUpon;

    private bool hasBeenInteractedWith = false;
    public bool HasBeenInteractedWith
    {
        get
        {
            return hasBeenInteractedWith;
        }
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        foreach(Interaction _interaction in interactions)
        {
            _interaction.Initialize();

            foreach(IInteractionFilter _filter in _interaction.filters)
            {
                _filter.interactable = this;
            }
        }

        outlineComponent = transform.root.GetComponentInChildren<Outline>();

        if(outlineComponent != null)
        {
            outlineComponent.enabled = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(ValidateCollider(other))
        {
            if(outlineComponent != null)
            {
                outlineComponent.enabled = true;
            }

            objectActingUpon = other.gameObject;

            Evaluate();
            EvaluateValidInteractionInput();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (ValidateCollider(other))
        {
            if (outlineComponent != null)
            {
                outlineComponent.enabled = false;
            }

            Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Evaluate()
    {
        foreach(Interaction _interaction in interactions)
        {
            if(_interaction.EvaluateFilters())
            {
                if(!validInteractions.Contains(_interaction))
                {
                    validInteractions.Add(_interaction);
                }
            }
            else
            {
                if (validInteractions.Contains(_interaction))
                {
                    validInteractions.Remove(_interaction);        
                }
            }
        }
    }

    public void EvaluateValidInteractionInput()
    {
        if (validInteractions.Count > 0)
        {
            foreach(Interaction _interaction in validInteractions)
            {
                if(_interaction.EvaluateInput())
                {
                    _interaction.ExecuteInteraction();
                    hasBeenInteractedWith = true;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Clears out all the interactions
    /// </summary>
    public void Clear()
    {
        objectActingUpon = null;

        validInteractions.Clear();
    }

    private bool ValidateCollider(Collider other)
    {
        if (LayerMaskUtility.IsWithinLayerMask(layers, other.gameObject.layer))
        {
            Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
            if (_entity != null)
            {
                if (interactableIdentities.Contains(_entity.IdentityType))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
