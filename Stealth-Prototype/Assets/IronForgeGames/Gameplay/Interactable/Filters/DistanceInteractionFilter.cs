using UnityEngine;

public class DistanceInteractionFilter : IInteractionFilter
{
    public InteractableComponent interactable { get; set; }
    public Interaction interaction { get; set; }

    public float maxDistance;

    public bool Filter()
    {
        return IsWithInDistance();
    }

    private bool IsWithInDistance()
    {
        //return Vector3.Distance()
        return Vector3Utility.DistanceSquared(interactable.transform.position, interactable.objectActingUpon.transform.position) < maxDistance * maxDistance;
    }
}
