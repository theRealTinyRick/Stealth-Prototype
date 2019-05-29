/*
Author: Aaron Hines
Description: A simple script to provide a more realistic fall by effecting the players y velocity and multiplying gravity. 
The Unity default physics dont look that good so I added this. Plus it keeps weird bouncing from happening.
This should provide the effect that the player is accelerating as they fall
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGravityComponent : MonoBehaviour
{
    [SerializeField]
    private float fallMultiplier;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = transform.root.GetComponentInChildren<Rigidbody>();
    }

	void FixedUpdate ()
    {
        if (_rigidbody.velocity.y  < 0)
        {
         	_rigidbody.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplier - 1) * Time.deltaTime;

        }
        else if (_rigidbody.velocity.y  > 0 )
        {
         	_rigidbody.velocity += Vector3.up * Physics.gravity.y * 2 * Time.deltaTime;
        }
    }
}
