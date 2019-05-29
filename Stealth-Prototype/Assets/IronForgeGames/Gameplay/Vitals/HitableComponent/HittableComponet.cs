using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

[RequireComponent(typeof(VitalsComponent))]
public class HittableComponet : MonoBehaviour 
{
	private VitalsComponent vitalsComponent;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	[Tooltip("Will be filled out at run time")]
	public Entity entity;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	public List <DamageType> vulnerableToTypes;

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public HitEvent hitEvent = new HitEvent();

	void Start () 
	{
		vitalsComponent = transform.parent.GetComponentInChildren<VitalsComponent>();
		entity = transform.root.GetComponentInChildren<Entity>();
	}
	
	public void Hit(DamageData data, IdentityType damager)
	{
		vitalsComponent.RemoveHealth(data.amount);
		hitEvent.Invoke();
	}

    [Button]
	public void Hit(float damageAmount)
	{
		vitalsComponent.RemoveHealth(damageAmount);
		hitEvent.Invoke();
	}
}
