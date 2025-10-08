using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Scripts
{
    public static class GameConstants 
    {
        public static float GridSize = 0.5f;
        public static int ChunkSizeInBlocks = 16;//6
        public static int VerticalBlocksInChunk = 4;//4
        public static int RegionSizeInChunks = 3;//3
    
        public static int WorldSeed;

        [Range(0,10)]
        public static int tessellationScale = 3;

        //public static float minningPlayerYOffset = 1.8f;
        public static float minningPlayerYOffset = 2f;
        public static int GetChunkSize()
        {
            int size = (int)(ChunkSizeInBlocks * GridSize);
            if (size <= 0) size = 1;
            return size;
        }

        public static int GetRegionSize()
        {
            int size = (int)(RegionSizeInChunks * ChunkSizeInBlocks * GridSize);
            if (size <= 0) size = 1;
            return size;
        }

        public static int GenerateWorldSeed()
        {
            WorldSeed = (int)Random.Range(1, 125000);
            return WorldSeed;
        }
    }
}