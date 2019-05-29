using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// not finished
/// </summary>
public class AnimatorParameter
{
    private string _parameterName;

    public string parameterName
    {
        get
        {
            return _parameterName;
        }
        set
        {
            if(value == _parameterName)
            {
                return;
            }
            hash = Animator.StringToHash(value);

            _parameterName = value;
        }
    }

    public int hash;
}
