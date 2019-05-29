using UnityEngine;

public class PhysicsLayers
{
    public static LayerMask playerLayer = 1 << 8;
    public static LayerMask ingnorePlayerLayer = ~playerLayer;
}