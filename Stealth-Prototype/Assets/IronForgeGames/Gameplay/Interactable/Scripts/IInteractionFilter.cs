using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public interface IInteractionFilter
{
    InteractableComponent interactable 
    {
        get;
        set;
    }

    Interaction interaction 
    {
        get;
        set;
    }

    bool Filter();
}
