using UnityEngine;

using AH.Max.System;
using AH.Max;

class AngleLookAtInteractionFilter : IInteractionFilter
{
    public InteractableComponent interactable { get; set; }
    public Interaction interaction { get; set; }

    public IdentityType identityType;
    public Entity entity;

    public float maxAngle;

    public bool Filter()
    {
        return IsBeingLookedAt();
    }

    private bool IsBeingLookedAt()
    {
        if(entity == null)
        {
            entity = EntityManager.GetEntity(identityType);
        }

        if(entity != null)
        {
            Vector3 _direction = interactable.transform.position - entity.transform.position;
            float _angle = Vector3.Angle(entity.transform.forward, _direction);

            return _angle < maxAngle;
        }

        return false;
    }
}
