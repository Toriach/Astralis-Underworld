using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Astralis_Underworld.Entities.Player.Scripts
{
    public class GridMiningDetector : MonoBehaviour
    {
        [SerializeField] private int framesToSkip = 10;

        private int frameCount = 0;
        private PlayerFacade _player;
        private void Start()
        {
            _player = PlayerFacade.instance;
        }

        void Update()
        {
            frameCount++;
            if (frameCount >= framesToSkip)
            {
                CheckForWallInFront();
                frameCount = 0;
            }
        }

        private void CheckForWallInFront()
        {
            var playerPosForward = _player.transform.position + _player.transform.forward * GameConstants.GridSize;

            var detectedCell = GridUtilityGetInRange.FinsGridCellByPosition(playerPosForward);

            if (detectedCell == null)
            {
                _player.playerMining.SetDetectedCell(null);
                return;
            }

            if (detectedCell.IsEmpty == false)
            {
                _player.playerMining.SetDetectedCell(detectedCell);
                _player.AnimatorController.PlayMine();
            }
            else
            {
                _player.AnimatorController.StopMine();
            }
        }
    }
}