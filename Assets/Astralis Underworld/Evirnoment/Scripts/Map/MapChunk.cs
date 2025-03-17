using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    [RequireComponent(typeof(RocksDestruction), typeof(BoxCollider), typeof(ChunkDataGenerator))]
    [RequireComponent(typeof(ChunkBlocksDrawer))]
    public class MapChunk : MonoBehaviour
    {
        public List<BlockData> allBlockInChunk;
        private List<BlockData> _destroyedBlocks;
        private List<Matrix4x4[]> blockMatrixlist;

        private RocksDestruction _rocksDestruction;
        private BoxCollider _boxCollider;
        private ChunkDataGenerator _chunkDataGenerator;
        private ChunkBlocksDrawer _chunkBlocksDrawer;

        private List<BlockData> rockBlocks;
        private List<BlockData> resourceBlocks;
        private List<BlockData> rareBlocks;
        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("MainBlock"); 
            _boxCollider = GetComponent<BoxCollider>();
            _rocksDestruction = GetComponent<RocksDestruction>();
            _chunkDataGenerator = GetComponent<ChunkDataGenerator>();
            _chunkBlocksDrawer = GetComponent<ChunkBlocksDrawer>();

            _boxCollider.isTrigger = true;

            rockBlocks = new List<BlockData>();
            resourceBlocks = new List<BlockData>();
            rareBlocks = new List<BlockData>();
            _chunkDataGenerator.SetRockLists(rockBlocks, resourceBlocks, rareBlocks);
        }

        public void Activate(float offsetX, float offsetY)
        {
            ReGenerateChunk(GameConstants.ChunkSizeInBlocks, offsetX, offsetY);
        }

        public void ReGenerateChunk(int chunkSize, float offsetX, float offsetY)
        {
            float halfChunkSize = chunkSize / 2f;
            _boxCollider.center = new Vector3(halfChunkSize / 2, 1, halfChunkSize /2);
            _boxCollider.size = new Vector3(halfChunkSize, 2f, halfChunkSize);

            blockMatrixlist = _chunkDataGenerator.GenerateChunkData(transform.position, chunkSize,offsetX, offsetY);
            _chunkBlocksDrawer.UpdateChunkDrawData(blockMatrixlist);
            allBlockInChunk = _chunkDataGenerator.GetChunkBlocks();
        }

        public void Hit()
        {
            _rocksDestruction.HitRocks(allBlockInChunk);

            ModifyDrawList();

            _chunkBlocksDrawer.UpdateChunkDrawData(blockMatrixlist);

            if(allBlockInChunk.Count <= 0)
            {
                _boxCollider.enabled = false;
            }
        }

        private void ModifyDrawList()
        {
            rockBlocks.Clear();
            resourceBlocks.Clear();
            rareBlocks.Clear();

            _destroyedBlocks = new List<BlockData>();

            blockMatrixlist.Clear();
            for (int i = 0; i < allBlockInChunk.Count; i++)
            {
                if (allBlockInChunk[i].IsDestroyed)
                {
                    _destroyedBlocks.Add(allBlockInChunk[i]);
                    continue;
                }

                switch (allBlockInChunk[i].MaterialID)
                {
                    case 0:
                        rockBlocks.Add(allBlockInChunk[i]);
                        break;
                    case 1:
                        rareBlocks.Add(allBlockInChunk[i]);
                        break;
                    case 2:
                        resourceBlocks.Add(allBlockInChunk[i]);
                        break;
                }
            }
            RemoveDestroyedBlocks();
            CreateArrays();
        }

        private void CreateArrays() // duplicated code :"(
        {
            var stoneBlockMatrix = new Matrix4x4[rockBlocks.Count];
            var resourceBlockMatrix = new Matrix4x4[resourceBlocks.Count];
            var rareBlockMatrix = new Matrix4x4[rareBlocks.Count];

            for (int i = 0; i < rockBlocks.Count; ++i)
            {
                stoneBlockMatrix[i] = rockBlocks[i].matrix;
            }

            for (int i = 0; i < resourceBlocks.Count; ++i)
            {
                resourceBlockMatrix[i] = resourceBlocks[i].matrix;
            }

            for (int i = 0; i < rareBlocks.Count; ++i)
            {
                rareBlockMatrix[i] = rareBlocks[i].matrix;
            }

            blockMatrixlist.Add(stoneBlockMatrix);
            blockMatrixlist.Add(resourceBlockMatrix);
            blockMatrixlist.Add(rareBlockMatrix);
        }

        private void RemoveDestroyedBlocks()
        {
            for (int i = 0;i< _destroyedBlocks.Count; ++i)
            {
                allBlockInChunk.Remove(_destroyedBlocks[i]);
            }
        }
    }
}