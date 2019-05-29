using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

[System.Serializable]
public class HandleProxyEvent : UnityEvent
{

}

[System.Serializable]
public class HandleStopEvent : UnityEvent
{
}

[System.Serializable]
public class CancelProxyEvent : UnityEvent
{
}

public class UseProxyComponent : MonoBehaviour
{
    [TabGroup("Properties")]
    public Animator thisAnimator;

    [TabGroup("Events")]
    public HandleProxyEvent handleProxyEvent;

    [TabGroup("Events")]
    public HandleStopEvent handleStopEvent;

    [TabGroup("Events")]
    public CancelProxyEvent cancelProxyEvent;

    public string Using = "Using";

    public string Use = "Use";

    private void OnEnable()
    {
        if(handleProxyEvent == null)
        {
            handleProxyEvent = new HandleProxyEvent();
        }

        if(handleStopEvent == null)
        {
            handleStopEvent = new HandleStopEvent();
        }

        if(cancelProxyEvent == null)
        {
            cancelProxyEvent = new CancelProxyEvent();
        }

        if(thisAnimator == null)
        {
            thisAnimator = GetComponent<Animator>();
        }

    }
    public void UseProxy()
    {
        thisAnimator.SetTrigger(Use);
        thisAnimator.SetBool(Using, true);
    }

    public void StopProxy()
    {
        thisAnimator.SetBool(Using, false);
    }

    public void HandleProxy()
    {
        handleProxyEvent.Invoke();
    }

    public void HandleStopProxy()
    {
        handleStopEvent.Invoke();
    }

    public void CancelUseProxy()
    {
        cancelProxyEvent.Invoke();
    }


}
