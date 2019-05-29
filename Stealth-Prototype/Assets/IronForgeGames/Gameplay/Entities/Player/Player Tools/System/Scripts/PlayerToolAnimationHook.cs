using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class WeaponInfo
{
    public string eqippedAnimation;
    public string unEqippedAnimation;

    public float weaponTypeAnimatorFloat;
}

public class PlayerToolAnimationHook : SerializedMonoBehaviour
{
    [SerializeField]
    public Dictionary<WeaponType, WeaponInfo> weaponTypeMap = new Dictionary<WeaponType, WeaponInfo>();

    private Animator animator;
    
    public const string EquippedFloatName = "Equipped";
    public const string WeaponFloatName = "WeaponType";

    private void OnEnable()
    {
        animator = transform.root.gameObject.GetComponentInChildren<Animator>();
    }

    public void OnToolEqipped(WeaponType currentWeapon, WeaponType previousWeapon)
    {
        // play animation
        if(weaponTypeMap.ContainsKey(currentWeapon))
        {
            WeaponInfo _weaponInfo = weaponTypeMap[currentWeapon];
            if(_weaponInfo != null)
            {
                animator.SetFloat(EquippedFloatName, 1);
                animator.SetFloat(WeaponFloatName, _weaponInfo.weaponTypeAnimatorFloat);
                // animator.Play(_weaponInfo.eqippedAnimation);
            }
        }
    }    

    public void OnUneqipped()
    {
        
    }
}
