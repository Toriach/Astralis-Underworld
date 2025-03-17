using Assets.Astralis_Underworld.Evirnoment.Scripts;
using System.Collections.Generic;
using UnityEngine;


namespace Astralis_Underworld.Evirnoment
{
    public class MapGeneratorOld : MonoBehaviour
    {
        [SerializeField] private GameObject RockPrefab;
        [SerializeField] private GameObject DirtPrefab;
        [SerializeField] private GameObject CoalPrefab;
        [SerializeField] private int mapWidth, mapHeight;
        [SerializeField] private int mapDepth = 1;
        [SerializeField] private float blockSize = 2;
        [SerializeField] private float mapYOffset = 0;
        [Space]
        [Min(1f)]
        [SerializeField] private float noiseScale = 1.0f;
        [SerializeField] private float seed = 1.0f;

        [SerializeField] private float xOffset = 0;
        [SerializeField] private float yOffset = 0;
        [SerializeField] private float zOffset = 0;
        [Range(-0.2f,0.2f)]
        [SerializeField] private float minMaxOffset = 0;
         public UnityEngine.UI.Image minimap;

        [Space]
        public float lowestV;
        public float highestV;
        
        private float _yOffset, _xOffset,_zOffset;
        private float[,] mapData;
        private float[,,] mapData3D;

        private GameObject _prefabToSpawn;
        private UnityEngine.Color _blockColor;

        private List<MainBlockRoot> mainBlockRoots;

        public float oldNoise;
        public float oldSeed;
        private void Start()
        {
            _xOffset = (mapWidth / 2) * blockSize;
            _yOffset = (mapHeight / 2) * blockSize;
            _zOffset = (mapDepth / 2) * blockSize;
            mainBlockRoots = new List<MainBlockRoot>();
            GenerateMap();
            oldNoise = noiseScale;
            oldSeed = seed;
          //  StaticBatchingUtility.Combine(gameObject);
        }

        [ContextMenu("Re-Generate map")]
        private void ReGenerateMap()
        {
            for (int i = 0; i < mainBlockRoots.Count; i++)
            {
                Destroy(mainBlockRoots[i].gameObject);
            }
            mainBlockRoots.Clear();
            GenerateMap();
        }
        private void GenerateMap()
        {
            // GenerateData();
            // GenerateData3D();
          //  mapData3D = ChunkNoiseDataGenerator.GenerateChunkData(125);
             Texture2D texture = new Texture2D(mapWidth, mapHeight);

            mapWidth = mapData3D.GetLength(0);
            mapHeight = mapData3D.GetLength(1);
            mapDepth = mapData3D.GetLength(2);
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    for (int d = 0; d < mapDepth; d++)
                    {
                       // _prefabToSpawn = RockPrefab;

                        float value = mapData3D[i, j, d];
                         texture.SetPixel(i, j, new UnityEngine.Color(value, value, value));

                        switch (value)
                        {
                            case < 0.3f:
                                _prefabToSpawn = CoalPrefab;
                           //     texture.SetPixel(i, j, UnityEngine.Color.black);
                                break;
                            case < 0.6f:
                                _prefabToSpawn = RockPrefab;
                                //   texture.SetPixel(i, j, UnityEngine.Color.white);
                                break;
                            case < 0.8f:
                                _prefabToSpawn = DirtPrefab;
                               // texture.SetPixel(i, j, UnityEngine.Color.yellow);
                                break;
                            case <= 1f:
                                _prefabToSpawn = RockPrefab;
                                // texture.SetPixel(i, j, UnityEngine.Color.white);
                                break;
                                default:
                                _prefabToSpawn = null;
                                break;

                        }

                        if (_prefabToSpawn == null) continue;

                         Vector3 blockPosition = new Vector3(_xOffset - i * blockSize, _zOffset - d * blockSize + (mapDepth * blockSize), _yOffset - j * blockSize);
                         MainBlockRoot block = Instantiate(_prefabToSpawn, blockPosition, Quaternion.identity, transform).GetComponent<MainBlockRoot>();
                         mainBlockRoots.Add(block);
                    }
                }
            }
           // texture.Apply();
           //
           // Sprite sprite = Sprite.Create(texture, new Rect(0, 0, mapWidth, mapHeight), Vector2.zero);
           // minimap.sprite = sprite;

            StaticBatchingUtility.Combine(gameObject);
        }

        private void Update()
        {
            if (oldSeed == seed) return;
            oldNoise = noiseScale;
            oldSeed = seed;
            ReGenerateMap();
        }
        private void GenerateData()
        {
            mapData = new float[mapWidth, mapHeight];
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    float xCord = (float)i / mapWidth * noiseScale + xOffset;
                    float yCord = (float)j / mapHeight * noiseScale + yOffset;
                    mapData[i, j] = Mathf.PerlinNoise(xCord,yCord);
                }
            }
        }
        private void GenerateData3D()
        {
            highestV = 0;
            lowestV = 125;

/*            xOffset = Random.Range(-10f, 10f);
            yOffset = Random.Range(-10f, 10f);
            zOffset = Random.Range(-10f, 10f);*/
            mapData3D = new float[mapWidth, mapHeight, mapDepth];
            /*for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    for (int d = 0; d < mapDepth; d++)
                    {
                        /*                        float xCord = (float)i / mapWidth * noiseScale + xOffset;
                                                float yCord = (float)j / mapHeight * noiseScale + yOffset;
                                                float zCord = (float)d / mapDepth * noiseScale + zOffset;#1#

                        float xCord = (float)i / mapWidth * noiseScale + seed;
                        float yCord = (float)j / mapHeight * noiseScale + seed;
                        float zCord = (float)d / mapDepth * noiseScale + seed;
                        mapData3D[i, j, d] = Perlin3DNoise.Get3DNoiseAt(xCord,yCord,zCord,minMaxOffset);
                        if (mapData3D[i, j, d] < lowestV) lowestV = mapData3D[i, j, d];
                        if (mapData3D[i, j, d] > highestV) highestV = mapData3D[i, j, d];
                    }
                }
            }*/
        }
    }
}