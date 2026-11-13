using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDroneInputManager : MonoBehaviour
{
    private PlayerControls _input;
    private bool _controllingDrone;

    public Vector2 DroneMovement { get; private set; }
    public bool DroneThrust { get; private set; }

    private void Awake()
    {
        _input = new PlayerControls();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Drone.Disable();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SwitchControls();
        }

        if (_controllingDrone)
        {
            DroneMovement = _input.Drone.Movement.ReadValue<Vector2>();

            DroneThrust = _input.Drone.Thrust.IsPressed();
        }
        else
        {
            DroneMovement = Vector2.zero;
            DroneThrust = false;
        }
    }

    private void SwitchControls()
    {
        _controllingDrone = !_controllingDrone;

        if (_controllingDrone)
        {
            _input.Player.Disable();
            _input.Drone.Enable();
            Debug.Log("Drone Control Active");
        }
        else
        {
            _input.Drone.Disable();
            _input.Player.Enable();
            Debug.Log("Player Control Active");
        }
    }

    private void OnDisable()
    {
        _input.Player.Disable();
        _input.Drone.Disable();
    }

    private void OnDestroy()
    {
        _input.Dispose();
    }
}