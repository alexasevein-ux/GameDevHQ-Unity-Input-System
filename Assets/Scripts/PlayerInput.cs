using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    //1. Get a reference and start an instance of our input actions
    //2. Enable Input Action Map (dog)
    //3. Register perform functions
    private PlayerInputActions _input;

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Dog.Enable();
        _input.Dog.Bark.performed += Bark_performed;
        _input.Dog.Walk.performed += Walk_performed;
        _input.Dog.Run.performed += Run_performed;
        _input.Dog.Run.canceled += Run_canceled;
        _input.Dog.Die.performed += Die_performed;
        _input.Dog.Die.performed += MyCustomMethodAsWell;
    }

    private void MyCustomMethodAsWell(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger Death: " + context);
    }

    private void Die_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger Death: " + context);
    }

    private void Run_canceled(InputAction.CallbackContext context)
    {
        Debug.Log("Running done: " + context);
    }

    private void Run_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Running: " + context);
    }

    private void Walk_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Walking: " + context);
    }

    private void Bark_canceled(InputAction.CallbackContext context)
    {
        Debug.Log("Done Barking: " + context);
    }

    private void Bark_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Barking: " + context);
    }
}
