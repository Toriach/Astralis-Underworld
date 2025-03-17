using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Evirnoment.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Astralis_Underworld.Evirnoment
{
    [RequireComponent(typeof(FOWDesolve))]
    public class Block : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rockDestruction;
        [SerializeField] private List<Block> blocks;
        [SerializeField] private float StopAt = 1;
        [SerializeField] private List<Mesh> SmallMeshes;

        public GameObject blockPrfab;
        public List<Block> allBlocks;
        public MeshRenderer BlockRenderer;
        public float DistanceToPlayer;
        public bool _lowestChild = false;
        public Mesh BlockMesh;
        public Collider blockCollider;

        protected FOWDesolve fowDesolver;
        protected bool _fowRemoved = false;
        protected float scale = 2;

        private bool _alreadyDivided = false;
        private int _blocksInARow = 0;
        private float _smallBlockSize;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            fowDesolver = GetComponent<FOWDesolve>();
            _meshFilter = GetComponent<MeshFilter>();
            blockCollider = GetComponent<Collider>();
            BlockRenderer = GetComponent<MeshRenderer>();

            blocks = new List<Block>();
            allBlocks = new List<Block>();

            BlockMesh = _meshFilter.mesh;
        }

        public void SetScale(float newScale) { scale = newScale; }

        public void SetColor(Color color)
        {
           // BlockRenderer.material.color = color;
        }

        public void Divide(List<Block> lowestChilds, float minBlockSize)
        {
            BlockRenderer.enabled = false;
            StopAt = minBlockSize;
            Vector3 mainPosition = transform.position;

            Vector3 blockScale = new Vector3(scale, scale, scale) / 2;

            float quaterSize = scale / 4;

            if (scale <= StopAt)
            {
                BlockRenderer.enabled = true;
                _lowestChild = true;
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
                blocks[i].Divide(lowestChilds,minBlockSize);
            }
            DisableStufff();
        }

        private void SpawnBlock(Vector3 blockPosition, Vector3 scale)
        {
            GameObject newBlock = Instantiate(blockPrfab, blockPosition, Quaternion.identity);
            Block blockTree = newBlock.GetComponent<Block>();
            blocks.Add(blockTree);
            newBlock.transform.localScale = scale;
        }

        private void DisableStufff()
        {
            blockCollider.enabled = false;
            BlockRenderer.enabled = false;
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

        public void CalculateDistance()
        {
            Vector3 playerPos = PlayerFacade.instance.transform.position + Vector3.up * 2;
            DistanceToPlayer = Vector3.Distance(playerPos, this.transform.position);
        }

        public void ReplaceMesh()
        {
/*            //_meshFilter.mesh = SmallMeshes[UnityEngine.Random.Range(0,SmallMeshes.Count)];
            _meshFilter.mesh = SmallMeshes[1];
            Vector3 directionToPlayer = GetDirectionToPlayer();
            float angle = 0;

            if (directionToPlayer.x <= -0.5f)
            {
                angle = -90;
            }
            if (directionToPlayer.x >= 0.5f)
            {
                angle = 90;
            }

            transform.Rotate(180, angle, 0, Space.Self);*/
        }

        private Vector3 GetDirectionToPlayer()
        {
            return (transform.position - PlayerFacade.instance.transform.position).normalized;
        }

        protected virtual void DestroyRock()
        {
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