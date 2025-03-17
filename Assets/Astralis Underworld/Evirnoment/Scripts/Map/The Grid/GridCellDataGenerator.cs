using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    public static class GridCellDataGenerator
    {
        public static GridCell GenerateGridCellData(string newCellName, Vector3 cellPosition, float offsetX,
            float offsetY, int cellXIndex, int cellZIndex, int tesselationScale)
        {
            BlockData[] blocks = new BlockData[GameConstants.VerticalBlocksInChunk];
            for (var i = 0; i < blocks.Length; i++)
            {
                blocks[i] = CreateBlockData(cellPosition, i, cellXIndex, cellZIndex, tesselationScale);
                /*var xCord = cellPosition.x * noiseScale + offsetX;
                var yCord = cellPosition.z * noiseScale + offsetY;
                var zCord = i * noiseScale;*/
                //blocks[i].SetUpBlock(Perlin3DNoise.Get3DNoiseAt(xCord, yCord, zCord));
                blocks[i].SetUpBlock();
                ChoseBlockResource(blocks[i]);
            }

            cellPosition = new Vector3(cellPosition.x - 0.25f, cellPosition.y - 0.25f, cellPosition.z - 0.25f);
            var newGridCell = new GridCell(newCellName, cellPosition, blocks);

            return newGridCell;
        }

        private static BlockData CreateBlockData(Vector3 cellPosition, int yPos, int cellXIndex, int cellZIndex,
            int tesselationScale)
        {
            var blockData = new BlockData(cellXIndex, cellZIndex, yPos, tesselationScale);
            var blockSize = GameConstants.GridSize;

            blockData.Position = cellPosition + new Vector3(0, yPos * blockSize, 0);
            blockData.Rotation = Quaternion.identity;
            blockData.Scale = new Vector3(blockSize, blockSize, blockSize);


            return blockData;
        }

        private static void ChoseBlockResource(BlockData block)
        {
            int materialID = 0;

            switch (block.NoiseValue)
            {
                case < 0.5f:
                    materialID = 0;
                    break;
                case <= 0.9f:
                    materialID = 1;
                    break;
                case <= 1f:
                    materialID = 0;
                    break;
            }

            block.MaterialID = materialID;
        }
    }
}