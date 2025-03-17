using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astralis_Underworld.Scripts
{
    public class FPSLimiter : MonoBehaviour
    {
        public int FPSLimit = 60;
        private void Awake()
        {
            Application.targetFrameRate = FPSLimit;
        }
    }
}