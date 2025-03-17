using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astralis_Underworld.Entities.Scripts
{
    public class HitTrigger : MonoBehaviour
    {
        [SerializeField] private string tagToDetect = "Minable";
        public event Action<Collider> OnHit;

        private void Awake()
        {
            enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tagToDetect) == false) return;
            OnHit?.Invoke(other);
        }

        public void Activate()
        {
            StartCoroutine(CycleActivation());
        }

        private IEnumerator CycleActivation()
        {
            enabled = true;

            yield return 0;
            yield return 0;
            yield return 0;

            enabled = false;
        }
    }
}