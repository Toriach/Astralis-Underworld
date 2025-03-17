using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    [System.Serializable]
    public class GridRegion : MonoBehaviour
    {
        public List<GridChunk> chunks;
        public string RegionName;
        public Vector3 RegionPosition;
        public int RegionX;
        public int RegionZ;
        public bool IsActive = false;
        private int _regionSize;
        private int _chunkSize;

        public void SetNewRegion(string name, Vector3 position, int x, int z)
        {
            RegionName = name;
            _regionSize = GameConstants.GetRegionSize();
            _chunkSize = GameConstants.GetChunkSize();
            gameObject.name = RegionName;
            gameObject.transform.position = position;
            RegionPosition = position;
            RegionX = x;
            RegionZ = z;
            IsActive = true;

            chunks = new List<GridChunk>();

            CreateChunks();
        }

        public void Activate()
        {
            IsActive = true;
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i].Activate();
            }
        }

        private void CreateChunks()
        {
            Vector3 newChunkPosition;
            for (int i = 0; i < GameConstants.RegionSizeInChunks; i++)
            {
                for (int j = 0; j < GameConstants.RegionSizeInChunks; j++)
                {
                    newChunkPosition = new Vector3(RegionPosition.x + i * _chunkSize, 0, RegionPosition.z + j * _chunkSize);
                    float x = RegionPosition.x + i;
                    float z = RegionPosition.z + j;
                    chunks.Add(CreateChunk(newChunkPosition, x, z));
                }
            }
        }
        private GridChunk CreateChunk(Vector3 chunkPosition, float localX, float localY)
        {
            GameObject newGameObject = new GameObject();

            newGameObject.AddComponent<GridChunk>();

            GridChunk newChunk = newGameObject.GetComponent<GridChunk>();
            int chunkX = (int)chunkPosition.x / _chunkSize;
            int chunkZ = (int)chunkPosition.z / _chunkSize;

            chunkPosition = new Vector3(chunkX * _chunkSize, 0, chunkZ * _chunkSize);
            string newChunkName = string.Format("Chunk: {0} - {1}", chunkX, chunkZ);

            newChunk.SetNewChunk(newChunkName, chunkPosition, localX, localY);

            newChunk.transform.parent = transform;

            return newChunk;
        }

        public GridChunk FindChunkAt(int x, int z)
        {
            string nameOfChunkToFind = string.Format("Chunk: {0} - {1}", x, z);

            GridChunk chunkFound = null;
            foreach (GridChunk chunk in chunks)
            {
                if (chunk.ChunkName == nameOfChunkToFind)
                {
                    chunkFound = chunk;
                }
            }
            return chunkFound;
        }

        public List<GridCell> GetSortedCellsByDistance(GridChunk chunk)
        {
            return GridCellsOperations.SortByDistance(chunk.Cells);
        }
    }
}