using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInputActions1 _input;

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions1();

        _input.Player.Enable();

    }



    // Update is called once per frame
    void Update()
    {
        //poll or check the input readings
        CalculateMovement();


    }


    void CalculateMovement()
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();

        transform.Translate(new Vector3(move.x, 0, move.y) * Time.deltaTime * 3.0f);
        //transform.Translate(move * Time.deltaTime * 3.0f);
    }
}
