using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player4 : MonoBehaviour
{
    private PlayerInputActions4 _input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();
        transform.Translate(new Vector3(move.x, 0, 0) * Time.deltaTime * 3.0f);
    }
}
