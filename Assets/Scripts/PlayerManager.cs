using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Player5 _player;
    private GameInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        InitializeInputs();
    }

    private void Update()
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();
        _player.Move(move);
    }

    void InitializeInputs()
    {
        _input = new GameInputs();
        _input.Player.Enable();

        _input.Player.Fire.performed += Fire_performed;
    }

    private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _player.Fire();
    }
}
