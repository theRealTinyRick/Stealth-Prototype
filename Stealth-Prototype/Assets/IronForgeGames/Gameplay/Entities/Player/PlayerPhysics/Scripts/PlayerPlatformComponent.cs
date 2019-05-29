using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;

public class PlayerPlatformComponent : MonoBehaviour
{
    [SerializeField]
    [TabGroup(Tabs.Properties)]
    private LayerMask platformLayer;

	public void CheckPlatform(GameObject ground)
    {
        if(LayerMaskUtility.IsWithinLayerMask(platformLayer, ground.layer))
        {
            transform.SetParent(ground.transform);
        }
        else
        {
            if(transform.parent != null)
            {
                transform.parent = null;
            }
        }
    }

    public void OnIsNotOnGround()
    {
        if (transform.parent != null)
        {
            transform.parent = null;
        }
    }
}
