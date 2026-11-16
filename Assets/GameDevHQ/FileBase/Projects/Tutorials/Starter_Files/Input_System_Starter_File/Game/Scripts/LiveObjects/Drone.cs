using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _speed = 5f;

        private bool _inFlightMode = false;

        [SerializeField]
        private float _rotationSpeed = 150f;

        [SerializeField]
        private Animator _propAnim;

        [SerializeField]
        private CinemachineVirtualCamera _droneCam;

        [SerializeField]
        private InteractableZone _interactableZone;

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (zone == null)
                return;

            if (!_inFlightMode && zone.GetZoneID() == 4)
            {
                if (_propAnim != null)
                    _propAnim.SetTrigger("StartProps");

                if (_droneCam != null)
                    _droneCam.Priority = 11;

                _inFlightMode = true;

                OnEnterFlightMode?.Invoke();

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.DroneView(true);
                    UIManager.Instance.DisplayDroneControlMessage(true);
                }

                if (_interactableZone != null)
                    _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {
            if (_droneCam != null)
                _droneCam.Priority = 9;

            _inFlightMode = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.DroneView(false);
                UIManager.Instance.DisplayDroneControlMessage(false);
            }
        }

        private void Update()
        {
            if (!_inFlightMode || Keyboard.current == null)
                return;

            CalculateTilt();
            CalculateMovementUpdate();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _inFlightMode = false;

                onExitFlightmode?.Invoke();

                ExitFlightMode();
            }
        }

        private void FixedUpdate()
        {
            if (_rigidbody == null)
                return;

            _rigidbody.AddForce(transform.up * 9.81f, ForceMode.Acceleration);

            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.qKey.isPressed)
            {
                transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
            }

            if (Keyboard.current.eKey.isPressed)
            {
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            }
        }

        private void CalculateMovementFixedUpdate()
        {
            if (Keyboard.current == null || _rigidbody == null)
                return;

            if (Keyboard.current.spaceKey.isPressed)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }

            if (Keyboard.current.vKey.isPressed)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        private void CalculateTilt()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.aKey.isPressed)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 30);

            else if (Keyboard.current.dKey.isPressed)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);

            else if (Keyboard.current.wKey.isPressed)
                transform.rotation = Quaternion.Euler(
                    30, transform.localRotation.eulerAngles.y, 0
                );

            else if (Keyboard.current.sKey.isPressed)
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);

            else
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}