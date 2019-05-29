using UnityEngine;

public class LayerMaskUtility
{
    /// <summary>
    /// Check if a game objects layer is within a Layer Mask - useful for collisions and raycasting
    /// </summary>
    /// <param name="layerMask">The layer masks that you are checking, that the layer is within</param>
    /// <param name="layer">The layer that you want to make sure is within the layer mask</param>
    /// <returns></returns>
    public static bool IsWithinLayerMask(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
