using Assets.Astralis_Underworld.Evirnoment.Scripts;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astralis_Underworld.Entities.Player.Scripts
{
    public class BlockDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask detectionLayer;
        [SerializeField] private int refreshRate = 5;
        [SerializeField] private float colliderSize = 0.5f;
        [SerializeField] private float forwardOffset = 0f;
        [SerializeField] private bool drawDebugGizmos = false;
        [SerializeField] private Color gizmoColor = Color.yellow;
        public event Action<GridChunk> RockDetected;
        public event Action<GridChunk> DetectionLost;

        public bool IsAnyDetected => _ChunkList.Count > 0;

        private int _framesPassed = 0;
        private int _maxColliders = 5;
        private Collider[] _hitColliders;
        private List<GridChunk> _ChunkList;
        private List<GridChunk> _oldChunkList;

        private Vector3 _sphereCastPos;

        private void Awake()
        {
            _hitColliders = new Collider[_maxColliders];
            _ChunkList = new List<GridChunk>();
            _oldChunkList = new List<GridChunk>();
        }

        private void Update()
        {
            _framesPassed++;
            if (_framesPassed < refreshRate) return;

            _framesPassed = 0;
            RefreshDetection();
        }

        private void RefreshDetection()
        {
            _oldChunkList = new List<GridChunk>(_ChunkList);
            _ChunkList.Clear();

            _sphereCastPos = transform.position + (transform.forward * forwardOffset);
            _sphereCastPos.y += 0.8f;

            int detectedTargets = Physics.OverlapSphereNonAlloc(_sphereCastPos, colliderSize, _hitColliders, detectionLayer);

            for (int i = 0; i < detectedTargets; i++)
            {
                if (_hitColliders[i].TryGetComponent<GridChunk>(out var chunk))
                {
                    //if(chunk.Cell.IsEmpty) continue;
                    _ChunkList.Add(chunk);
                    RockDetected?.Invoke(chunk);
                }
                RockDetected?.Invoke(chunk);
            }

            for (int i = 0; i < _oldChunkList.Count; i++)
            {
                if (_ChunkList.Contains(_oldChunkList[i])) continue;

                DetectionLost?.Invoke(_oldChunkList[i]);
            }
            
            _hitColliders = new Collider[_maxColliders];
        }

        public List<GridChunk> GetDetected() {  return _ChunkList; }

        public GridChunk GetClosest()
        {
            var count = _ChunkList.Count;

            switch (count)
            {
                case 0:
                    return null;
                case 1:
                    return _ChunkList[0];
            }

            GridChunk closest = null;
            var closesDistance = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                var distance = Vector3.Distance(transform.position, _ChunkList[i].transform.position);
                if (distance < closesDistance)
                {
                    closest = _ChunkList[i];
                    closesDistance = distance;
                }
            }

            return closest;
        }

        void OnDrawGizmosSelected()
        {
            if(drawDebugGizmos == false) return;

            _sphereCastPos = transform.position + (transform.forward * forwardOffset);
            _sphereCastPos.y += 0.8f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(_sphereCastPos, colliderSize);
        }
    }
}