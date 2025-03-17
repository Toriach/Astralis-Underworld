using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using Assets.Astralis_Underworld.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public class ChunkDataGenerator : MonoBehaviour
    {
        public float[,,] chunkNoiseData;
        public float blockSize = 0.5f;

        public bool removeStone = false;

        private Vector3 blockScale;
        private Matrix4x4[] stoneBlockMatrix;
        private Matrix4x4[] resourceBlockMatrix;
        private Matrix4x4[] rareBlockMatrix;

        private List<Matrix4x4[]> chunkBlockMatrixList;

        private List<BlockData> _rockBlocks;
        private List<BlockData> _resourceBlocks;
        private List<BlockData> _rareBlocks;
        [Space]
        private List<BlockData> allBlockInChunk;
        private int _chunkSize;
        public List<Matrix4x4[]> GenerateChunkData(Vector3 startPosition,int chunkSize, float offsetX, float offsetY)
        {
            CreateLists();

            blockScale = new Vector3(blockSize, blockSize, blockSize);
           // chunkNoiseData = ChunkNoiseDataGenerator.GenerateChunkData(chunkSize, GameConstants.PerlinNoiseScale, offsetX, offsetY);
            _chunkSize = chunkSize;

            for (int i = 0; i < chunkNoiseData.GetLength(0); i++)
            {
                for (int j = 0; j < chunkNoiseData.GetLength(1); j++)
                {
                    for (int d = 0; d < chunkNoiseData.GetLength(2); d++)
                    {
                        CreateBlockData(i, j, d, startPosition);
                    }
                }
            }

            CreateArrays();

            return chunkBlockMatrixList;
        }

        private void CreateBlockData(int i, int j, int d, Vector3 startPosition)
        {
           /* float value = chunkNoiseData[i, j, d];
            BlockData blockData = new BlockData();

            int materialID = 0;

            switch (value)
            {
                case < 0.3f:
                    materialID = 1;
                    _rareBlocks.Add(blockData);
                    break;
                case < 0.6f:

                  //  materialID = 0;
                  //  _rockBlocks.Add(blockData);
                    break;

                case <= 1f:
                    materialID = 2;
                    _resourceBlocks.Add(blockData);

                    break;

            }

            blockData.Position = CalculateBlockPosition(i, j, d, startPosition);
            blockData.Rotation = Quaternion.identity;
            blockData.Scale = blockScale;

            blockData.MaterialID = materialID;

            allBlockInChunk.Add(blockData);*/
        }

        private Vector3 CalculateBlockPosition(int i, int j, int d, Vector3 startPosition)
        {
            float _xOffset = _chunkSize * blockSize - blockSize / 2;
            float _yOffset = _chunkSize * blockSize - blockSize / 2;
            float _zOffset = 4 * blockSize - blockSize / 2;

            Vector3 position = new Vector3(
                startPosition.x + _xOffset - i * blockSize,
                 startPosition.y + _zOffset - d * blockSize,
                startPosition.z + _yOffset - j * blockSize

                );

            return position;
        }

        private void CreateArrays()
        {
            stoneBlockMatrix = new Matrix4x4[_rockBlocks.Count];
            resourceBlockMatrix = new Matrix4x4[_resourceBlocks.Count];
            rareBlockMatrix = new Matrix4x4[_rareBlocks.Count];

            for (int i = 0; i < _rockBlocks.Count; ++i)
            {
                stoneBlockMatrix[i] = _rockBlocks[i].matrix;
            }

            for (int i = 0; i < _resourceBlocks.Count; ++i)
            {
                resourceBlockMatrix[i] = _resourceBlocks[i].matrix;
            }

            for (int i = 0; i < _rareBlocks.Count; ++i)
            {
                rareBlockMatrix[i] = _rareBlocks[i].matrix;
            }

            chunkBlockMatrixList.Add(stoneBlockMatrix);
            chunkBlockMatrixList.Add(resourceBlockMatrix);
            chunkBlockMatrixList.Add(rareBlockMatrix);
        }

        private void CreateLists()
        {
            chunkBlockMatrixList = new List<Matrix4x4[]>();
/*
            _rockBlocks = new List<BlockData>();
            _resourceBlocks = new List<BlockData>();
            _rareBlocks = new List<BlockData>();*/

            allBlockInChunk = new List<BlockData>();
        }

        public List<BlockData> GetChunkBlocks()
        {
            return allBlockInChunk;
        }

        public void SetRockLists(List<BlockData> rockBlocks,List<BlockData> resourceBlocks,List<BlockData> rareBlocks)
        {
            _rockBlocks = rockBlocks;
            _resourceBlocks = resourceBlocks;
            _rareBlocks = rareBlocks;
        }
    }
}