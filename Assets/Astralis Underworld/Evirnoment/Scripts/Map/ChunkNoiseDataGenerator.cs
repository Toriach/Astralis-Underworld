using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public static class ChunkNoiseDataGenerator
    {
        private static int chunkDepth = 4;

        /*public static float[,,] GenerateChunkData(int chunkSize, float noiseScale, float offsetX, float offsetY)
        {
            float[,,] chunkData = new float[chunkSize, chunkSize, chunkDepth];
            for (int i = 0; i < chunkSize; i++)
            {
                for (int j = 0; j < chunkSize; j++)
                {
                    for (int d = 0; d < chunkDepth; d++)
                    {
                        float xCord = (float)i / chunkSize * noiseScale + offsetX;
                        float yCord = (float)j / chunkSize * noiseScale + offsetY;
                        float zCord = (float)d / chunkDepth * noiseScale;
                        chunkData[i, j, d] = Perlin3DNoise.Get3DNoiseAt(xCord, yCord, zCord);
                    }
                }
            }
            return chunkData;
        }*/
    }
}