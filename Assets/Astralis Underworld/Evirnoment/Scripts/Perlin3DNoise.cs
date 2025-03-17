using Assets.Astralis_Underworld.Scripts;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public static class Perlin3DNoise
    {
        public static float Get3DNoiseAt(float x, float y, float z, float minMaxOffset, float powMin, float powMax,float noiseScale)
        {
            x += GameConstants.WorldSeed;
            y += GameConstants.WorldSeed;
            z += GameConstants.WorldSeed;

            x *= noiseScale;
            y *= noiseScale;
            z *= noiseScale;
            
            var AB = Mathf.PerlinNoise(x, y);
            var BC = Mathf.PerlinNoise(y, z);
            var AC = Mathf.PerlinNoise(x, z);

            var BA = Mathf.PerlinNoise(y, x);
            var CB = Mathf.PerlinNoise(z, y);
            var CA = Mathf.PerlinNoise(z, x);

            var ABC = AB + BC + AC + BA + CB + CA;
            ABC = ABC / 6;
            ABC += minMaxOffset;

            if (ABC < 0.5f)
            {
                ABC = Mathf.Pow(ABC, powMax);
            }
            else
            {
                ABC = Mathf.Pow(ABC, powMin);
            }

            return ABC;
        }
    }
}