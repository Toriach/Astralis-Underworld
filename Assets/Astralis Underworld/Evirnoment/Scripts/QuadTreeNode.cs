using Astralis_Underworld.Evirnoment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class QuadTreeNode : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        public Renderer Renderer;
        public float distance;
        public  float detectionDistance;

        private float size;
        private float _quaterSize;
        private Vector3 _blockScale;
        private List<QuadTreeNode> _myNodes;
        private Action<QuadTreeNode> _destroyed;
        private FOWDesolve _desolve;
        private void Awake()
        {
            _myNodes = new List<QuadTreeNode>();
            Renderer = GetComponent<Renderer>();
            _desolve = GetComponent<FOWDesolve>();
            _desolve.SetRenderer(Renderer);
            Renderer.enabled = true;
        }
        public void Divide(float smallestSize, List<QuadTreeNode> _nodes)
        {
            size = transform.localScale.x;
            _quaterSize = size / 4;
            float childScale = transform.localScale.x / 2;
            if (childScale < smallestSize)
            {
                Renderer.enabled = true;
                _nodes.Add(this);
                return;
            }

            Renderer .enabled = false;

            _blockScale = new Vector3(childScale, transform.localScale.y, childScale);

            SpawnSmallerBlocks();

            for (int i = 0; i < _myNodes.Count; i++)
            {
                _myNodes[i].Divide(smallestSize, _nodes);
                _myNodes[i].transform.parent = transform;
            }
        }
        
        public void CheckIfColliding(float colliderSize,Vector3 detectorPos, Action<QuadTreeNode> destroyed)
        {
            _destroyed = destroyed;
            colliderSize += 1.5f;
             detectionDistance = colliderSize - size /2;
             distance = Vector3.Distance(transform.position, detectorPos);
            if (distance <= detectionDistance)
            {
                _destroyed?.Invoke(this);
                _desolve.Desolve();
                //Destroy(gameObject);
            }
        }

        private void SpawnSmallerBlocks() 
        {
            Vector3 mainPosition = transform.position;
            Vector3 LDDPos = new Vector3(mainPosition.x - _quaterSize, mainPosition.y , mainPosition.z - _quaterSize);
            SpawnBlock(LDDPos, _blockScale);                                          
                                                                                      
            Vector3 RDDPos = new Vector3(mainPosition.x + _quaterSize, mainPosition.y , mainPosition.z - _quaterSize);
            SpawnBlock(RDDPos, _blockScale);                                          
                                                                                      
            Vector3 LDUPos = new Vector3(mainPosition.x - _quaterSize, mainPosition.y , mainPosition.z + _quaterSize);
            SpawnBlock(LDUPos, _blockScale);                                          
                                                                                      
            Vector3 RDUPos = new Vector3(mainPosition.x + _quaterSize, mainPosition.y , mainPosition.z + _quaterSize);
            SpawnBlock(RDUPos, _blockScale);
        }

        private void SpawnBlock(Vector3 blockPosition, Vector3 scale)
        {
            GameObject newBlock = Instantiate(prefab, blockPosition, Quaternion.identity);
            QuadTreeNode node = newBlock.GetComponent<QuadTreeNode>();
            newBlock.transform.localScale = scale;
            _myNodes.Add(node);
          //  blockTree.SetColor(_blockColor);
        }
    }
}