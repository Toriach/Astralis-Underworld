using System;
using Assets.Astralis_Underworld.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    public static class GridUtilityGetInRange
    {
        private static int _regionSize;
        private static int _chunkSize;
        private static float _gridSize;

        static GridUtilityGetInRange()
        {
            _regionSize = GameConstants.GetRegionSize();
            _chunkSize = GameConstants.GetChunkSize();
            _gridSize = GameConstants.GridSize;
        }

        public static List<GridChunk> GetChunksInRange(Vector3 position, float range)
        {
            List<GridChunk> chunksInRange = new List<GridChunk>();

            List<GridRegion> regions = GridMap.instance.regions;

            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i].IsActive == false) continue;

                for (int j = 0; j < regions[i].chunks.Count; j++)
                {
                    float distance = CalculateDistance(position, regions[i].chunks[j].ChunkCenterPosition);
                    if (distance <= range)
                    {
                        chunksInRange.Add(regions[i].chunks[j]);
                    }
                }
            }

            return chunksInRange;
        }

        public static List<GridCell> GetGridCellsInRange(Vector3 position, float rangeForChunks, float rangeForCells)
        {
            List<GridChunk> chunksInRange = GetChunksInRange(position, rangeForChunks);
            List<GridCell> cellsInRange = new List<GridCell>();
            for (int i = 0; i < chunksInRange.Count; i++)
            {
                for (int j = 0; j < chunksInRange[i].Cells.Count; j++)
                {
                    if (chunksInRange[i].Cells[j].IsEmpty) continue;
                    float distance = CalculateDistance(position, chunksInRange[i].Cells[j].CellPosition);
                    if (distance <= rangeForCells)
                    {
                        cellsInRange.Add(chunksInRange[i].Cells[j]);
                    }
                }
            }

            return cellsInRange;
        }

        public static GridCell FindGridCellByPosition(Vector3 position)
        {
            var region = GetRegion(position);
            if (region == null) return null;

          //  Debug.DrawRay(region.RegionPosition, Vector3.up * 15, Color.blue, 1);

            var chunk = FindChunkAt(position, region);
            if (chunk == null) return null;

          //  Debug.DrawRay(chunk.ChunkPosition, Vector3.up * 10, Color.red, 1);
            
            GridCell foundCell = null;
            var cellX = position.x / _gridSize;
            var cellZ = position.z / _gridSize;

            cellX = cellX >= 0 ? (float)Math.Floor(cellX) : (float)Math.Ceiling(cellX);
            cellZ = cellZ >= 0 ? (float)Math.Floor(cellZ) : (float)Math.Ceiling(cellZ);
            
            var cellName = string.Format("Cell: {0} - {1}", cellX , cellZ);
            foreach (var cell in chunk.Cells)
            {
                if (cell.CellName == cellName)
                {
                    foundCell = cell;
                }
            }
          // if(foundCell != null) Debug.DrawRay(foundCell.CellPosition, Vector3.up * 5, Color.green, 1);
            return foundCell;
        }
        
        public static GridRegion GetRegion(Vector3 position)
        {
            GridRegion regionFound = null;

            var regionX = (int)position.x / _regionSize;
            var regionZ = (int)position.z / _regionSize;

            var regionName = string.Format("Region: {0} - {1}", regionX, regionZ);

            foreach (var region in GridMap.instance.regions)
            {
                if (region.RegionName == regionName)
                {
                    regionFound = region;
                }
            }

            return regionFound;
        }

        public static GridChunk FindChunkAt(Vector3 position, GridRegion regionFound)
        {
            var x = (int)position.x / _chunkSize;
            var z = (int)position.z / _chunkSize;

            var nameOfChunkToFind = string.Format("Chunk: {0} - {1}", x, z);

            GridChunk chunkFound = null;
            foreach (var chunk in regionFound.chunks)
            {
                if (chunk.ChunkName == nameOfChunkToFind)
                {
                    chunkFound = chunk;
                }
            }

            return chunkFound;
        }

        private static float CalculateDistance(Vector3 position, Vector3 position2)
        {
            return DistanceXZ.Distance(position, position2);
        }
    }
}