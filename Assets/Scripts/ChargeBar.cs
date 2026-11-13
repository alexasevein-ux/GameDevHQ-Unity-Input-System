using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ChargeBar : MonoBehaviour
{
    private UIInputActions _input;

    [SerializeField]
    private Slider _slider;

    private bool _isCharging = false;

    void Start()
    {
        // Check whether the Slider has been assigned.
        if (_slider == null)
        {
            Debug.LogError("ChargeBar: Slider reference is missing!", this);
            enabled = false;
            return;
        }

        // Initialize the Input System.
        _input = new UIInputActions();
        _input.UI.Enable();

        _input.UI.ChargeBar.started += ChargeBar_started;
        _input.UI.ChargeBar.canceled += ChargeBar_canceled;
    }

    private void ChargeBar_started(InputAction.CallbackContext obj)
    {
        _isCharging = true;

        StartCoroutine(ChargeBarRoutine());

        Debug.Log("Charging...");
    }

    private void ChargeBar_canceled(InputAction.CallbackContext obj)
    {
        _isCharging = false;

        Debug.Log("Charging Complete");
    }

    IEnumerator ChargeBarRoutine()
    {
        // Increase the charge while the button is held.
        while (_isCharging && _slider.value < _slider.maxValue)
        {
            _slider.value += Time.deltaTime / 4f;
            yield return null;
        }

        // Decrease the charge after releasing the button.
        while (_slider.value > _slider.minValue)
        {
            _slider.value -= Time.deltaTime / 4f;
            yield return null;
        }

        _slider.value = _slider.minValue;
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.UI.ChargeBar.started -= ChargeBar_started;
            _input.UI.ChargeBar.canceled -= ChargeBar_canceled;

            _input.UI.Disable();
            _input.Dispose();
        }
    }
}