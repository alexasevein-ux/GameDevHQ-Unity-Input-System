using Game.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game.Scripts.LiveObjects
{
    public class InteractableZone : MonoBehaviour
    {
        private enum ZoneType
        {
            Collectable,
            Action,
            HoldAction
        }

        private enum KeyState
        {
            Press,
            PressHold
        }

        [SerializeField]
        private ZoneType _zoneType;

        [SerializeField]
        private int _zoneID;

        [SerializeField]
        private int _requiredID;

        [SerializeField]
        [Tooltip("Press the (---) Key to .....")]
        private string _displayMessage;

        [SerializeField]
        private GameObject[] _zoneItems;

        private bool _inZone = false;
        private bool _itemsCollected = false;
        private bool _actionPerformed = false;

        [SerializeField]
        private Sprite _inventoryIcon;

        [SerializeField]
        private KeyCode _zoneKeyInput;

        [SerializeField]
        private KeyState _keyState;

        [SerializeField]
        private GameObject _marker;

        private bool _inHoldState = false;

        private static int _currentZoneID = 0;

        public static int CurrentZoneID
        {
            get
            {
                return _currentZoneID;
            }
            set
            {
                _currentZoneID = value;
            }
        }

        public static event Action<InteractableZone> onZoneInteractionComplete;
        public static event Action<int> onHoldStarted;
        public static event Action<int> onHoldEnded;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += SetMarker;
        }

        private void Update()
        {
            if (!_inZone)
                return;

            if (!TryGetZoneKey(out KeyControl zoneKey))
                return;

            // Single key press
            if (zoneKey.wasPressedThisFrame && _keyState != KeyState.PressHold)
            {
                switch (_zoneType)
                {
                    case ZoneType.Collectable:

                        if (!_itemsCollected)
                        {
                            CollectItems();
                            _itemsCollected = true;

                            UIManager.Instance.DisplayInteractableZoneMessage(false);
                        }

                        break;

                    case ZoneType.Action:

                        if (!_actionPerformed)
                        {
                            PerformAction();
                            _actionPerformed = true;

                            UIManager.Instance.DisplayInteractableZoneMessage(false);
                        }

                        break;
                }
            }

            // Hold key
            else if (zoneKey.isPressed && _keyState == KeyState.PressHold && !_inHoldState)
            {
                _inHoldState = true;

                switch (_zoneType)
                {
                    case ZoneType.HoldAction:
                        PerformHoldAction();
                        break;
                }
            }

            // Release key
            if (zoneKey.wasReleasedThisFrame && _keyState == KeyState.PressHold)
            {
                _inHoldState = false;
                onHoldEnded?.Invoke(_zoneID);
            }
        }

        private bool TryGetZoneKey(out KeyControl zoneKey)
        {
            zoneKey = null;

            if (Keyboard.current == null)
                return false;

            string keyName = _zoneKeyInput.ToString();

            // Convert legacy KeyCode names to Input System Key names.
            if (keyName.StartsWith("Alpha"))
                keyName = keyName.Replace("Alpha", "Digit");

            else if (keyName.StartsWith("Keypad"))
                keyName = keyName.Replace("Keypad", "Numpad");

            else if (keyName == "Return")
                keyName = "Enter";

            if (Enum.TryParse(keyName, true, out Key key))
            {
                zoneKey = Keyboard.current[key];
                return zoneKey != null;
            }

            Debug.LogError("Unsupported interaction key: " + _zoneKeyInput);
            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _currentZoneID > _requiredID)
            {
                switch (_zoneType)
                {
                    case ZoneType.Collectable:

                        if (!_itemsCollected)
                        {
                            _inZone = true;

                            if (!string.IsNullOrEmpty(_displayMessage))
                            {
                                string message = $"Press the {_zoneKeyInput} key to {_displayMessage}.";

                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                            {
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput} key to collect");
                            }
                        }

                        break;

                    case ZoneType.Action:

                        if (!_actionPerformed)
                        {
                            _inZone = true;

                            if (!string.IsNullOrEmpty(_displayMessage))
                            {
                                string message = $"Press the {_zoneKeyInput} key to {_displayMessage}.";

                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            
                            else
                            {
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput} key to perform action");
                            }
                        }

                        break;

                    case ZoneType.HoldAction:

                        _inZone = true;

                        if (!string.IsNullOrEmpty(_displayMessage))
                        {
                            string message =
                                $"Press the {_zoneKeyInput} key to {_displayMessage}.";

                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                        {
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_zoneKeyInput} key to perform action");
                        }

                        break;
                }
            }
        }

        private void CollectItems()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(false);
            }

            UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            CompleteTask(_zoneID);

            onZoneInteractionComplete?.Invoke(this);
        }

        private void PerformAction()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(true);
            }

            if (_inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            onZoneInteractionComplete?.Invoke(this);
        }

        private void PerformHoldAction()
        {
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            onHoldStarted?.Invoke(_zoneID);
        }

        public GameObject[] GetItems()
        {
            return _zoneItems;
        }

        public int GetZoneID()
        {
            return _zoneID;
        }

        public void CompleteTask(int zoneID)
        {
            if (zoneID == _zoneID)
            {
                _currentZoneID++;
                onZoneInteractionComplete?.Invoke(this);
            }
        }

        public void ResetAction(int zoneID)
        {
            if (zoneID == _zoneID)
                _actionPerformed = false;
        }

        public void SetMarker(InteractableZone zone)
        {
            if (_marker != null)
                _marker.SetActive(_zoneID == _currentZoneID);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _inZone = false;

                if (_inHoldState)
                {
                    _inHoldState = false;
                    onHoldEnded?.Invoke(_zoneID);
                }

                UIManager.Instance.DisplayInteractableZoneMessage(false);
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= SetMarker;

            if (_inHoldState)
            {
                _inHoldState = false;
                onHoldEnded?.Invoke(_zoneID);
            }
        }
    }
}