using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class C4 : MonoBehaviour
    {
        [SerializeField]
        private GameObject _explosionPrefab;

        private Collider[] _hits = new Collider[5];

        public void Explode()
        {
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("C4 explosion prefab is missing!");
            }

            int count = Physics.OverlapSphereNonAlloc(transform.position, 1f, _hits);

            for (int i = 0; i < count; i++)
            {
                Collider hit = _hits[i];

                if (hit != null &&
                    hit.TryGetComponent<IDestroyable>(out IDestroyable destroyable))
                {
                    destroyable.DestroyAction();
                }

                _hits[i] = null;
            }

            Destroy(gameObject);
        }

        public void Place(Transform target)
        {
            if (target == null)
            {
                Debug.LogError("C4 placement target is missing!");
                return;
            }

            transform.SetPositionAndRotation(target.position, target.rotation);

            transform.SetParent(null);
        }
    }
}