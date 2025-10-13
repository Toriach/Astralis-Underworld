using Assets.Astralis_Underworld.Evirnoment.Scripts.Map;
using Assets.Astralis_Underworld.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    [System.Serializable]
    public class BlockData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public float NoiseValue;
        public int MaterialID;
        public bool IsDestroyed = false;

        public List<BlockFace> blockFaces = new List<BlockFace>();

        public int TesselationScale = 1;

        public bool HaveTop;
        public bool HaveUp;
        public bool HaveDown;
        public bool HaveLeft;
        public bool HaveRight;

        public int MaxHealth = 5;

        private int Health = 1;

        private int _cellXIndex;
        private int _cellZIndex;
        private int _yIndexInCell;

        public BlockData(int cellXIndex, int cellZIndex, int yIndexInCell, int tesselationScale = 1)
        {
            _cellXIndex = cellXIndex;
            _cellZIndex = cellZIndex;
            _yIndexInCell = yIndexInCell;
            TesselationScale = tesselationScale;
        }

        public void SetUpBlock()
        {
            Health = MaxHealth;
            float scale = 0.09f;// 0.09f
            int octaves = 2;//2f
            float lacunarity = 2f;//2f
            float gain = 0.7f;//0.7f
            float threshold = 0.77f;// > 0.77f

            // tunels
            if (RidgeNoise.GetValue(Position.x, Position.z, GameConstants.WorldSeed, scale, octaves, lacunarity, gain) > threshold)
                IsDestroyed = true;

            float Cscale = 0.10f;// 0.08f
            float Cgain = 0.5f;//0.5f
            float Cthreshold = 0.33f;// > 0.35f
            float verticalBias = 0.35f; // min:0.2f max:0.7f sweet spot 0.35f

            // caves         
            if (RidgeNoise.GetValue3D(Position.x, Position.y, Position.z, GameConstants.WorldSeed,
                Cscale, octaves, lacunarity, Cgain, verticalBias) > Cthreshold)
                IsDestroyed = true;
        }

        public void ReCreateVertices(int tessellationScale)
        {
            if (IsDestroyed) return;
            TesselationScale = tessellationScale;
            CreateVertices(_cellXIndex, _cellZIndex, _yIndexInCell);
        }

        public void CreateVertices(int cellXIndex, int cellZIndex, int yIndexInCell)
        {
            float blockSize = (float)GameConstants.GridSize;
            float x = cellXIndex * blockSize;
            float z = cellZIndex * blockSize;
            float y = yIndexInCell * blockSize;

            float tesselatedBlockSize = blockSize / (float)TesselationScale;

            List<BlockFace> newFaces = new List<BlockFace>();

            BlockFace GetExistingFace(FaceDirection dir)
            {
                if (blockFaces == null) return null;
                return blockFaces.FirstOrDefault(f => f.Direction == dir);
            }

            void AddFace(FaceDirection dir, Func<int, int, Vector3> vertexFunc, bool visible)
            {
                if (!visible) return;

                var existing = GetExistingFace(dir);
                var face = new BlockFace { Direction = dir };

                int vertsPerRow = TesselationScale + 1;
                bool existingHasCorrectSize = existing != null && existing.Vertices.Count == vertsPerRow * vertsPerRow;

                for (int i = 0; i <= TesselationScale; i++)
                {
                    for (int j = 0; j <= TesselationScale; j++)
                    {
                        Vector3 vertex;

                        if (existingHasCorrectSize)
                        {
                            vertex = existing.Vertices[i * vertsPerRow + j];
                        }
                        else
                        {
                            vertex = vertexFunc(i, j);
                        }

                        face.Vertices.Add(vertex);
                        face.UVs.Add(new Vector2((float)j / TesselationScale, (float)i / TesselationScale));
                    }
                }

                newFaces.Add(face);
            }
            AddFace(FaceDirection.Up,
                (i, j) => new Vector3(
                    x + j * tesselatedBlockSize,  
                    y + blockSize,                  
                    z + (TesselationScale - i) * tesselatedBlockSize 
                ),
                HaveTop);

            AddFace(FaceDirection.North,
                (i, j) => new Vector3(
                    x + j * tesselatedBlockSize,
                    y + i * tesselatedBlockSize,
                    z + blockSize
                ),
                HaveUp);

            AddFace(FaceDirection.South,
                (i, j) => new Vector3(
                    x + (TesselationScale - j) * tesselatedBlockSize,
                    y + i * tesselatedBlockSize,
                    z
                ),
                HaveDown);

            AddFace(FaceDirection.West,
                (i, j) => new Vector3(
                    x,
                    y + i * tesselatedBlockSize,
                    z + (TesselationScale - j) * tesselatedBlockSize
                ),
                HaveLeft);

            AddFace(FaceDirection.East,
                (i, j) => new Vector3(
                    x + blockSize,
                    y + i * tesselatedBlockSize,
                    z + j * tesselatedBlockSize
                ),
                HaveRight);

            blockFaces = newFaces;
        }

        public List<int> CreateTrianglesForFace(BlockFace face, int vertexOffset)
        {
            int tesselation = GameConstants.tessellationScale;
            int vertsPerRow = tesselation + 1;
            var triangles = new List<int>(tesselation * tesselation * 6);

            for (int i = 0; i < tesselation; i++)
            {
                int rowStart = i * vertsPerRow;
                int nextRowStart = (i + 1) * vertsPerRow;

                for (int j = 0; j < tesselation; j++)
                {
                    int a = rowStart + j + vertexOffset;
                    int b = a + 1;
                    int d = nextRowStart + j + vertexOffset;
                    int c = d + 1;

                    switch (face.Direction)
                    {
                        case FaceDirection.Up:
                        case FaceDirection.North:
                        case FaceDirection.South:
                            triangles.Add(a); triangles.Add(b); triangles.Add(d);
                            triangles.Add(b); triangles.Add(c); triangles.Add(d);
                            break;

                        case FaceDirection.West:
                        case FaceDirection.East:
                            triangles.Add(a); triangles.Add(d); triangles.Add(b);
                            triangles.Add(b); triangles.Add(d); triangles.Add(c);
                            break;
                    }
                }
            }

            return triangles;
        }

        public int TakeDamage(int incommingDmg)
        {
            int leftOverDamage = incommingDmg - Health;
            if (leftOverDamage < 0) leftOverDamage = 0;

            Health -= incommingDmg;
            if (Health < 0) { IsDestroyed = true; }
            return leftOverDamage;
        }
    }
}