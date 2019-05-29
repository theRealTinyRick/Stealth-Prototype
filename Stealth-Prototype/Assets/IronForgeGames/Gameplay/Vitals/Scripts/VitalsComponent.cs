using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System.Stats;
using AH.Max.System;
using AH.Max;

public class VitalsComponent : SerializedMonoBehaviour
{
	[SerializeField]
	[TabGroup(Tabs.Properties)]
	[Tooltip("Will be filled out at run time")]
	public Entity entity;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	public StatType healthStatType;

    public float Health 
    {
        get 
        {
            if(entity != null && entity.HasStat(healthStatType))
            {
                return entity.GetStat(healthStatType).Amount;
            }

            return 0; 
        }
        set 
        {
            if(Health != value)
            {
                if(entity != null && entity.HasStat(healthStatType))
                {
                    entity.SetStat(healthStatType, value);
                }
            }
        }
    }

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public AddedHealthEvent addedHealthEvent = new AddedHealthEvent();

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public RemovedHealthEvent removedHealthEvent = new RemovedHealthEvent();

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public NoHealthEvent noHealthEvent = new NoHealthEvent();

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		entity = transform.root.GetComponentInChildren<Entity>();

		entity.GetStat(healthStatType).Reset();
	}

	public void RemoveHealth(float amount)
	{
        Stat _stat = entity.GetStat(healthStatType);

        _stat.Subtract(amount);
		removedHealthEvent.Invoke();

		if(_stat.Amount <= _stat.MinimumAmount)
	    {
			noHealthEvent.Invoke();
		}
	}

	public void AddHealth(float amount)
	{
        entity.GetStat(healthStatType).Add(amount);
		addedHealthEvent.Invoke();
	}

	public void RemoveAllHealth()
	{
        entity.GetStat(healthStatType).RemoveAll();
        noHealthEvent.Invoke();
	}
}
