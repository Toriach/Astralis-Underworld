﻿using Assets.Astralis_Underworld.Scripts;
using Astralis_Underworld.Evirnoment;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [Serializable]
    public class GridChunk : MonoBehaviour
    {
        public List<GridCell> Cells;
        public GridCell[,] CellTable;
        public string ChunkName;
        public Vector3 ChunkPosition;
        public Vector3 ChunkCenterPosition;

        public List<Vector3> blocksVerts;

        //public List<List<int>> GroupedVertices;
        public List<int> blocksTriangles;
        public List<Vector2> blocksUvs;

        public int TesselationScale = 1;
        public bool RecreateUpdate = false;

        private int _chunkSize;
        private float _gridSize;
        private float _chunkLocalX;
        private float _chunkLocalY;
        private ChunkBlocksDrawer _chunkBlocksDrawer;
        private List<Matrix4x4[]> blockMatrixlist;

        private int _frameSkipped = 0;

        private MeshFilter _filter;
        private MeshRenderer _renderer;
        private Mesh _mesh;
        private MeshCollider _meshCollider;
        private List<List<int>> indexesInDistance;

        private void Awake()
        {
            _chunkBlocksDrawer = GetComponent<ChunkBlocksDrawer>();
            _filter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
            _renderer.sharedMaterial = BlockMaterialReferenceSingleton.instance.materials[0];
            _mesh = new Mesh();
            _filter.sharedMesh = _mesh;

            _meshCollider = gameObject.AddComponent<MeshCollider>();

            gameObject.layer = LayerMask.NameToLayer("Rock");
        }

        public void SetNewChunk(string newChunkName, Vector3 chunkPosition, float chunkLocalX, float chunkLocalY)
        {
            ChunkName = newChunkName;
            _chunkSize = GameConstants.GetChunkSize();
            _gridSize = GameConstants.GridSize;
            gameObject.name = ChunkName;
            gameObject.transform.position = chunkPosition;
            ChunkPosition = chunkPosition;
            float halfChunkSize = (GameConstants.ChunkSizeInBlocks * GameConstants.GridSize) / 2;
            ChunkCenterPosition = new Vector3(ChunkPosition.x + halfChunkSize, ChunkPosition.y,
                ChunkPosition.z + halfChunkSize);

            _chunkLocalX = chunkLocalX;
            _chunkLocalY = chunkLocalY;

            Cells = new List<GridCell>();

            indexesInDistance = new List<List<int>>();
            blockMatrixlist = new List<Matrix4x4[]>();

            CellTable = new GridCell[GameConstants.ChunkSizeInBlocks, GameConstants.ChunkSizeInBlocks];

            CreateGridCells();

            BlocksSideChecker.CheckSides(CellTable);

            // CreateMatrixArrays();

            blocksVerts = new List<Vector3>();
            blocksTriangles = new List<int>();
            blocksUvs = new List<Vector2>();

            TesselationScale = GameConstants.tessellationScale;
            ReGenerateMesh();
        }

        private void ReCheckBlockSides()
        {
            BlocksSideChecker.CheckSides(CellTable);
        }

        public void Activate()
        {
            //_myChunk.Activate(_localX, _localY);
        }

        private void CreateGridCells()
        {
            Vector3 newCellPosition;
            for (int i = 0; i < GameConstants.ChunkSizeInBlocks; i++)
            {
                for (int j = 0; j < GameConstants.ChunkSizeInBlocks; j++)
                {
                    newCellPosition = new Vector3(ChunkPosition.x + i * _gridSize, 0, ChunkPosition.z + j * _gridSize);
                    GridCell cell = CreateCell(newCellPosition, i, j);
                    Cells.Add(cell);
                    CellTable[i, j] = cell;
                }
            }
        }

        private GridCell CreateCell(Vector3 cellPosition, int cellXIndex, int cellZIndex)
        {
            float cellX = cellPosition.x / _gridSize;
            float cellZ = cellPosition.z / _gridSize;

            cellPosition = new Vector3(cellX * _gridSize + _gridSize / 2, 0 + _gridSize / 2,
                cellZ * _gridSize + _gridSize / 2);
            string newCellName = string.Format("Cell: {0} - {1}", cellX, cellZ);

            GridCell newCell = GridCellDataGenerator.GenerateGridCellData(newCellName, cellPosition, _chunkLocalX,
                _chunkLocalY, cellXIndex, cellZIndex, TesselationScale);

            return newCell;
        }

        /*private void CreateMatrixArrays()
        {
            blockMatrixlist.Clear();
            List<Matrix4x4> matrixListStone = new List<Matrix4x4>();
            List<Matrix4x4> matrixListCoal = new List<Matrix4x4>();
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].IsEmpty) continue;

                for (int j = 0; j < Cells[i].blocks.Length; j++)
                {
                    if (Cells[i].blocks[j].IsDestroyed) continue;
                    switch (Cells[i].blocks[j].MaterialID)
                    {
                        case 0:
                            matrixListStone.Add(Cells[i].blocks[j].matrix);
                            break;
                        case 1:
                            matrixListCoal.Add(Cells[i].blocks[j].matrix);
                            break;
                    }
                }
            }

            Matrix4x4[] array = new Matrix4x4[matrixListStone.Count];
            Matrix4x4[] arrayCoal = new Matrix4x4[matrixListCoal.Count];
            array = matrixListStone.ToArray();
            arrayCoal = matrixListCoal.ToArray();
            blockMatrixlist.Add(array);
            blockMatrixlist.Add(arrayCoal);
            //  _chunkBlocksDrawer.UpdateChunkDrawData(blockMatrixlist);
        }*/

/*        private void Update()
        {
            _frameSkipped++;
            if (_frameSkipped < 60) return;
            _frameSkipped = 0;
           // RemoveRandomCell();
          //  if (RecreateUpdate == false) return;
          //  GenerateMesh();
            // CreateMatrixArrays();
        }*/

        [ContextMenu("RE Generate mesh")]
        public void ReGenerateMesh()
        {
            ReCheckBlockSides();
            foreach (var cell in Cells)
            {
                cell.UpdateVerticesInBlocks(TesselationScale);
            }

            ModifyVertsOnCellBlocks();
            GenerateMesh();
        }

        [ContextMenu("Generate mesh")]
        private void GenerateMesh()
        {
            _mesh.Clear();
            if (RecreateUpdate == false)
            {
                blocksVerts.Clear();
                blocksTriangles.Clear();
                blocksUvs.Clear();

                int index = 0;
                for (int i = 0; i < Cells.Count; i++)
                {
                    for (int y = 0; y < Cells[i].blocks.Length; y++)
                    {
                        if (Cells[i].blocks[y].IsDestroyed) continue;
                        for (int j = 0; j < Cells[i].blocks[y].Vertices.Count; j++)
                        {
                            blocksVerts.Add(Cells[i].blocks[y].Vertices[j]);
                            blocksUvs.Add(Cells[i].blocks[y].uvs[j]);
                        }

                        Cells[i].blocks[y].CreateTriangles(index);

                        for (int j = 0; j < Cells[i].blocks[y].Triangles.Count; j++)
                        {
                            blocksTriangles.Add(Cells[i].blocks[y].Triangles[j]);
                        }

                        index += Cells[i].blocks[y].Vertices.Count;
                    }
                }
            }

            //ModifyVerts();

            _mesh.vertices = blocksVerts.ToArray();
            _mesh.triangles = blocksTriangles.ToArray();

            if (_mesh.triangles.Length <= 0) return;

            _mesh.uv = blocksUvs.ToArray();
            _mesh.RecalculateNormals();
            _meshCollider.sharedMesh = _mesh;
        }

        private void UpdateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = blocksVerts.ToArray();
            _mesh.triangles = blocksTriangles.ToArray();
            _mesh.uv = blocksUvs.ToArray();
            _mesh.RecalculateNormals();
            _meshCollider.sharedMesh = _mesh;
        }

        private void ModifyVertsOnCellBlocks()
        {
            var displacmentNoise = 4f;
            var gridSize = GameConstants.GridSize / TesselationScale;

            foreach (var cell in Cells)
            {
                foreach (var block in cell.blocks)
                {
                    for (int i = 0; i < block.Vertices.Count; i++)
                    {
                        var noisePos = (block.Vertices[i] + ChunkPosition) * gridSize;

                        var noiseValue = Perlin3DNoise.Get3DNoiseAt(noisePos.x, noisePos.y, noisePos.z, 0, 0.95f, 1.05f,
                            displacmentNoise);
                        noiseValue -= 0.5f;

                        var offset = new Vector3(noiseValue, 0, noiseValue);

                        block.Vertices[i] += offset;
                    }
                }
            }
        }

        private void RemoveRandomCell()
        {
            int random = UnityEngine.Random.Range(0, Cells.Count - 1);
            Debug.DrawRay(Cells[random].CellPosition,Vector3.up * 3,Color.green,1);
            foreach(var block in Cells[random].blocks)
            {
                block.IsDestroyed = true;
            }
            RecreateUpdate = false;
            ReGenerateMesh();
        }
        /*public void Hit(Transform playerTransform, float maxDistance, float miningPower)
        {
            var cellsInRange = GetCellsInDistanceFromPlayer(maxDistance, playerTransform);

            var playerOffset = new Vector3(0, GameConstants.minningPlayerYOffset, 0);
            var playerPosition = playerTransform.position + playerOffset;
            indexesInDistance.Clear();

            var miningPowerPart = miningPower / 4f;

            /*foreach (var vertexGroup in GroupedVertices)
            {
                var vertexPosition = blocksVerts[vertexGroup[0]] + ChunkPosition;
                var distanceToPlayer = Vector3.Distance(vertexPosition, playerPosition);

                if (distanceToPlayer > maxDistance) continue;
                indexesInDistance.Add(vertexGroup);
            }#1#

            for (int i = 0; i < indexesInDistance.Count; i++)
            {
                var offset = UnityEngine.Random.Range(-miningPowerPart, miningPowerPart);
                foreach (var index in indexesInDistance[i])
                {
                    var direction = playerPosition - (blocksVerts[index] + ChunkPosition);
                    direction.y = 0;
                    direction.Normalize();
                    blocksVerts[index] -= direction * miningPower;
                }
            }
            UpdateMesh();
        }*/
        public void Hit(Transform playerTransform, float maxDistance, float miningPower)
        {
            RecreateUpdate = false;
            var cellsInRange = GetCellsInDistanceFromPlayer(maxDistance * 2, playerTransform);
            foreach (var cell in cellsInRange)
            {
                //Debug.DrawRay(cell.CellPosition, Vector3.up * 10, Color.red, 1f);
                cell.DestroyBlocksFromTop(3);

            }
            RecreateUpdate = false;
            ReGenerateMesh();
        }

        /*     public void Hit(Transform playerTransform, float maxDistance, float miningPower) // newest
         {
             //GridUtilityGetInRange.FinsGridCellByPosition(playerTransform.position);
             *//*var cellsInRange = GetCellsInDistanceFromPlayer(maxDistance, playerTransform);

             var playerOffset = new Vector3(0, GameConstants.minningPlayerYOffset, 0);
             var playerPosition = playerTransform.position + playerOffset;

             foreach (var cell in cellsInRange)
             {
                 Debug.DrawRay(cell.CellPosition,Vector3.up * 10, Color.red, 1f);
                 foreach (var block in cell.blocks)
                 {
                     for (int i = 0; i < block.Vertices.Count; i++)
                     {
                         var vertexPosition = block.Vertices[i] + ChunkPosition;
                         var distanceToPlayer = Vector3.Distance(vertexPosition, playerPosition);

                         if (distanceToPlayer > maxDistance) continue;

                         // Calculate a factor based on the distance to the player
                         float distanceFactor = 1 - (distanceToPlayer / maxDistance);

                         // Apply the factor to the mining power
                         float adjustedMiningPower = miningPower * distanceFactor;

                         var direction = vertexPosition - playerPosition;
                         direction.y = 0;
                         direction.Normalize();

                         block.Vertices[i] += direction * adjustedMiningPower; // This line is changed
                     }
                 }
             }
             GenerateMesh();*//*
         }*/

        public List<GridCell> GetCellsInDistanceFromPlayer(float distance, Transform playerTransform)
        {
            // Get the player's position
            Vector3 playerPos = playerTransform.position;

            // Create a list to store the cells within the specified distance
            List<GridCell> cellsInDistance = new List<GridCell>();

            // Iterate over each cell
            foreach (GridCell cell in Cells)
            {
                // Calculate the distance from the cell to the player
                float cellDistance = Vector3.Distance(cell.CellPosition, playerPos);

                // If the cell is within the specified distance, add it to the list
                if (cellDistance <= distance)
                {
                    cellsInDistance.Add(cell);
                }
            }

            // Return the list of cells within the specified distance
            return cellsInDistance;
        }
    }
}