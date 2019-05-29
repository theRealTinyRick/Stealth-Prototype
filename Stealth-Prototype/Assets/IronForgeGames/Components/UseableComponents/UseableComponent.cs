using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

[System.Serializable]
public class UseEvent : UnityEvent
{
}

[System.Serializable]
public class StopUseEvent : UnityEvent
{
}

[System.Serializable]
public class CancelUseEvent : UnityEvent
{
}

[System.Serializable]
public class HandleEvent : UnityEvent
{
}

[System.Serializable]
public class StopHandleEvent : UnityEvent
{
}

public class UseableComponent : MonoBehaviour {

    [TabGroup("Events")]
    public UseEvent useEvent;

    [TabGroup("Events")]
    public StopUseEvent stopUseEvent;

    [TabGroup("Events")]
    public CancelUseEvent cancelUseEvent;

    [TabGroup("Events")]
    public HandleEvent handleEvent;

    [TabGroup("Events")]
    public StopHandleEvent stopHandleEvent;

    [TabGroup("Properties")]
    public bool inUse;

    [TabGroup("Properties")]
    public bool useCooldown;

    [TabGroup("Properties")]
    public float coolDown;

    private void OnEnable()
    {
        if (useEvent == null)
        {
            useEvent = new UseEvent();
        }

        if(stopUseEvent == null)
        {
            stopUseEvent = new StopUseEvent();
        }

        if(cancelUseEvent == null)
        {
            cancelUseEvent = new CancelUseEvent();
        }

        if(handleEvent == null)
        {
            handleEvent = new HandleEvent();
        }

        if(stopHandleEvent == null)
        {
            stopHandleEvent = new StopHandleEvent();
        }        
    }

    public void Use()
    {
        if(inUse)
        {
            return;
        }

        inUse = true;

        if(useCooldown)
        {
            StartCoroutine(Cooldown());
        }

        useEvent.Invoke();
    }

    /// <summary>
    /// Is called when the tool has finished being used
    /// </summary>
    public void StopUse()
    {
        if (!inUse)
        {
            return;
        }

        inUse = false;

        stopUseEvent.Invoke();
    }

    public void CancelUse()
    {
        if (!inUse)
        {
            return;
        }

        inUse = false;

        StopAllCoroutines();

        cancelUseEvent.Invoke();
    }

    /// <summary>
    /// handle is to be used specifically for animation uses
    /// </summary>
    public void Handle()
    {
        if (inUse)
        {
            return;
        }

        inUse = true;

        handleEvent.Invoke();
    }

    public void StopHandle()
    {
        if (!inUse)
        {
            return;
        }

        inUse = false;

        stopHandleEvent.Invoke();
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(coolDown);

        StopUse();

    }
}
