using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Scripts.LiveObjects;
using Cinemachine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        private CharacterController _controller;
        private Animator _anim;

        [SerializeField]
        private float _speed = 5.0f;

        [SerializeField]
        private InputActionReference _movementAction;

        private bool _playerGrounded;

        [SerializeField]
        private Detonator _detonator;

        private bool _canMove = true;

        [SerializeField]
        private CinemachineVirtualCamera _followCam;

        [SerializeField]
        private GameObject _model;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _anim = GetComponentInChildren<Animator>();

            if (_controller == null)
                Debug.LogError("Player Character Controller is missing!");

            if (_anim == null)
                Debug.LogError("Player Animator is missing!");

            if (_movementAction == null || _movementAction.action == null)
                Debug.LogError("Player Movement Action has not been assigned!");
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete += ReleasePlayerControl;
            Laptop.onHackEnded += ReturnPlayerControl;
            Forklift.onDriveModeEntered += ReleasePlayerControl;
            Forklift.onDriveModeExited += ReturnPlayerControl;
            Forklift.onDriveModeEntered += HidePlayer;
            Drone.OnEnterFlightMode += ReleasePlayerControl;
            Drone.onExitFlightmode += ReturnPlayerControl;

            if (_movementAction != null && _movementAction.action != null)
                _movementAction.action.Enable();
        }

        private void Update()
        {
            if (_canMove)
                CalcutateMovement();
        }

        private void CalcutateMovement()
        {
            if (_controller == null || _anim == null)
                return;

            if (_movementAction == null || _movementAction.action == null)
                return;

            _playerGrounded = _controller.isGrounded;

            Vector2 movementInput =
                _movementAction.action.ReadValue<Vector2>();

            float h = movementInput.x;
            float v = movementInput.y;

            transform.Rotate(transform.up, h);

            Vector3 direction = transform.forward * v;
            Vector3 velocity = direction * _speed;

            _anim.SetFloat("Speed", Mathf.Abs(velocity.magnitude));

            if (_playerGrounded)
                velocity.y = 0f;

            if (!_playerGrounded)
                velocity.y += -20f * Time.deltaTime;

            _controller.Move(velocity * Time.deltaTime);
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            if (zone == null)
                return;

            int zoneID = zone.GetZoneID();

            // Laptop hacking uses Zone ID 3.
            // It does not require the Detonator.
            if (zoneID != 1 && zoneID != 2)
                return;

            if (_detonator == null)
            {
                Debug.LogError("Detonator is missing on Player instance: " + name + " | Instance ID: " + GetInstanceID());
                return;
            }

            switch (zoneID)
            {
                case 1:
                    _detonator.Show();
                    break;

                case 2:
                    TriggerExplosive();
                    break;
            }
        }

        private void ReleasePlayerControl()
        {
            _canMove = false;

            if (_followCam != null)
                _followCam.Priority = 9;
        }

        private void ReturnPlayerControl()
        {
            if (_model != null)
                _model.SetActive(true);

            _canMove = true;

            if (_followCam != null)
                _followCam.Priority = 10;
        }

        private void HidePlayer()
        {
            if (_model != null)
                _model.SetActive(false);
        }

        private void TriggerExplosive()
        {
            if (_detonator != null)
                _detonator.TriggerExplosion();
            else
                Debug.LogError("Player Detonator reference is missing!");
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete -= ReleasePlayerControl;
            Laptop.onHackEnded -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= ReleasePlayerControl;
            Forklift.onDriveModeExited -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= HidePlayer;
            Drone.OnEnterFlightMode -= ReleasePlayerControl;
            Drone.onExitFlightmode -= ReturnPlayerControl;

            if (_movementAction != null && _movementAction.action != null)
                _movementAction.action.Disable();
        }
    }
}