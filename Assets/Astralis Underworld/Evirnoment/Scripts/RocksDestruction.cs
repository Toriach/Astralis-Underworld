using Astralis_Underworld.Evirnoment;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class RocksDestruction : MonoBehaviour
    {
        [SerializeField] private int damage = 15; 
        private List<BlockData> blocksToDestroy;
        private void Awake()
        {
            blocksToDestroy = new List<BlockData>();
        }
        public void HitRocks(List<BlockData> allBlocks)
        {
            blocksToDestroy.Clear();
            SortByDistance(allBlocks);

            int max = allBlocks.Count;
            int min = damage;

            if (min > max) min = max;
            int randomNrRocksToDestroy = Random.Range(min, damage + 15);
            if (randomNrRocksToDestroy > max) randomNrRocksToDestroy = max;

            for (int i = 0; i < randomNrRocksToDestroy; i++)
            {
                blocksToDestroy.Add(allBlocks[i]);
            }

            DestroyBlocks();

            for (int i = 0; i < blocksToDestroy.Count; i++)
            {
                allBlocks.Remove(blocksToDestroy[i]);
            }

/*            max = allBlocks.Count;
            if (randomNrRocksToDestroy > max) randomNrRocksToDestroy = max;
            for (int i = 0; i < randomNrRocksToDestroy; i++)
            {
                allBlocks[i].ReplaceMesh();
            }*/
        }

        private void DestroyBlocks()
        {
            for (int i = 0; i < blocksToDestroy.Count; i++)
            {
                blocksToDestroy[i].IsDestroyed = true;
                blocksToDestroy[i].Position = new Vector3(blocksToDestroy[i].Position.x, 1250, blocksToDestroy[i].Position.z);
            }
        }
        private void SortByDistance(List<BlockData> allBlocks)
        {
            for (int i = 0; i < allBlocks.Count; i++)
            {
                if(allBlocks[i].IsDestroyed) continue;
                allBlocks[i].CalculateDistance();
            }

            allBlocks.Sort((one, secound) =>
                one.DistanceToPlayer.CompareTo(secound.DistanceToPlayer));
        }

      
    }
}