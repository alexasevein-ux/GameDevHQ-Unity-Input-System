using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftInput : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3.0f;

    [SerializeField]
    private float _turnSpeed = 90.0f;

    private PlayerControls _input;

    private void Awake()
    {
        _input = new PlayerControls();
    }

    private void OnEnable()
    {
        _input.Forklift.Enable();
    }

    private void Update()
    {
        Vector2 movement =
            _input.Forklift.Movement.ReadValue<Vector2>();

        // W/S: Forward and backward
        transform.Translate(Vector3.forward * movement.y * _moveSpeed * Time.deltaTime);

        // A/D: Turn left and right
        transform.Rotate(Vector3.up, movement.x * _turnSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        _input.Forklift.Disable();
    }

    private void OnDestroy()
    {
        _input.Dispose();
    }
}