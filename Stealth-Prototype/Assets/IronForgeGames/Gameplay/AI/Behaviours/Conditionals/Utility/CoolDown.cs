using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Utility")]
    public class CoolDown : Conditional
    {
        public SharedUnityEvent cooldownEvent;

        public SharedUnityEvent stopCooldownEvent;

        public SharedFloat timer;

        public bool onCooldown;

        public bool startCooldown;

        public bool blocking;

        private Coroutine cooldownCoroutine;

        private IEnumerator cooldownIEnumerator;

        public override void OnAwake()
        {
            if (cooldownEvent != null)
            {
                if(cooldownEvent.Value == null)
                {
                    cooldownEvent.Value = new UnityEngine.Events.UnityEvent();
                }
                cooldownEvent.Value.AddListener(StartCooldownTimer);
            }

            if(stopCooldownEvent != null)
            {
                if(stopCooldownEvent.Value == null)
                {
                    stopCooldownEvent.Value = new UnityEngine.Events.UnityEvent();
                }
                stopCooldownEvent.Value.AddListener(StopCooldownTimer);
            }
            cooldownIEnumerator = CooldownTimer();
        }

        public void OnDisable()
        {
            Debug.Log("i disabled");
            cooldownEvent.Value.RemoveListener(StartCooldownTimer);
            StopCoroutine(cooldownIEnumerator);
            cooldownCoroutine = null;
        }

        public override TaskStatus OnUpdate()
        {
            if (startCooldown && cooldownCoroutine == null)
            {
                cooldownCoroutine = StartCoroutine(cooldownIEnumerator);
            }

            if (onCooldown)
            {
                if(blocking)
                {
                    return TaskStatus.Running;
                }

                return TaskStatus.Failure;
            }

            return TaskStatus.Success;

        }

        public void StartCooldownTimer()
        {
            if (cooldownCoroutine == null)
            {
                cooldownCoroutine = StartCoroutine(CooldownTimer());
            }
        }

        public void StopCooldownTimer()
        {
            Debug.Log("moving into stop timer");
            if (cooldownCoroutine != null)
            {
                //for some reason behaviour designers StopCoroutine
                //does not accept regular coroutines in their parameter
                StopCoroutine(cooldownIEnumerator);
                onCooldown = false;
                cooldownCoroutine = null;
                Debug.Log("made it!");
            }
        }

        private IEnumerator CooldownTimer()
        {

            Debug.Log("entered timer");
            onCooldown = true;

            yield return new WaitForSeconds(timer.Value);

            Debug.Log("exited timer");
            onCooldown = false;
        }
    }
}