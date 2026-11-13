using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class DestructibleCrateInput : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody _crateRigidbody;
    [SerializeField] 
    private float _tapForce = 5f;
    [SerializeField] 
    private float _holdForce = 20f;

    private PlayerControls _input;

    private void Awake()
    {
        _input = new PlayerControls();
    }

    private void OnEnable()
    {
        _input.Crate.CrateInteract.performed += OnCrateInteract;
        _input.Crate.Enable();
    }

    private void OnCrateInteract(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            ApplyForce(_holdForce);
            Debug.Log("Crate held - stronger force!");
        }
        else if (context.interaction is TapInteraction)
        {
            ApplyForce(_tapForce);
            Debug.Log("Crate tapped - weaker force!");
        }
    }

    private void ApplyForce(float force)
    {
        if (_crateRigidbody == null)
        {
            Debug.LogWarning("Crate Rigidbody is missing!");
            return;
        }

        _crateRigidbody.AddForce(Vector3.forward * force, ForceMode.Impulse);
    }

    private void OnDisable()
    {
        _input.Crate.CrateInteract.performed -= OnCrateInteract;
        _input.Crate.Disable();
    }

    private void OnDestroy()
    {
        _input.Dispose();
    }
}