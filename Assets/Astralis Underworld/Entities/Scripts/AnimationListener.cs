using System;
using System.Collections;
using UnityEngine;

namespace Astralis_Underworld.Entities.Scripts
{
    public class AnimationListener : MonoBehaviour
    {
        public event Action OnHit;
        public void Hit()
        {
            OnHit?.Invoke();
        }

        
    }
}