using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using Assets.Astralis_Underworld.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Entities.Player.Scripts
{
    public class PlayerMining : MonoBehaviour
    {
        public int miningPower = 3;
        public float miningRadius = 3;

        public List<GridCell> cellsInMiningRange;

        [SerializeField] private Transform hitPoint;
        private PlayerFacade _player;
        private List<GridChunk> _chunks;
        private Vector3 _hitPointPos;
        private List<GridCell> _cellsInDistance;
        private void Awake()
        {
            _player = PlayerFacade.instance;
            _player.OnInitDone += Init;

            _cellsInDistance = new List<GridCell>();
            cellsInMiningRange = new List<GridCell>();
        }

        private void Init()
        {
            _player.OnInitDone -= Init;

            _player.AnimationListener.OnHit += OnAnimationHit;

            _player.ChunkDetector.ChunkDetected += StartMining;
            _player.ChunkDetector.ChunkDetected += StopMining;

            _chunks = new List<GridChunk>();
        }
        private void StartMining()
        {
            _player.AnimatorController.PlayMine();
        }

        private void StopMining()
        {
            _player.AnimatorController.StopMine();
        }


        private void OnAnimationHit()
        {
            _chunks.Clear();
            cellsInMiningRange.Clear();

            List<GridChunk> chunks = _player.ChunkDetector.GetDetected();
            cellsInMiningRange = GetCellsInMiningDistance(chunks, miningRadius);

            if (cellsInMiningRange.Count == 0) return;

            foreach (var cell in cellsInMiningRange)
            {
                cell.DestroyBlocksFromTop(miningPower, _hitPointPos);
            }

            foreach (var chunk in chunks)
            {
                chunk.GenerateMesh();
            }
        }
        public List<GridCell> GetCellsInMiningDistance(List<GridChunk> chunks, float distance)
        {
            _cellsInDistance.Clear();
            _hitPointPos = hitPoint.position;

            if (chunks == null) return null;

            foreach (GridChunk chunk in chunks)
            {
                List<GridCell> Cells = chunk.Cells;
                foreach (GridCell cell in Cells)
                {
                    if(cell.IsEmpty) continue;
                    float cellDistance = Vector3.Distance(cell.CellPosition, _hitPointPos);

                    if (cellDistance <= distance)
                    {
                        _cellsInDistance.Add(cell);
                    }
                }
            }

            return _cellsInDistance;
        }

        private void OnDestroy()
        {
            _player.OnInitDone -= Init;

            _player.AnimationListener.OnHit -= OnAnimationHit;
            _player.ChunkDetector.ChunkDetected -= StartMining;
            _player.ChunkDetector.ChunkDetected -= StopMining;
        }
    }
}