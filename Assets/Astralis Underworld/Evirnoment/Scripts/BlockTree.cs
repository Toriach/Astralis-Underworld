using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class BlockTree : MonoBehaviour
    {
        public float scale;
        public Renderer MainRenderer;
        public GameObject block;
        public List<BlockTree> blocks;
        public int StopAt = 1;

        private void Awake()
        {
            blocks = new List<BlockTree>();
        }

        public void SetScale(float newScale) { scale = newScale; }

        [ContextMenu("Divide")]
        public void Divide()
        {
            MainRenderer.enabled = false;
            Vector3 mainPosition = transform.position;

            Vector3 blockScale = new Vector3(scale, scale, scale) / 2;

            float quaterSize = scale / 4;

            if (scale <= StopAt)
            {
                MainRenderer.enabled = true;
                return;
            }
            blocks.Clear();

            Vector3 LDDPos = new Vector3(mainPosition.x - quaterSize, mainPosition.y - quaterSize, mainPosition.z - quaterSize);
            SpawnBlock(LDDPos, blockScale);

            Vector3 RDDPos = new Vector3(mainPosition.x + quaterSize, mainPosition.y - quaterSize, mainPosition.z - quaterSize);
            SpawnBlock(RDDPos, blockScale);

            Vector3 LUDPos = new Vector3(mainPosition.x - quaterSize, mainPosition.y + quaterSize, mainPosition.z - quaterSize);
            SpawnBlock(LUDPos, blockScale);

            Vector3 RUDPos = new Vector3(mainPosition.x + quaterSize, mainPosition.y + quaterSize, mainPosition.z - quaterSize);
            SpawnBlock(RUDPos, blockScale);

            // --- //

            Vector3 LDUPos = new Vector3(mainPosition.x - quaterSize, mainPosition.y - quaterSize, mainPosition.z + quaterSize);
            SpawnBlock(LDUPos, blockScale);

            Vector3 RDUPos = new Vector3(mainPosition.x + quaterSize, mainPosition.y - quaterSize, mainPosition.z + quaterSize);
            SpawnBlock(RDUPos, blockScale);

            Vector3 LUUPos = new Vector3(mainPosition.x - quaterSize, mainPosition.y + quaterSize, mainPosition.z + quaterSize);
            SpawnBlock(LUUPos, blockScale);

            Vector3 RUUPos = new Vector3(mainPosition.x + quaterSize, mainPosition.y + quaterSize, mainPosition.z + quaterSize);
            SpawnBlock(RUUPos, blockScale);

            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].SetScale(scale / 2);
                blocks[i].Divide();
            }
        }

        private void SpawnBlock(Vector3 blockPosition,Vector3 scale)
        {
            GameObject newBlock =  Instantiate(block, blockPosition, Quaternion.identity);
            BlockTree blockTree = newBlock.GetComponent<BlockTree>();
            blocks.Add(blockTree);
            newBlock.transform.localScale = scale;
        }
    }
}