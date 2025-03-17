using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Astralis_Underworld.Evirnoment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    [RequireComponent(typeof(RocksDestruction))]
    public class MainBlockRoot : MonoBehaviour
    {
        public GameObject blockPrfab;
        [SerializeField] private ParticleSystem rockDestruction;

        [SerializeField] private List<Block> blocks;
        [SerializeField] private List<Block> allBlocks;
        [SerializeField] private float minBlockSize = 1;

        public MeshRenderer BlockRenderer;

        protected Collider blockCollider;
        protected float scale = 2;
        private int _blocksInARow = 0;
        private float _smallBlockSize;
        private RocksDestruction _rocksDestruction;

        private bool _alreadyDivided = false;
        private Color _blockColor;

        private void Awake()
        {
            blockCollider = GetComponent<Collider>();
            BlockRenderer = GetComponent<MeshRenderer>();
            _rocksDestruction = GetComponent<RocksDestruction>();
            blocks = new List<Block>();
            allBlocks = new List<Block>();
        }

        public void SetColor(Color color)
        {
            BlockRenderer.material.color = color;
            //_blockColor = color;
        }
        public void Divide(List<Block> lowestChilds)
        {
            BlockRenderer.enabled = false;

            blocks.Clear();

            SpawnBlocks();

            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].SetScale(scale / 2);
                blocks[i].Divide(lowestChilds, minBlockSize);
            }
            DisableStufff();


            GetAllChildrens(lowestChilds);

            ClearListAndRemoveDontNeededBlicks();

            double rootThreeOfAllBlocks = Math.Pow(allBlocks.Count, 1.0 / 3);
            _blocksInARow = Convert.ToInt32(rootThreeOfAllBlocks);
            _smallBlockSize = scale / _blocksInARow;

            SetParentOfAllSmallBlocks();

            FixTexturesOnChildrens();

           StaticBatchingUtility.Combine(gameObject);
            
        }

        private void SpawnBlocks()
        {
            Vector3 mainPosition = transform.position;

            Vector3 blockScale = new Vector3(scale, scale, scale) / 2;

            float quaterSize = scale / 4;

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
        }

        private void ReArrangeBlocks()
        {
            Vector3 startPos = transform.position;
            float startOffset = _smallBlockSize * _blocksInARow / 2 - _smallBlockSize / 2;
            startPos += new Vector3(-startOffset, -startOffset, -startOffset);

            float newScaleSize = scale / _blocksInARow / 2;
            Vector3 blockTilingScale = new Vector3(newScaleSize, newScaleSize, newScaleSize);

            int x = 0, y = 0, z = 0;
            for (int i = 0; i < allBlocks.Count; i++)
            {
                Vector3 offset = new Vector3(x * _smallBlockSize, y * _smallBlockSize, z * _smallBlockSize);
                allBlocks[i].transform.position = startPos + offset;


                allBlocks[i].BlockRenderer.material.mainTextureScale = blockTilingScale;

                float offsetSize = newScaleSize * x;
                Vector2 txtureOffset = new Vector2(newScaleSize, newScaleSize * x);
                allBlocks[i].BlockRenderer.material.mainTextureOffset = txtureOffset;


                x++;
                if (x >= _blocksInARow)
                {
                    x = 0;
                    z++;
                    if (z >= _blocksInARow)
                    {
                        z = 0;
                        y++;
                    }
                }

            }
        }

        private void SetParentOfAllSmallBlocks()
        {
            for (int i = 0; i < allBlocks.Count; i++)
            {
                allBlocks[i].transform.parent = gameObject.transform;
            }
        }

        private void SpawnBlock(Vector3 blockPosition, Vector3 scale)
        {
            GameObject newBlock = Instantiate(blockPrfab, blockPosition, Quaternion.identity);
            Block blockTree = newBlock.GetComponent<Block>();
            blocks.Add(blockTree);
            blockTree.SetColor(_blockColor);
            newBlock.transform.localScale = scale;
        }

        private void DisableStufff()
        {
            BlockRenderer.enabled = false;
            blockCollider.isTrigger = true;
        }

        public void GetAllChildrens(List<Block> blocksToAdd)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i]._lowestChild == false)
                {
                    blocks[i].GetAllChildrens(blocksToAdd);
                    if (blocks != null) { blocksToAdd.Concat<Block>(blocks[i].allBlocks); }
                }
                else
                {
                    blocksToAdd.Add(blocks[i]);
                }
            }
        }

        private void ClearListAndRemoveDontNeededBlicks()
        {
            if (blocks[0]._lowestChild == false)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    Destroy(blocks[i].gameObject);
                }
                blocks.Clear();
            }
        }


        public virtual void Hit()
        {
             if (_alreadyDivided == false)
             {
                _alreadyDivided = true;
                Divide(allBlocks);
             }
          //  _rocksDestruction.HitRocks(allBlocks);
            if (allBlocks.Count <= 0)
            {
                blockCollider.enabled = false;
                Destroy(gameObject);
            }
        }

        private void FixTexturesOnChildrens()
        {
            float newScaleSize = scale / _blocksInARow / 2;
            Vector3 blockTilingScale = new Vector3(newScaleSize, newScaleSize, newScaleSize);
            int nrInRow = 0;

            for (int i = 0; i < allBlocks.Count; i++)
            {
                allBlocks[i].BlockRenderer.material.mainTextureScale = blockTilingScale;

                float offsetSize = newScaleSize * nrInRow;
                Vector2 offset = new Vector2(offsetSize, newScaleSize * nrInRow);
                allBlocks[i].BlockRenderer.material.mainTextureOffset = offset;
                nrInRow++;
                if (nrInRow > _blocksInARow) nrInRow = 0;
            }
        }

        protected virtual void DestroyRock()
        {
            //  rockDestruction.Play();
            blockCollider.enabled = false;
            StartCoroutine(DeleteRock());
        }

        protected IEnumerator DeleteRock()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}