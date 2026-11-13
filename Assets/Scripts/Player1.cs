using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player1 : MonoBehaviour
{
    private PlayerInputActions2 _input;
    private MeshRenderer _render;

    private bool _isDriving = false;

    private void Awake()
    {
        _input = new PlayerInputActions2();
        _render = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        if (_input == null)
            return;

        _input.Player.Enable();

        _input.Player.ChangeColor.performed += ChangeColor_performed;
        _input.Player.DrivingState.performed += DrivingState_performed;
    }

    private void OnDisable()
    {
        if (_input == null)
            return;

        _input.Player.ChangeColor.performed -= ChangeColor_performed;
        _input.Player.DrivingState.performed -= DrivingState_performed;

        _input.Player.Disable();
        _input.Driving.Disable();
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.Dispose();
        }
    }

    private void DrivingState_performed(InputAction.CallbackContext context)
    {
        // Switch from Player controls to Driving controls.
        _isDriving = true;

        _input.Player.Disable();
        _input.Driving.Enable();

        Debug.Log("Driving Mode Enabled");
    }

    private void ChangeColor_performed(InputAction.CallbackContext context)
    {
        // Assign a random color to the Player.
        if (_render != null)
        {
            _render.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    private void Update()
    {
        if (!_isDriving)
        {
            // Rotate while using the Player action map.
            float rotateDirection = _input.Player.Rotate.ReadValue<float>();

            transform.Rotate(Vector3.up * rotateDirection * 30f * Time.deltaTime);
        }
        else
        {
            // Move while using the Driving action map.
            Vector2 move = _input.Driving.Movement.ReadValue<Vector2>();

            Vector3 movement = new Vector3(move.x, 0f, move.y);

            transform.Translate(movement * 3.0f * Time.deltaTime, Space.Self);
        }
    }
}