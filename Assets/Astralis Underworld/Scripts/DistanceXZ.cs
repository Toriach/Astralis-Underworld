using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Scripts
{
    public static class DistanceXZ 
    {
        public static float Distance(Vector3 from, Vector3 to)
        {
            var v1 = new Vector2(from.x, from.z);
            var v2 = new Vector2(to.x, to.z);
            return Vector2.Distance(v1, v2);
        }
    }
}