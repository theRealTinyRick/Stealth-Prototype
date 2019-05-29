using System;
using System.Collections.Generic;

using UnityEngine;

using AH.Max.System;
using AH.Max;

using AH.Max.Gameplay.System.Components;

public class EvaluateStateComponentFilter : IInteractionFilter
{
    public InteractableComponent interactable { get; set; }
    public Interaction interaction { get; set; }

    public State state;

    public bool invertState;

    public StateComponent stateComponent;

    public bool Filter()
    {
        return CheckState();
    }

    private bool CheckState()
    {
       if(stateComponent != null)
       {
            if(stateComponent.HasState(state))
            {
                return invertState ? !stateComponent.GetState(state) : stateComponent.GetState(state);
            }
       }

       Debug.LogWarning("No State Component Assigned", interactable.gameObject);
       return false;
    }
}
