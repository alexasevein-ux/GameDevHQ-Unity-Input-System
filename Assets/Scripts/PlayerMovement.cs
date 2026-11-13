using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _characterController;
    private PlayerInputActions1 _input;
    private Vector2 _movementInput;
    private float _verticalVelocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (_characterController == null)
        {
            Debug.LogError("Character Controller is missing!");
            enabled = false;
            return;
        }

        _input = new PlayerInputActions1();

        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (_input == null)
            return;

        _input.Player.Enable();

        _input.Player.Movement.performed += OnMovementPerformed;
        _input.Player.Movement.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        if (_input == null)
            return;

        _input.Player.Movement.performed -= OnMovementPerformed;
        _input.Player.Movement.canceled -= OnMovementCanceled;

        _input.Player.Disable();
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.Dispose();
        }
    }

    private void Update()
    {
        Vector3 forward = _cameraTransform != null
            ? _cameraTransform.forward
            : Vector3.forward;

        Vector3 right = _cameraTransform != null
            ? _cameraTransform.right
            : Vector3.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (right * _movementInput.x) + (forward * _movementInput.y);

        movement = Vector3.ClampMagnitude(movement, 1f);

        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 finalMovement = movement * _speed + Vector3.up * _verticalVelocity;

        _characterController.Move(finalMovement * Time.deltaTime);

        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _movementInput = Vector2.zero;
    }
}