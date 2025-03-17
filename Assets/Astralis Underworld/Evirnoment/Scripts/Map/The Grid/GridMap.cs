using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    public class GridMap : MonoSingleton<GridMap>
    {
        public int MapSeed;

        public List<GridRegion> regions;
        public List<GridCell> GridsNearPlayer;

        public GridRegion RegionPlayerIsIn;
        public GridChunk ChunkPlayerIsIn;

        private GridRegion _lastFoundRegion;

        private int _regionSize;
        private int _chunkSize;
        private PlayerFacade _player;
        private void Awake()
        {
            regions = new List<GridRegion>();
            _regionSize = GameConstants.GetRegionSize();
            _chunkSize = GameConstants.GetChunkSize();

            if (MapSeed <= 0)
            {
                MapSeed = GameConstants.GenerateWorldSeed();
            }
            else
            {
                GameConstants.WorldSeed = MapSeed;
            }

            _player = PlayerFacade.instance;
            int playerPosX = (int)_player.transform.position.x / _regionSize;
            int playerPosZ = (int)_player.transform.position.z / _regionSize;
            Vector3 startingRegionPos = new Vector3(playerPosX, 0, playerPosZ);
            RegionPlayerIsIn = CreateRegion(startingRegionPos);
            RegionPlayerIsIn.Activate();
            SwitchSurroundingRegions(RegionPlayerIsIn);
        }

        private void Start()
        {
           // CreateSpaceForPlayer();
        }

        public GridRegion GetRegion(string regionName)
        {
            GridRegion regionFound = null;
            foreach (GridRegion region in regions)
            {
                if (region.RegionName == regionName)
                {
                    regionFound = region;
                }
            }
            return regionFound;
        }

        public GridRegion CreateRegion(Vector3 regionPosition)
        {
            GameObject newGameObject = new GameObject();

            newGameObject.AddComponent<GridRegion>();

            GridRegion newRegion = newGameObject.GetComponent<GridRegion>();
            int regionX = (int)regionPosition.x / _regionSize;
            int regionZ = (int)regionPosition.z / _regionSize;

            regionPosition = new Vector3(regionX * _regionSize, 0, regionZ * _regionSize);
            string newRegionName = string.Format("Region: {0} - {1}", regionX, regionZ);

            newRegion.SetNewRegion(newRegionName, regionPosition, regionX, regionZ);

            regions.Add(newRegion);
            newRegion.transform.parent = transform;

            return newRegion;
        }

        private void CheckForPlayerRegionChange()
        {
            float playerPosX = PlayerFacade.instance.transform.position.x;
            float playerPosZ = PlayerFacade.instance.transform.position.z;
            float currRegionX = RegionPlayerIsIn.transform.position.x;
            float currRegionZ = RegionPlayerIsIn.transform.position.z;

            bool playerIsInsideX = playerPosX >= currRegionX && playerPosX <= currRegionX + _regionSize;
            bool playerIsInsideZ = playerPosZ >= currRegionZ && playerPosZ <= currRegionZ + _regionSize;

            if (playerIsInsideX && playerIsInsideZ) return;

            playerPosX = (int)playerPosX;
            playerPosZ = (int)playerPosZ;

            if (playerPosX < 0) playerPosX -= _regionSize;
            if (playerPosZ < 0) playerPosZ -= _regionSize;

            DeActivateAllRegions();

            if (RegionExists((int)playerPosX / _regionSize, (int)playerPosZ / _regionSize, true))
            {
                // LOAD REGION TODO;
            }
            else
            {
                Vector3 newRegionPosition = new Vector3(playerPosX, 0, playerPosZ);
                RegionPlayerIsIn = CreateRegion(newRegionPosition);
            }

            RegionPlayerIsIn.gameObject.SetActive(true);

            SwitchSurroundingRegions(RegionPlayerIsIn);
            DisableNonActiveRegions();
        }
        private void SwitchSurroundingRegions(GridRegion ceterRegion)
        {
            ceterRegion.IsActive = true;

            var neighborRegions = new List<GridRegion>();
            GridRegion topLeft = LoadOrCreateRegion(ceterRegion.RegionX - 1, ceterRegion.RegionZ + 1);
            GridRegion top = LoadOrCreateRegion(ceterRegion.RegionX, ceterRegion.RegionZ + 1);
            GridRegion topRight = LoadOrCreateRegion(ceterRegion.RegionX + 1, ceterRegion.RegionZ + 1);
            GridRegion right = LoadOrCreateRegion(ceterRegion.RegionX + 1, ceterRegion.RegionZ);
            GridRegion bottomRight = LoadOrCreateRegion(ceterRegion.RegionX + 1, ceterRegion.RegionZ - 1);
            GridRegion bottom = LoadOrCreateRegion(ceterRegion.RegionX, ceterRegion.RegionZ - 1);
            GridRegion bottomLeft = LoadOrCreateRegion(ceterRegion.RegionX - 1, ceterRegion.RegionZ - 1);
            GridRegion left = LoadOrCreateRegion(ceterRegion.RegionX - 1, ceterRegion.RegionZ);

            neighborRegions.Add(topLeft);
            neighborRegions.Add(top);
            neighborRegions.Add(topRight);
            neighborRegions.Add(right);
            neighborRegions.Add(bottomRight);
            neighborRegions.Add(bottom);
            neighborRegions.Add(bottomLeft);
            neighborRegions.Add(left);

            for (int i = 0; i < neighborRegions.Count; i++)
            {
                neighborRegions[i].gameObject.SetActive(true);
                neighborRegions[i].Activate();
            }

            FindChunkThatPlayerIsIn();
        }

        private GridRegion LoadOrCreateRegion(int x, int z)
        {
            if (RegionExists(x, z))
            {
                return _lastFoundRegion; // load stuff TODO
            }
            else
            {
                Vector3 newRegionPos = new Vector3(x * _regionSize, 0, z * _regionSize);
                return CreateRegion(newRegionPos);
            }
        }

        private void DeActivateAllRegions()
        {
            for (int i = 0; i < regions.Count; i++)
            {
                regions[i].IsActive = false;
            }
        }
        private void DisableNonActiveRegions()
        {
            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i].IsActive) continue;
                regions[i].gameObject.SetActive(false);
            }
        }

        private bool RegionExists(int x, int z)
        {
            string nameOfRegionToCheck = string.Format("Region: {0} - {1}", x, z);
            GridRegion regionChecked = GetRegion(nameOfRegionToCheck);

            if (regionChecked == null) return false;

            _lastFoundRegion = regionChecked; // TO Rework?
            return true;
        }

        private bool RegionExists(int x, int z, bool setCurrentRegionToThis)
        {
            string nameOfRegionToCheck = string.Format("Region: {0} - {1}", x, z);
            GridRegion regionChecked = GetRegion(nameOfRegionToCheck);

            if (regionChecked == null) return false;

            if (setCurrentRegionToThis) { RegionPlayerIsIn = regionChecked; } // TO Rework?
            return true;
        }

        private void FindChunkThatPlayerIsIn()
        {
            var x = (int)_player.transform.position.x / _chunkSize;
            var z = (int)_player.transform.position.z / _chunkSize;
            ChunkPlayerIsIn = RegionPlayerIsIn.FindChunkAt(x,z);
        }

/*        private void CreateSpaceForPlayer()
        {
            List<GridCell>  sortedCells = RegionPlayerIsIn.GetSortedCellsByDistance(ChunkPlayerIsIn);
            for (var i = 0; i < 25; i++)
            {
                sortedCells[i].DestroyThisCell();
            }
        }*/

        private void Update()
        {
            CheckForPlayerRegionChange();
        }

    }
}