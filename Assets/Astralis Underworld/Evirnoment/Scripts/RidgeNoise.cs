using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class RidgeNoise : MonoBehaviour
    {
        /// <summary>
        /// Ridge Noise - "grzbietowy" wariant Perlin Noise
        /// Zwraca wartość w zakresie 0–1
        /// </summary>
        public static float GetValue(float x, float z, int seed, float scale = 0.05f, int octaves = 4, float lacunarity = 2.0f, float gain = 0.5f)
        {
            if (scale <= 0f) scale = 0.0001f;

            float amplitude = 1.0f;
            float frequency = 1.0f;
            float sum = 0.0f;
            float normalization = 0.0f;

            // Wprowadzamy seed do przesunięcia współrzędnych
            float offsetX = seed * 37.1f;
            float offsetZ = seed * 91.7f;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x + offsetX) * scale * frequency;
                float sampleZ = (z + offsetZ) * scale * frequency;

                float noise = Mathf.PerlinNoise(sampleX, sampleZ);
                // ridge transform → odwracamy i wzmacniamy grzbiety
                noise = 1f - Mathf.Abs(noise * 2f - 1f);
                noise *= noise; // zwiększa ostrość grzbietów

                sum += noise * amplitude;
                normalization += amplitude;

                amplitude *= gain;
                frequency *= lacunarity;
            }

            // Normalizacja do 0–1
            return Mathf.Clamp01(sum / normalization);
        }

        public static float GetValue3D(
            float x, float y, float z,
            int seed,
            float scale,
            int octaves,
            float lacunarity,
            float gain,
            float verticalBias // 0 = dół, 1 = góra
)
        {
            float amplitude = 1.0f;
            float frequency = scale;
            float value = 0f;
            float totalAmplitude = 0f;
            float weight = 1f;
            const float offset = 1.0f;

            for (int i = 0; i < octaves; i++)
            {
                // pseudo-3D noise z 3x 2D PerlinNoise
                float n = (
                    Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency) +
                    Mathf.PerlinNoise((y + seed) * frequency, (z + seed) * frequency) +
                    Mathf.PerlinNoise((z + seed) * frequency, (x + seed) * frequency)
                ) / 3f;

                n = offset - Mathf.Abs(n);
                n *= n;
                n *= weight;
                weight = Mathf.Clamp01(n * gain);

                value += n * amplitude;
                totalAmplitude += amplitude;

                frequency *= lacunarity;
                amplitude *= gain;
            }

            // Normalizacja wyniku do 0–1
            value /= totalAmplitude;
            value = Mathf.Clamp01(value);

            // 🎚️ Vertical bias (rozłożenie w pionie bez zmniejszania kontrastu)
            float yNorm = Mathf.Clamp01(y / 4f); // max wysokość 4
            float biasCurve = Mathf.Lerp(1f - yNorm, yNorm, verticalBias);
            float biasStrength = Mathf.Abs(verticalBias - 0.5f) * 2f;

            // mieszamy w zależności od biasStrength
            value = Mathf.Lerp(value, biasCurve * value + (1f - biasCurve) * (1f - value), biasStrength);

            return Mathf.Clamp01(value);
        }
    }
}