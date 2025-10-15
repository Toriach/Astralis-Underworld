using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Scripts
{
    public static class Vector3Utils
    {
        public static bool ApproximatelyEqual(Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return (a - b).sqrMagnitude < tolerance * tolerance;
        }
    }
}