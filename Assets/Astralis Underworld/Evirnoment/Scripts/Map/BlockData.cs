using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using System.Collections.Generic;
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

        public List<Vector3> Vertices = new List<Vector3>();
        public List<int> Triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();

        public int TesselationScale = 1;
        public int indexOffset = 0;

        public bool HaveTop;
        public bool HaveUp;
        public bool HaveDown;
        public bool HaveLeft;
        public bool HaveRight;

        public int Health = 5;

        private int _cellXIndex;
        private int _cellZIndex;
        private int _yIndexInCell;

        private int sides;
        public BlockData(int cellXIndex, int cellZIndex, int yIndexInCell, int tesselationScale = 1)
        {
            _cellXIndex = cellXIndex;
            _cellZIndex = cellZIndex;
            _yIndexInCell = yIndexInCell;
            TesselationScale = tesselationScale;
        }

        public void SetUpBlock()
        {
            NoiseValue = Perlin3DNoise.Get3DNoiseAt(Position.x, Position.y, Position.z, 0, 0.8f, 1.25f, 0.2f);

            if (NoiseValue >= 0.5f) IsDestroyed = true;
        }

        public void ReCreateVertices(int tessellationScale)
        {
            if (IsDestroyed) return;
            TesselationScale = tessellationScale;
            Vertices.Clear();
            CreateVertices(_cellXIndex, _cellZIndex, _yIndexInCell);
            SetUVs(GameConstants.tessellationScale);
        }

        public void SetAllSidesTo(bool value)
        {
            HaveTop = value;
            HaveUp = value;
            HaveDown = value;
            HaveLeft = value;
            HaveRight = value;
        }

        public void CreateVertices(int cellXIndex, int cellZIndex, int yIndexInCell)
        {
            var blockSize = GameConstants.GridSize;

            var x = cellXIndex * blockSize;
            var z = cellZIndex * blockSize;
            var y = yIndexInCell * blockSize;

            var tesselatedBlockSize = blockSize / TesselationScale;
            sides = 0;

            if (HaveTop)
            {
                for (var i = 0; i < TesselationScale + 1; i++)
                {
                    for (var j = 0; j < TesselationScale + 1; j++)
                    {
                        Vertices.Add(new Vector3(x + i * tesselatedBlockSize, y + blockSize,
                            z + j * tesselatedBlockSize));
                    }
                }
                sides++;
            }

            if (HaveLeft)
            {
                for (int i = 0; i < TesselationScale + 1; i++)
                {
                    for (int j = 0; j < TesselationScale + 1; j++)
                    {
                        Vertices.Add(new Vector3(x, y + i * tesselatedBlockSize,
                            z + j * tesselatedBlockSize));
                    }
                }
                sides++;
            }

            if (HaveRight)
            {
                for (int i = 0; i < TesselationScale + 1; i++)
                {
                    for (int j = 0; j < TesselationScale + 1; j++)
                    {
                        Vertices.Add(new Vector3(x + blockSize, y + j * tesselatedBlockSize,
                            z + i * tesselatedBlockSize));
                    }
                }
                sides++;
            }

            if (HaveDown)
            {
                for (int i = 0; i < TesselationScale + 1; i++)
                {
                    for (int j = 0; j < TesselationScale + 1; j++)
                    {
                        Vertices.Add(new Vector3(x + i * tesselatedBlockSize,
                            y + j * tesselatedBlockSize, z));
                    }
                }
                sides++;
            }

            if (HaveUp)
            {
                for (int i = 0; i < TesselationScale + 1; i++)
                {
                    for (int j = 0; j < TesselationScale + 1; j++)
                    {
                        Vertices.Add(new Vector3(x + j * tesselatedBlockSize,
                            y + i * tesselatedBlockSize, z + blockSize));
                    }
                }
                sides++;
            }
        }

        public List<int> CreateTriangles(int indexOffset)
        {
            Triangles.Clear();

            if (IsDestroyed) return Triangles; // if destroyed, return empty list.

            int col;
            for (int i = 0; i < sides; i++)
            {
                var sideOffset = i * (TesselationScale + 1) * (TesselationScale + 1);
                for (int j = 0; j < TesselationScale; j++)
                {
                    for (int k = 0; k < TesselationScale; k++)
                    {
                        col = j * (TesselationScale + 1) + k + sideOffset;
                        Triangles.Add(col + indexOffset);
                        Triangles.Add(col + 1 + indexOffset);
                        Triangles.Add(col + TesselationScale + 1 + indexOffset);

                        Triangles.Add(col + TesselationScale + 1 + indexOffset);
                        Triangles.Add(col + 1 + indexOffset);
                        Triangles.Add(col + TesselationScale + 2 + indexOffset);
                    }
                }
            }

            return Triangles;
        }

        /* private void SetUVs()
         {
             uvs.Add(new Vector2(0, 0));
             uvs.Add(new Vector2(0, 1));
             uvs.Add(new Vector2(1, 0));
             uvs.Add(new Vector2(1, 1));

             uvs.Add(new Vector2(0, 0));
             uvs.Add(new Vector2(0, 1));
             uvs.Add(new Vector2(1, 0));
             uvs.Add(new Vector2(1, 1));

             uvs.Add(new Vector2(0, 0));
             uvs.Add(new Vector2(1, 0));
             uvs.Add(new Vector2(0, 1));
             uvs.Add(new Vector2(1, 1));


             //
             uvs.Add(new Vector2(0, 0));
             uvs.Add(new Vector2(1, 0));
             uvs.Add(new Vector2(0, 1));
             uvs.Add(new Vector2(1, 1));

             uvs.Add(new Vector2(0, 0));
             uvs.Add(new Vector2(1, 0));
             uvs.Add(new Vector2(0, 1));
             uvs.Add(new Vector2(1, 1));
         }*/
        private void SetUVs(int tesselationScale)
        {
            // Wyczyść listę UVs, aby uniknąć dublowania
            uvs.Clear();

            // Oblicz odstęp między punktami UV na podstawie tessalacji
            float uvStep = 1f / tesselationScale;

            // Iteruj przez wszystkie wierzchołki
            for (int i = 0; i < Vertices.Count; i++)
            {
                // Oblicz współrzędne UV na podstawie indeksu wierzchołka
                float u = (i % (tesselationScale + 1)) * uvStep;
                float v = (i / (tesselationScale + 1)) * uvStep;

                // Dodaj punkt UV do listy
                uvs.Add(new Vector2(u, v));
            }
        }

        public float DistanceToPlayer; // move to grid

        public Matrix4x4 matrix
        {
            get { return Matrix4x4.TRS(Position, Rotation, Scale); }
        }

        public void CalculateDistance()
        {
            Vector3 playerPos = PlayerFacade.instance.transform.position + Vector3.up * 2;
            DistanceToPlayer = Vector3.Distance(playerPos, Position);
        }

        public int TakeDamage(int incommingDmg)
        {
            int leftOverDamage = incommingDmg - Health;
            if(leftOverDamage < 0) leftOverDamage = 0;

            Health -= incommingDmg;
            if (Health < 0) { IsDestroyed = true; }
            return leftOverDamage;
        }

        }
    }