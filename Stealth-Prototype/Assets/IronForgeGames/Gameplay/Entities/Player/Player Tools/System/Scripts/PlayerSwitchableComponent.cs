using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

[System.Serializable]
public class SwitchToEvent : UnityEngine.Events.UnityEvent<int>
{
}

public class PlayerSwitchableComponent : SerializedMonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public PlayerToolComponent playerToolComponent;

    [Button]
    public virtual void Next()
    {
        if(playerToolComponent.currentTool == playerToolComponent.defaultTool && playerToolComponent.tools.Count > 0)
        {
            playerToolComponent.SetCurrent(0);
            return;
        }

        if (playerToolComponent.tools.Count <= 1)
        {
            return;
        }

        int _nextIndex = playerToolComponent.tools.IndexOf(playerToolComponent.currentTool);

        do
        {
            _nextIndex++;
            if (_nextIndex >= playerToolComponent.tools.Count)
            {
                _nextIndex = 0;
            }

            if (playerToolComponent.tools[_nextIndex] == playerToolComponent.currentTool)
            {
                return;
            }
        }
        while (ToolDatabase.IsLocked(playerToolComponent.tools[_nextIndex]));

        playerToolComponent.SetCurrent(_nextIndex);
    }

    [Button]
    public virtual void Previous()
    {
        if (playerToolComponent.currentTool == playerToolComponent.defaultTool && playerToolComponent.tools.Count > 0)
        {
            playerToolComponent.SetCurrent(playerToolComponent.tools.Count - 1);
            return;
        }

        if (playerToolComponent.tools.Count <= 1)
        {
            return;
        }

        int _nextIndex = playerToolComponent.tools.IndexOf(playerToolComponent.currentTool);

        do
        {
            _nextIndex--;
            if (_nextIndex < 01)
            {
                _nextIndex = playerToolComponent.tools.Count - 1;
            }

            if (playerToolComponent.tools[_nextIndex] == playerToolComponent.currentTool)
            {
                return;
            }
        }
        while (ToolDatabase.IsLocked(playerToolComponent.tools[_nextIndex]));

        playerToolComponent.SetCurrent(_nextIndex);
    }
}
