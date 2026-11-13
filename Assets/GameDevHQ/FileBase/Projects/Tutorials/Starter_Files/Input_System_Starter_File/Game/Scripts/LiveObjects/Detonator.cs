using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Detonator : MonoBehaviour
    {
        [Header("C4 Settings")]
        [SerializeField]
        private C4 _c4Prefab;

        private C4 _c4;
        private bool _c4Placed;

        private MeshRenderer _render;

        [SerializeField]
        private InteractableZone[] _interactableZone;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }

        private void Start()
        {
            _render = GetComponent<MeshRenderer>();

            if (_render == null)
            {
                Debug.LogWarning("Detonator MeshRenderer is missing!");
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            if (_c4Placed || zone == null)
                return;

            if (zone.GetZoneID() == 1)
            {
                if (zone.GetItems() == null ||
                    zone.GetItems().Length == 0 ||
                    zone.GetItems()[0] == null)
                {
                    Debug.LogError("C4 placement target is missing!");
                    return;
                }

                PlaceC4(zone.GetItems()[0].transform);
            }
        }

        private void PlaceC4(Transform target)
        {
            if (_c4Prefab == null)
            {
                Debug.LogError("C4 prefab has not been assigned!");
                return;
            }

            if (_c4 != null)
            {
                Debug.LogWarning("A C4 instance already exists!");
                return;
            }

            _c4 = Instantiate(_c4Prefab);

            _c4.Place(target);
            _c4.gameObject.SetActive(true);

            _c4Placed = true;

            if (_interactableZone != null && _interactableZone.Length > 0 && _interactableZone[0] != null)
            {
                _interactableZone[0].CompleteTask(1);
            }
        }

        public void TriggerExplosion()
        {
            if (!_c4Placed || _c4 == null)
                return;

            _c4Placed = false;

            C4 currentC4 = _c4;
            _c4 = null;

            currentC4.Explode();

            if (_interactableZone != null && _interactableZone.Length > 1 && _interactableZone[1] != null)
            {
                _interactableZone[1].CompleteTask(2);
            }

            Destroy(gameObject);
        }

        public void Show()
        {
            if (_render != null)
            {
                _render.enabled = true;
            }
        }
    }
}