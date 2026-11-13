using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private GameObject _projectilePrefab;
    private float _canFire = -1f;
    [SerializeField]
    private float _fireDelay = 0.5f;
    private int _ammoCount = 3;
    
    public void Move(Vector2 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
    }

    public void Fire()
    {
        if (Time.time > _canFire && _ammoCount > 0)
        {
            Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            _canFire = Time.time + _fireDelay;
            _ammoCount--; 
        }
    }
}