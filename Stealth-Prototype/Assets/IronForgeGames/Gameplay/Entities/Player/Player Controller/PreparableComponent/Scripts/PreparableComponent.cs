using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    public class PreparableComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Events)]
        public StartedPreparingEvent startedPreparingEvent = new StartedPreparingEvent();

        [TabGroup(Tabs.Events)]
        public StoppedPreparingEvent stoppedPreparingEvent = new StoppedPreparingEvent();

        void OnEnable ()
        {
            InputDriver.preparedInputPressedEvent.AddListener(StartPreparing);
            InputDriver.preparedInputReleasedEvent.AddListener(StopPreparing);
	    }
	
	    void OnDisable ()
        {
            InputDriver.preparedInputPressedEvent.RemoveListener(StartPreparing);
            InputDriver.preparedInputReleasedEvent.RemoveListener(StopPreparing);
	    }

        public void StartPreparing()
        {
            if(startedPreparingEvent != null)
            {
                startedPreparingEvent.Invoke();
            }
        }

        public void StopPreparing()
        {
            if(stoppedPreparingEvent != null)
            {
                stoppedPreparingEvent.Invoke();
            }
        }
    }
}
