using UnityEngine;

public class RaycastLookAtInteractionFilter : IInteractionFilter
{
    public InteractableComponent interactable { get; set; }
    public Interaction interaction { get; set; }

    public LayerMask layerMask;
    public float distance;
    public Vector3 originOffset;

    public bool Filter()
    {
        return IsLookingAtObject();
    }

    private bool IsLookingAtObject()
    {
        Vector3 _origin = interactable.objectActingUpon.transform.position + originOffset;
        Vector3 _direction = interactable.objectActingUpon.transform.forward;

        Ray _ray = new Ray(_origin, _direction);

        Debug.DrawRay(_origin, _direction * distance, Color.red);

        return Physics.Raycast(_ray, distance, layerMask);
    }
}
