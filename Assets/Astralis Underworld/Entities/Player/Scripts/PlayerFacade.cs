using Assets.Astralis_Underworld.Evirnoment.Scripts;
using Astralis_Underworld.Entities.Player;
using Astralis_Underworld.Entities.Player.Scripts;
using Astralis_Underworld.Entities.Scripts;
using System;

namespace Assets.Astralis_Underworld.Entities.Player.Scripts
{
    public class PlayerFacade : MonoSingleton<PlayerFacade>
    {
        public BlockDetector DetectorForFogRemover;

        public event Action OnInitDone;
        public PlayerAnimatorController AnimatorController { get; private set; }
        public AnimationListener AnimationListener { get; private set; }
        public ChunkDetector ChunkDetector { get; private set; }

        public HitTrigger HitTrigger { get; private set; }

        public PlayerMining playerMining { get; private set; }

        private void Awake()
        {
            AnimatorController = GetComponent<PlayerAnimatorController>();

            AnimationListener = GetComponentInChildren<AnimationListener>();
            ChunkDetector = GetComponentInChildren<ChunkDetector>();
            HitTrigger = GetComponentInChildren<HitTrigger>();
            playerMining = GetComponent<PlayerMining>();

            OnInitDone?.Invoke();
        }
    }
}