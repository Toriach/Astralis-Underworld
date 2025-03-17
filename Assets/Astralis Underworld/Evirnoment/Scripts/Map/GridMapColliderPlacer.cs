using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public class GridMapColliderPlacer : MonoBehaviour
    {
        public float rangeForChunk = 10f;
        public float rangeForCells = 10f;
        public GameObject objectWithCollider;
        private GridMap _map;
        private List<CellAndCollider> _usedColliders;
        private List<CellAndCollider> _freeColliders;
        private List<CellAndCollider> _toRemoveList;

        private int _initCollidersCount = 25;
        private Vector3 _offset;
        private List<GridCell> _cellsInRange;
        private GameObject _collidersParent;
        private void Awake()
        {
            _map = GridMap.instance;
            _freeColliders = new List<CellAndCollider>();
            _usedColliders = new List<CellAndCollider>();
            _toRemoveList = new List<CellAndCollider>();

            _cellsInRange = new List<GridCell>();
            _offset = new Vector3(0.25f, 1, 0.25f);
            _collidersParent = new GameObject("Colliders parent");
            _collidersParent.transform.parent = transform;
            SpawnColliders(_initCollidersCount);
        }

        private void SpawnColliders(int amountToSpawn)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                GameObject go = Instantiate(objectWithCollider);
                CellAndCollider cellAndCollider = new CellAndCollider();
                cellAndCollider.ColliderGO = go;
                cellAndCollider.CellCollider = go.GetComponent<CellCollider>();

                _freeColliders.Add(cellAndCollider);
                go.transform.parent = _collidersParent.transform;
            }
        }

        int skipped = 0;
        private void Update()
        {
            skipped++;
            if (skipped > 10)
            {
                _cellsInRange.Clear();
                _cellsInRange =  GridUtilityGetInRange.GetGridCellsInRange(PlayerFacade.instance.transform.position, rangeForChunk, rangeForCells);
                _map.GridsNearPlayer = _cellsInRange;

                skipped = 0;
                CheckForOutOfRangeCells();
                UpdateColliders();
            }
        }

        private void CheckForOutOfRangeCells()
        {
            _toRemoveList.Clear();
            for (int i = 0; i < _usedColliders.Count; i++)
            {
                if (_usedColliders[i].Cell.GetDistanceToPlayer() > rangeForCells || _usedColliders[i].Cell.IsEmpty)
                {
                    _usedColliders[i].Cell.HaveAsignedCollider = false;
                    _usedColliders[i].Cell = null;
                    _usedColliders[i].CellCollider.Cell = null;
                    _usedColliders[i].ColliderGO.transform.position = new Vector3(0,100,0);
                    _freeColliders.Add(_usedColliders[i]);
                    _toRemoveList.Add(_usedColliders[i]);
                }
            }

            _usedColliders = _usedColliders.Except<CellAndCollider>(_toRemoveList).ToList();
        }

        private void UpdateColliders()
        {
            for (int i = 0; i < _cellsInRange.Count; i++)
            {
                if (_cellsInRange[i].HaveAsignedCollider) continue;
   
                AssignCollider(_cellsInRange[i]);
            }
        }

        private void AssignCollider(GridCell cell)
        {
            CellAndCollider newPair = GetFreeCollider();

            cell.HaveAsignedCollider = true;

            newPair.Cell = cell;
            newPair.CellCollider.Cell = cell;
            newPair.SetPosition(_offset);

            _usedColliders.Add(newPair);
        }

        private CellAndCollider GetFreeCollider()
        {
            if (_freeColliders.Count <= 0)
            {
                SpawnColliders(1);
            }
            CellAndCollider freeCollider = _freeColliders[0];
            _freeColliders.Remove(freeCollider);
            return freeCollider;
        }
        [System.Serializable]
        private class CellAndCollider
        {
            public GridCell Cell;
            public GameObject ColliderGO;
            public CellCollider CellCollider;

            public void SetPosition(Vector3 offset)
            {
                ColliderGO.transform.position = Cell.CellPosition + offset;
            }
        }
    }
}