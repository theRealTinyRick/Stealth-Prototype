using System;
using UnityEngine;

public class Vector3Utility
{
    public static float DistanceSquared(Vector3 to, Vector3 from, bool planar = false)
    {
        if(planar)
        {
            to.y = from.y;
        }

        Vector3 _ref = to - from;
        return (_ref.x * _ref.x) + (_ref.y * _ref.y) + (_ref.z + _ref.z);
    }

    public static float PlanarDistance(Vector3 to, Vector3 from)
    {
        to.y = from.y;
        return Vector3.Distance(to, from);
    }

    public static bool Approximately()
    {
        return false;
    }
}