using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        private int _chunkSize;
        private int _regionSquereSize;
        [SerializeField] GameObject chunkPrefab;

        private void Awake()
        {
            _chunkSize = GameConstants.GetChunkSize();
            _regionSquereSize = GameConstants.RegionSizeInChunks;
        }

        private void Start()
        {
            GenerateMap();
        }

        public void GenerateMap()
        {
            Vector3 startPos = transform.position;
            for (int i = 0; i < _regionSquereSize; i++)
            {
                for (int j = 0; j < _regionSquereSize; j++)
                {
                    Vector3 chunkPosition = new Vector3(startPos.x + i * _chunkSize ,0, startPos.z + j * _chunkSize);
                    MapChunk chunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity, transform).GetComponent<MapChunk>();
                  //  chunk.ReGenerateChunk(GameConstants.ChunkSizeInBlocks);
                }
            }
        }
    }
}