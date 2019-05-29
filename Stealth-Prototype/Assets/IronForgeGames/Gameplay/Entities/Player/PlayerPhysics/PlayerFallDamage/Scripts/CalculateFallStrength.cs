using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class CalculateFallStrength : MonoBehaviour {

    private PlayerGroundedComponent playerGroundedComponent;

    /// <summary>
    /// This is the vertical position of the player while on the ground
    /// </summary>
    private float currentVerticalPosition;

    /// <summary>
    /// 
    /// </summary>
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float hardFallthreshold;

    void Start ()
    {
        playerGroundedComponent = transform.root.GetComponentInChildren<PlayerGroundedComponent>();
	}

    void Update()
    {
        if (playerGroundedComponent)
        {
            if(playerGroundedComponent.IsGrounded)
            {
                float _currentValue = transform.root.position.y;

                if (_currentValue < currentVerticalPosition)
                {
                    float _fallDistance = currentVerticalPosition - _currentValue;

                   // if()
                }

               // currentVerticalPosition
            }
        }
	}
}
