using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Scripts;
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
        public bool HaveAsignedCollider = false;

        private int _tesselationScale = 1;

        public GridCell(string newCellName, Vector3 cellPosition, BlockData[] blocks)
        {
            SetNewCell(newCellName, cellPosition, blocks);
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
            HaveAsignedCollider = false;
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

        public void DestroyBlocksFromTop(int damage)
        {
            if (IsEmpty) return;
            for (int i = blocks.Length - 1; i >= 0; i--)
            {
                if (blocks[i].IsDestroyed) continue;
                if (damage == 0) break;

                damage = blocks[i].TakeDamage(damage);
            }
            CheckDestroyedBlocks();
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