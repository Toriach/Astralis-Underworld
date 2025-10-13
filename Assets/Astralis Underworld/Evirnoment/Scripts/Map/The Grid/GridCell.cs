using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Scripts;
using Astralis_Underworld.Evirnoment;
using System;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    [Serializable]
    public class GridCell
    {
        public string CellName;
        public Vector3 CellPosition;
        public BlockData[] blocks = new BlockData[GameConstants.VerticalBlocksInChunk];
        public bool IsEmpty;
        public float DistanceToPlayer;

        private int _tesselationScale = 1;
        private Vector3 _parentPos;
        public GridCell(string newCellName, Vector3 cellPosition, BlockData[] blocks, Vector3 parentChunkPosition)
        {
            SetNewCell(newCellName, cellPosition, blocks);
            _parentPos = parentChunkPosition;
        }

        public void UpdateVerticesInBlocks(int newTesselationScale)
        {
            _tesselationScale = newTesselationScale;
            foreach (var t in blocks)
            {
                t.ReCreateVertices(_tesselationScale);
            }
        }
        private void SetNewCell(string newCellName, Vector3 cellPosition, BlockData[] blocksData)
        {
            CellName = newCellName;
            CellPosition = cellPosition;
            blocks = blocksData;
            CheckDestroyedBlocks();
        }

        public float GetDistanceToPlayer()
        {
            Vector3 playerPos = PlayerFacade.instance.transform.position + Vector3.up * 2;
            DistanceToPlayer = DistanceXZ.Distance(playerPos, CellPosition);
            return DistanceToPlayer;
        }

        public void DestroyBlockAt(int blockIndex)
        {
            if (IsEmpty) return;
            blocks[blockIndex].IsDestroyed = true;
            CheckDestroyedBlocks();
        }

        public void DestroyBlocksFromTop(int damage, Vector3 hitPointPos)
        {
            if (IsEmpty) return;

            for (int i = blocks.Length - 1; i >= 0; i--)
            {
                if (blocks[i].IsDestroyed) continue;
                if (damage == 0) break;

                damage = blocks[i].TakeDamage(damage);
                DeformVertsAfterHit(blocks[i]);
            }
            CheckDestroyedBlocks();
        }

        private void DeformVertsAfterHit(BlockData block)
        {
            // TODO find formulas here
            var maxDistance =  GameConstants.GridSize * GameConstants.ChunkSizeInBlocks ;
            var dispPower = 1;
            var downOffset = 3.5f;

            Vector3 playerPos = PlayerFacade.instance.transform.position;
            foreach (var face in block.blockFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    var vertexPosition = face.Vertices[i] + _parentPos;
                    var distanceToPlayer = Vector3.Distance(vertexPosition, playerPos);

                    if (distanceToPlayer > maxDistance) continue;
      
                    float distanceFactor = 1 - (distanceToPlayer / maxDistance);

                    float adjustedMiningPower = dispPower * distanceFactor;
                    var modifiedPlayerPos = playerPos;
                    modifiedPlayerPos.y += downOffset;

                    // Debug.DrawRay(modifiedPlayerPos, Vector3.up * 1, Color.green, 0.1f);
                    var direction = vertexPosition - modifiedPlayerPos;

                    direction.Normalize();
                    var displacement = direction * adjustedMiningPower;
                   // Debug.DrawRay(vertexPosition, displacement * 1, Color.red, 0.1f);
                   // Debug.Break();
                    face.Vertices[i] += displacement;
                }
            }
        }
        private void CheckDestroyedBlocks()
        {
            if (IsEmpty) return;
            IsEmpty = true;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].IsDestroyed == false)
                {
                    IsEmpty = false;
                }
            }
        }
    }
}