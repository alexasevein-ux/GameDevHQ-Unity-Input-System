using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton

        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError("UI Manager is NULL.");

                return _instance;
            }
        }

        #endregion

        [Header("Existing UI References")]
        [SerializeField] private Text _interactableZone;
        [SerializeField] private Image _inventoryDisplay;
        [SerializeField] private RawImage _droneCamView;

        [Header("Control Instructions")]
        [SerializeField] private TMP_Text _droneControlMessage;
        [SerializeField] private TMP_Text _forkliftControlMessage;
        [SerializeField] private TMP_Text _cameraControlMessage;

        private void Awake()
        {
            _instance = this;

            if (_droneControlMessage != null)
                _droneControlMessage.gameObject.SetActive(false);

            if (_forkliftControlMessage != null)
                _forkliftControlMessage.gameObject.SetActive(false);

            if (_cameraControlMessage != null)
                _cameraControlMessage.gameObject.SetActive(false);
        }

        public void DisplayInteractableZoneMessage(bool showMessage, string message = null)
        {
            if (_interactableZone == null)
            {
                Debug.LogError("Interactable Zone Text has not been assigned to UIManager.");
                return;
            }

            _interactableZone.text = message;
            _interactableZone.gameObject.SetActive(showMessage);
        }

        public void UpdateInventoryDisplay(Sprite icon)
        {
            if (_inventoryDisplay == null)
            {
                Debug.LogError("Inventory Display has not been assigned to UIManager.");
                return;
            }

            _inventoryDisplay.sprite = icon;
        }

        public void DroneView(bool active)
        {
            if (_droneCamView == null)
            {
                Debug.LogError("Drone Camera View has not been assigned to UIManager.");
                return;
            }

            _droneCamView.enabled = active;
        }

        public void DisplayDroneControlMessage(bool showMessage)
        {
            if (_droneControlMessage == null)
            {
                Debug.LogError("Drone Control Message has not been assigned to UIManager.");
                return;
            }

            _droneControlMessage.gameObject.SetActive(showMessage);
        }

        public void DisplayForkliftControlMessage(bool showMessage)
        {
            if (_forkliftControlMessage == null)
            {
                Debug.LogError("Forklift Control Message has not been assigned to UIManager.");
                return;
            }

            _forkliftControlMessage.gameObject.SetActive(showMessage);
        }

        public void DisplayCameraControlMessage(bool showMessage)
        {
            if (_cameraControlMessage == null)
            {
                Debug.LogError("Camera Control Message has not been assigned to UIManager.");
                return;
            }

            _cameraControlMessage.gameObject.SetActive(showMessage);
        }
    }
}