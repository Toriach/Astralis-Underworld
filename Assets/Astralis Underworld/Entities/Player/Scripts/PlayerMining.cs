using Assets.Astralis_Underworld.Entities.Player.Scripts;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using Assets.Astralis_Underworld.Scripts;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Astralis_Underworld.Entities.Player.Scripts
{
    public class PlayerMining : MonoBehaviour
    {
        public int miningPower = 3;
        private PlayerFacade _player;
        private GridCell _detectedCell;

        private void Awake()
        {
            _player = PlayerFacade.instance;
            _player.OnInitDone += Init;
        }

        private void Init()
        {
            _player.OnInitDone -= Init;

            _player.AnimationListener.OnHit += OnAnimationHit;
        }

        private void OnAnimationHit()
        {
            if (_detectedCell.IsEmpty) return;

            _detectedCell.DestroyBlocksFromTop(miningPower);
            var playerPosForward = _player.transform.position + _player.transform.forward * GameConstants.GridSize;

            var reg = GridUtilityGetInRange.GetRegion(playerPosForward);
            var chunk = GridUtilityGetInRange.FindChunkAt(playerPosForward, reg);
            chunk.ReGenerateMesh();
        }

        public void SetDetectedCell(GridCell detectedCell) { _detectedCell = detectedCell; }

        private void OnDestroy()
        {
            _player.OnInitDone -= Init;

            _player.AnimationListener.OnHit -= OnAnimationHit;
        }
    }
}

/*public float miningDistance = 5f;
public float miningPower = 0.2f;
private PlayerFacade _player;

private void Awake()
{
    _player = PlayerFacade.instance;
    _player.OnInitDone += Init;
}

private void Init()
{
    _player.OnInitDone -= Init;
    _player.RockDetector.RockDetected += DoMining;
    _player.RockDetector.DetectionLost += StopMining;

    _player.AnimationListener.OnHit += OnAnimationHit;
}

private void DoMining(GridChunk chunk)
{
    _player.AnimatorController.PlayMine();
}
private void StopMining(GridChunk chunk)
{
    if (_player.RockDetector.IsAnyDetected) return;
    _player.AnimatorController.StopMine();
}

private void OnAnimationHit()
{
    var detected = _player.RockDetector.GetDetected();
    for (int i = 0; i < detected.Count; i++)
    {
        detected[i].Hit(_player.transform, miningDistance, miningPower);
    }
}

private void OnDestroy()
{
    _player.OnInitDone -= Init;
    _player.RockDetector.RockDetected -= DoMining;
    _player.RockDetector.DetectionLost -= StopMining;

    _player.AnimationListener.OnHit -= OnAnimationHit;
}*/