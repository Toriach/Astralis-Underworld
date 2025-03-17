using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Astralis_Underworld.Entities.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private FloatingJoystick _joystick;

        [SerializeField] private PlayerAnimatorController _animatorController;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotateSpeed;

        private Rigidbody _rigidbody;

        private Vector3 _moveVector;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Move();
        }

        private void FixedUpdate()
        {

            _moveVector = Vector3.zero;
            _moveVector.x = _joystick.Horizontal * _moveSpeed * Time.fixedDeltaTime;
            _moveVector.z = _joystick.Vertical * _moveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + _moveVector);
        }

        private void Move()
        {

            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
            {
                Vector3 direction = Vector3.RotateTowards(transform.forward, _moveVector, _rotateSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(direction);

                _animatorController.PlayRun();
            }

            else if (_joystick.Horizontal == 0 && _joystick.Vertical == 0)
            {
                _animatorController.PlayIdle();
            }
        }
    }
}