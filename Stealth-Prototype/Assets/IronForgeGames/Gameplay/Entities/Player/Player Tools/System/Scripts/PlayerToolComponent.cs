/*
Author: Aaron Hines
Description: This component is responsiple for spawning and despawning tools as well as keeping track of the tools 
*/
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

/// <summary>
/// 
/// </summary>
/// <typeparam name="WeaponType">The current weapon</typeparam>
/// <typeparam name="WeaponType">the previous weapon</typeparam>
[System.Serializable]
public class ToolWasEquipped : UnityEngine.Events.UnityEvent<WeaponType, WeaponType>
{
}

public class PlayerToolComponent : SerializedMonoBehaviour
{
    /// <summary>
    /// this is likely to be the empty hands tool
    /// </summary>
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType defaultTool;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType currentTool;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public WeaponType currentToolType;

    private GameObject currentToolObject;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType previousTool;

    private GameObject previousToolObject;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public List<IdentityType> tools = new List<IdentityType>();

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public int maxToolsInBelt;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private Dictionary<Handedness, Transform> IKMap = new Dictionary<Handedness, Transform>();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    private ToolWasEquipped toolWasEquipped = new ToolWasEquipped();

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if(defaultTool != null)
        {
            SetCurrent(defaultTool);
        }
        else
        {
            Debug.LogError("The default tool has not been set up. The Tool Component will not function properly");
            if(tools.Count > 0)
            {
                SetCurrent(tools[0]);
            }
            else
            {
                Debug.LogError("There are no tools in the player tool component");
            }
        }
    }

    public void SetCurrent(IdentityType tool)
    {
        if(currentTool != null)
        {
            previousTool = currentTool;
        }

        if(currentToolObject != null)
        {
            previousToolObject = currentToolObject;
        }

        currentTool = tool;

        currentToolObject = SpawnManager.Instance.Spawn(currentTool);

        if(previousToolObject != null)
        {
            SpawnManager.Instance.Despawn(previousToolObject.GetComponentInChildren<Entity>());
        }

        PlayerTool _playerTool = currentToolObject.GetComponentInChildren<PlayerTool>();

        if(_playerTool != null)
        {
            if (_playerTool.weaponType != null)
            {
                currentToolType = _playerTool.weaponType;

                _playerTool.transform.SetParent(GetIKPoint(_playerTool.weaponType.handedness));
                _playerTool.transform.localPosition = Vector3.zero;
                _playerTool.transform.localRotation = Quaternion.identity;

                if (toolWasEquipped != null)
                {
                    toolWasEquipped.Invoke(currentToolType, null);
                }
            }
        }
    }

    public void SetCurrent(int index)
    {
        if(index < tools.Count)
        {
            SetCurrent(tools[index]);
        }
    }

    public void GoToPrevious()
    {
        if(previousTool != null)
        {
            SetCurrent(previousTool);
            return;
        }
    }

    private Transform GetIKPoint(Handedness handedness)
    {
        Transform _ikPoint;

        if(IKMap.TryGetValue(handedness, out _ikPoint))
        {
            return _ikPoint;
        }

        return null;
    }

    public void AddToolToBelt(IdentityType tool)
    {
        /// are we at the max amount of tools the player can hold
        /// find out which one to remove if any need to
        /// 

        if(!tools.Contains(tool))
        {
            tools.Add(tool);
        }
    }

    /// <summary>
    /// switches the player to 
    /// </summary>
    public void SwitchToEmptyHands()
    {
        if (defaultTool != null)
        {
            SetCurrent(defaultTool);
        }
    }
}
