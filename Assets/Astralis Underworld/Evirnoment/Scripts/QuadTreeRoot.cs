using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class QuadTreeRoot : MonoBehaviour
    {
        [SerializeField] private float smallestSize = 0.5f;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private Vector3 yOffset;
        [SerializeField] private bool destroyOnStart = false;
        private QuadTreeNode _rootNode;
        private Renderer _rootNodeRenderer;
        private List<QuadTreeNode> _allNodes;

        private void Start()
        {
            if (destroyOnStart) Destroy(gameObject);
            _allNodes = new List<QuadTreeNode>();
            GameObject go = Instantiate(nodePrefab, transform.position, Quaternion.identity);
            go.transform.parent = transform;
            go.transform.position += yOffset;
            _rootNode = go.GetComponent<QuadTreeNode>();
            _rootNodeRenderer = _rootNode.Renderer;
        }
        bool divided = false;
        public void Divide(float detectionDistance,Vector3 detectorOriginPos)
        {
            if (divided) 
            {
                if(_allNodes.Count == 0) 
                {
                    Destroy(gameObject);
                    return; 
                }
                for (int i = 0; i < _allNodes.Count; i++)
                {
                    _allNodes[i].CheckIfColliding(detectionDistance, detectorOriginPos, RemoveNode);
                }
                return; 
            }

            divided = true;

            _rootNodeRenderer.enabled = false;
            _rootNode.Divide(smallestSize, _allNodes);
            _rootNode.transform.localScale = _rootNode.transform.localScale * 2;

           // StaticBatchingUtility.Combine(gameObject);
        }

        private void RemoveNode(QuadTreeNode nodeToRemove)
        {
            _allNodes.Remove(nodeToRemove);
        }
    }
}