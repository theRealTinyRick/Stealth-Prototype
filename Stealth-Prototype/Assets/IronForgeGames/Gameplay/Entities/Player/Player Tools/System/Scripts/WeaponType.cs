using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Type", menuName = "CompanyName/WeaponType", order = 1)]
public class WeaponType : SerializedScriptableObject
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    public Dictionary<string[], bool> animationSets = new Dictionary<string[], bool>();

    public string[] animations;
    public float clickTime;
    public Handedness handedness;

    public string[] GetAnimations()
    {
        // string[] _result = animationSets.Keys.ToList().Find(_key => animationSets[_key] == false);

        // if (_result != null)
        // {
        //     return _result;
        // }

        // foreach(string[] _key in animationSets.Keys)
        // {
        //     animationSets[_key] = false;
        // }

        // return animationSets.Keys.First();

        return animations;
    } 
}
