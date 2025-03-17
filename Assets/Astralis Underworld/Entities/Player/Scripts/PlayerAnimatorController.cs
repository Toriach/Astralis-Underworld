using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astralis_Underworld.Entities.Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void PlayIdle()
        {
            _animator.Play("Idle",0);
        }

        public void PlayRun()
        {
            _animator.Play("Run",0);
        }

        public void PlayMine()
        {
            _animator.Play("Mine",1);
            _animator.SetBool("IsMinning", true);
        }

        public void StopMine()
        {
            // _animator.Play("StopMine", 1);
            _animator.SetBool("IsMinning", false);
        }
    }
}