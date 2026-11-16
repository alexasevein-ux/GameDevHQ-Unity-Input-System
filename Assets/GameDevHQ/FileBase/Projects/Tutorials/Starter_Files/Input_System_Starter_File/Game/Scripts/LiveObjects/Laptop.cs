using Cinemachine;
using Game.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Scripts.LiveObjects
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private int _hackTime = 5;

        private bool _hacked = false;

        [SerializeField] private CinemachineVirtualCamera[] _cameras;

        private int _activeCamera = 0;

        [SerializeField] private InteractableZone _interactableZone;

        public static event Action onHackComplete;
        public static event Action onHackEnded;

        private void OnEnable()
        {
            InteractableZone.onHoldStarted += InteractableZone_onHoldStarted;
            InteractableZone.onHoldEnded += InteractableZone_onHoldEnded;
        }

        private void Update()
        {
            if (!_hacked || Keyboard.current == null)
                return;

            // Press E to switch virtual cameras.
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (_cameras == null || _cameras.Length == 0)
                    return;

                int previous = _activeCamera;

                _activeCamera++;

                if (_activeCamera >= _cameras.Length)
                    _activeCamera = 0;

                _cameras[_activeCamera].Priority = 11;
                _cameras[previous].Priority = 9;
            }

            // Press Escape to exit camera control.
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _hacked = false;

                UIManager.Instance.DisplayCameraControlMessage(false);

                onHackEnded?.Invoke();

                ResetCameras();
            }
        }

        private void ResetCameras()
        {
            foreach (var cam in _cameras)
            {
                cam.Priority = 9;
            }
        }

        private void InteractableZone_onHoldStarted(int zoneID)
        {
            if (zoneID == 3 && _hacked == false)
            {
                _progressBar.gameObject.SetActive(true);
                StartCoroutine(HackingRoutine());

                onHackComplete?.Invoke();
            }
        }

        private void InteractableZone_onHoldEnded(int zoneID)
        {
            if (zoneID == 3)
            {
                if (_hacked == true)
                    return;

                StopAllCoroutines();

                _progressBar.gameObject.SetActive(false);
                _progressBar.value = 0;

                onHackEnded?.Invoke();
            }
        }

        private IEnumerator HackingRoutine()
        {
            while (_progressBar.value < 1)
            {
                _progressBar.value += Time.deltaTime / _hackTime;
                yield return new WaitForEndOfFrame();
            }

            // Successfully hacked
            _hacked = true;

            _interactableZone.CompleteTask(3);

            // Show camera instructions after successful hacking
            UIManager.Instance.DisplayCameraControlMessage(true);

            // Hide progress bar
            _progressBar.gameObject.SetActive(false);

            // Enable first virtual camera
            _activeCamera = 0;
            _cameras[0].Priority = 11;
        }

        private void OnDisable()
        {
            InteractableZone.onHoldStarted -= InteractableZone_onHoldStarted;
            InteractableZone.onHoldEnded -= InteractableZone_onHoldEnded;
        }
    }
}
