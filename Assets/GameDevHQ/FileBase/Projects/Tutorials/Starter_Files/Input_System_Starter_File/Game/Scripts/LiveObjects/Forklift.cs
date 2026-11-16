using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Forklift : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lift, _steeringWheel, _leftWheel, _rightWheel, _rearWheels;

        [SerializeField]
        private Vector3 _liftLowerLimit, _liftUpperLimit;

        [SerializeField]
        private float _speed = 5f, _liftSpeed = 1f;

        [SerializeField]
        private CinemachineVirtualCamera _forkliftCam;

        [SerializeField]
        private GameObject _driverModel;

        private bool _inDriveMode = false;

        [SerializeField]
        private InteractableZone _interactableZone;

        public static event Action onDriveModeEntered;
        public static event Action onDriveModeExited;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterDriveMode;
        }

        private void EnterDriveMode(InteractableZone zone)
        {
            if (_inDriveMode != true && zone.GetZoneID() == 5)
            {
                _inDriveMode = true;

                _forkliftCam.Priority = 11;

                onDriveModeEntered?.Invoke();

                _driverModel.SetActive(true);

                // Display forklift control instructions
                UIManager.Instance.DisplayForkliftControlMessage(true);

                _interactableZone.CompleteTask(5);
            }
        }

        private void ExitDriveMode()
        {
            _inDriveMode = false;

            _forkliftCam.Priority = 9;

            _driverModel.SetActive(false);

            // Hide forklift control instructions
            UIManager.Instance.DisplayForkliftControlMessage(false);

            onDriveModeExited?.Invoke();
        }

        private void Update()
        {
            if (_inDriveMode == true)
            {
                LiftControls();
                CalculateMovement();

                // LEGACY INPUT SYSTEM - ORIGINAL CODE
                // if (Input.GetKeyDown(KeyCode.Escape))
                //     ExitDriveMode();

                // NEW INPUT SYSTEM
                if (Keyboard.current != null &&
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    ExitDriveMode();
                }
            }
        }

        private void CalculateMovement()
        {
            // LEGACY INPUT SYSTEM - ORIGINAL CODE
            /*
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            */

            // NEW INPUT SYSTEM
            float h = 0f;
            float v = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed)
                    h -= 1f;

                if (Keyboard.current.dKey.isPressed)
                    h += 1f;

                if (Keyboard.current.wKey.isPressed)
                    v += 1f;

                if (Keyboard.current.sKey.isPressed)
                    v -= 1f;
            }

            var direction = new Vector3(0, 0, v);
            var velocity = direction * _speed;

            transform.Translate(velocity * Time.deltaTime);

            if (Mathf.Abs(v) > 0)
            {
                var tempRot = transform.rotation.eulerAngles;
                tempRot.y += h * _speed / 2;
                transform.rotation = Quaternion.Euler(tempRot);
            }
        }

        private void LiftControls()
        {
            // LEGACY INPUT SYSTEM - ORIGINAL CODE
            /*
            if (Input.GetKey(KeyCode.F))
                LiftUpRoutine();
            else if (Input.GetKey(KeyCode.R))
                LiftDownRoutine();
            */

            // NEW INPUT SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.fKey.isPressed)
                    LiftUpRoutine();
                else if (Keyboard.current.rKey.isPressed)
                    LiftDownRoutine();
            }
        }

        private void LiftUpRoutine()
        {
            if (_lift.transform.localPosition.y < _liftUpperLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;

                tempPos.y += Time.deltaTime * _liftSpeed;

                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else
            {
                _lift.transform.localPosition = _liftUpperLimit;
            }
        }

        private void LiftDownRoutine()
        {
            if (_lift.transform.localPosition.y > _liftLowerLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;

                tempPos.y -= Time.deltaTime * _liftSpeed;

                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else
            {
                _lift.transform.localPosition = _liftLowerLimit;
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterDriveMode;

            if (_inDriveMode)
            {
                UIManager.Instance.DisplayForkliftControlMessage(false);
                _inDriveMode = false;
            }
        }
    }
}