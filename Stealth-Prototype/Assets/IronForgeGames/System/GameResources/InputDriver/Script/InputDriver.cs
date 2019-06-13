using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class InputDriver : Singleton_MonoBehavior<InputDriver>
{
    //events
    public static JumpButtonEvent jumpButtonEvent = new JumpButtonEvent();
    public static JumpButtonEvent jumpButtonHeldEvent = new JumpButtonEvent();
    public static JumpButtonEvent jumpButtonReleasedEvent = new JumpButtonEvent();

    public static LightAttackButtonEvent lightAttackButtonEvent = new LightAttackButtonEvent();
    public static PreparedInputPressedEvent preparedInputPressedEvent = new PreparedInputPressedEvent();
    public static PreparedInputReleasedEvent preparedInputReleasedEvent = new PreparedInputReleasedEvent(); 
    public static EvadeButtonEvent evadeButtonEvent = new EvadeButtonEvent();
    public static LockOnButtonPressedEvent lockOnButtonEvent = new LockOnButtonPressedEvent();

    public static bool jumpButtonIsBeingHeld;

    //menu specific
    public static SelectButtonEvent selectButtonEvent = new SelectButtonEvent();
    public static BackButtonEvent backButtonEvent = new BackButtonEvent();

    // any key
    public static AnyButtonWasPressedEvent anyButtonWasPressedEvent = new AnyButtonWasPressedEvent();

    //vectors
    public static Vector3 LocomotionDirection = new Vector3();
    public static Vector3 LocomotionOrientationDirection = new Vector3();
    public static Vector3 RightInputDirection = new Vector3();

    public static Vector3 cameraLookDirection = new Vector3();

	private void Update () 
	{
        PlayerInputListener();
        MenuInputListener();
        AnyButtonPressed();
    }

    private void PlayerInputListener() 
    {
        //receive locomotion input
        float _locomotionX = UnityEngine.Input.GetAxis(InputDataBase.Horizontal);
        float _locomotionY= UnityEngine.Input.GetAxis(InputDataBase.Vertical);
        ///float _locomotionX = Input.GetAxis(InputDataBase.LeftStickHorizontal);
        ///float _locomotionY = Input.GetAxis(InputDataBase.LeftStickVertical);

        // if no input could not be found check for input from the keyboard
        if(_locomotionX == 0 && _locomotionY == 0)
        {
        }

        //assign the value above
        LocomotionDirection.x = _locomotionX;
        LocomotionDirection.z = _locomotionY;

        float _rightInputDirectionX = UnityEngine.Input.GetAxis(InputDataBase.MouseX);
        float _rightInputDirectionY = UnityEngine.Input.GetAxis(InputDataBase.MouseY);

        //float _rightInputDirectionX = Input.GetAxis(InputDataBase.RightStickHorizontal);
        //float _rightInputDirectionY = Input.GetAxis(InputDataBase.RightStickVertical);

        if(_rightInputDirectionX == 0 && _rightInputDirectionY == 0)
        {
        }

        RightInputDirection.x = _rightInputDirectionX;
        RightInputDirection.y = _rightInputDirectionY;

        //jump button from game pad then keyboard
        if(/*Input.GetButtonDown(InputDataBase.AButton) ||*/ UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            if(jumpButtonEvent != null)
            {
                jumpButtonEvent.Invoke();
            }
        }

        if(/*Input.GetButton(InputDataBase.AButton) || */ UnityEngine.Input.GetKey(KeyCode.Space))
        {
            jumpButtonIsBeingHeld = true;
            if(jumpButtonHeldEvent != null)
            {
                jumpButtonHeldEvent.Invoke();
            }
        }
        else
        {
            jumpButtonIsBeingHeld = false;
        }

        if (/*Input.GetButtonUp(InputDataBase.AButton) ||*/ UnityEngine.Input.GetKeyUp(KeyCode.Space))
        {
            if (jumpButtonReleasedEvent != null)
            {
                jumpButtonReleasedEvent.Invoke();
            }
        }

        if (/*Input.GetButtonDown(InputDataBase.XButton) ||*/ UnityEngine.Input.GetMouseButtonDown(0))
        {
            if(lightAttackButtonEvent != null)
            {
                lightAttackButtonEvent.Invoke();
            }
        }

        if (/*Input.GetButtonDown(InputDataBase.XButton) ||*/ UnityEngine.Input.GetMouseButtonDown(1))
        {
            if(preparedInputPressedEvent != null)
            {
                preparedInputPressedEvent.Invoke();
            }
        }

        if (/*Input.GetButtonUp(InputDataBase.XButton) ||*/ UnityEngine.Input.GetMouseButtonUp(1))
        {
            if(preparedInputReleasedEvent != null)
            {
                preparedInputReleasedEvent.Invoke();
            }
        }

        if (/*Input.GetButtonDown(InputDataBase.BButton) ||*/ UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(evadeButtonEvent != null)
            {
                evadeButtonEvent.Invoke();
            }
        }

        if(UnityEngine.Input.GetMouseButtonDown(2))
        {
            if(lockOnButtonEvent != null)
            {
                lockOnButtonEvent.Invoke();
            }
        }

        //Held input ->
    }

    private void MenuInputListener()
    {
        // Back
        if(/*Input.GetButtonDown(InputDataBase.BButton) ||*/ UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if(backButtonEvent != null)
            {
                backButtonEvent.Invoke();
            }
        }

        // Select
        if(/*Input.GetButtonDown(InputDataBase.AButton) ||*/ UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if(selectButtonEvent != null)
            {
                selectButtonEvent.Invoke();
            }
        }
    }

    private void AnyButtonPressed()
    {
        if(Input.anyKeyDown)
        {
            if(anyButtonWasPressedEvent != null)
            {
                anyButtonWasPressedEvent.Invoke();
            }
        }
    }
}

[System.Serializable]
public class InputEventHandler
{
  
}

[System.Serializable]
public class InputDataBase
{
	public const string LeftStickHorizontal = "LeftStickHorizontal";
    public const string LeftStickVertical = "LeftStickVertical"; 
    
    public const string RightStickHorizontal = "RightStickHorizontal";
    public const string RightStickVertical = "RightStickVertical";

    public const string AButton = "AButton";
    public const string BButton = "BButton";
    public const string YButton = "YButton";
    public const string XButton = "XButton";

    public const string RightBumper = "RightBumper";
    public const string RightTrigger = "RightTrigger";

    public const string LeftBumper = "LeftBumper";
    public const string LeftTrigger = "LeftTrigger";
    
    // Keyboard inputs
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";

    public const string MouseX = "Mouse X";
    public const string MouseY = "Mouse Y";
}