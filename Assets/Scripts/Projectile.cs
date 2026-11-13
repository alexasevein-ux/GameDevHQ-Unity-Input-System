using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        // Move the projectile upward
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // Destroy the projectile when it goes above Y = 15
        if (transform.position.y > 15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameVisible()
    {

    }
}