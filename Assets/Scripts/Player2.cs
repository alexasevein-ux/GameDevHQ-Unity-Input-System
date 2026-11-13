using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2 : MonoBehaviour
{
    private PlayerInputActions3 _input;
    private Rigidbody _rb;

    private bool _jumped = false;

    [SerializeField] private float _fullJumpForce = 25f;
    [SerializeField] private float _lightJumpForce = 10f;

    private void Awake()
    {
        _input = new PlayerInputActions3();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.Player.Jump.performed += Jump_performed;
        _input.Player.Jump.canceled += Jump_canceled;

        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Jump.performed -= Jump_performed;
        _input.Player.Jump.canceled -= Jump_canceled;

        _input.Player.Disable();
    }

    private void OnDestroy()
    {
        _input.Dispose();
    }

    private void Jump_performed(InputAction.CallbackContext context)
    {
        if (_rb == null)
            return;

        // Full jump after completing the Hold interaction.
        _jumped = true;

        Debug.Log("Full Jump");

        _rb.AddForce(Vector3.up * _fullJumpForce, ForceMode.Impulse);
    }

    private void Jump_canceled(InputAction.CallbackContext context)
    {
        if (_rb == null)
            return;

        // Do not apply a second jump after a full jump.
        if (_jumped)
        {
            _jumped = false;
            return;
        }

        // Light jump if the button is released early.
        Debug.Log("Light Jump");

        _rb.AddForce(Vector3.up * _lightJumpForce, ForceMode.Impulse);
    }
}