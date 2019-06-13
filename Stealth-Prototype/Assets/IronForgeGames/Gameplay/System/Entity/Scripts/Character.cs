/*
Author: Aaron Hines
Edits By:
Description: An Extension to the Entity that includes input listeners and more! 
Notes: 
 */
using Sirenix.OdinInspector;
using UnityEngine;

namespace AH.Max
{
    public class Character : Entity
    {
        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public JumpButtonEvent jumpButtonPressed = new JumpButtonEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public JumpButtonEvent jumpButtonReleased = new JumpButtonEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public LightAttackButtonEvent lightAttackPressed = new LightAttackButtonEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public PreparedInputPressedEvent preparedPressed = new PreparedInputPressedEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public PreparedInputReleasedEvent preparedReleased = new PreparedInputReleasedEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public LockOnButtonPressedEvent lockedOnPressed = new LockOnButtonPressedEvent();

        [TabGroup(Tabs.InputEvents)]
        [SerializeField]
        public AnyButtonWasPressedEvent anyButtonWasPressed = new AnyButtonWasPressedEvent();

        protected override void Enable()
        {
            base.Enable();

            SetupInput();
        }

        protected override void Disable()
        {
            base.Disable();

            //RemoveInput();
        }

        private void SetupInput()
        {
            InputDriver.jumpButtonEvent.AddListener(jumpButtonPressed.Invoke);
            InputDriver.jumpButtonReleasedEvent.AddListener(jumpButtonReleased.Invoke);
            InputDriver.lightAttackButtonEvent.AddListener(lightAttackPressed.Invoke);
            InputDriver.preparedInputPressedEvent.AddListener(preparedPressed.Invoke);
            InputDriver.preparedInputReleasedEvent.AddListener(preparedReleased.Invoke);
            InputDriver.lockOnButtonEvent.AddListener(preparedReleased.Invoke);
            InputDriver.anyButtonWasPressedEvent.AddListener(anyButtonWasPressed.Invoke);
        }

        private void RemoveInput()
        {
            InputDriver.jumpButtonEvent.RemoveListener(jumpButtonPressed.Invoke);
            InputDriver.jumpButtonReleasedEvent.RemoveListener(jumpButtonReleased.Invoke);
            InputDriver.lightAttackButtonEvent.RemoveListener(lightAttackPressed.Invoke);
            InputDriver.preparedInputPressedEvent.RemoveListener(preparedPressed.Invoke);
            InputDriver.preparedInputReleasedEvent.RemoveListener(preparedReleased.Invoke);
            InputDriver.lockOnButtonEvent.RemoveListener(preparedReleased.Invoke);
            InputDriver.anyButtonWasPressedEvent.RemoveListener(anyButtonWasPressed.Invoke);
        }
    }
}

