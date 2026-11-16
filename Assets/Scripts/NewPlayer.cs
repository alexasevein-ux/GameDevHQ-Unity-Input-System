using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayer : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    private void Update()
    {
        // LEGACY INPUT SYSTEM - ORIGINAL CODE
        /*
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        */

        // NEW INPUT SYSTEM
        Vector2 movement = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                movement.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                movement.x += 1f;

            if (Keyboard.current.wKey.isPressed)
                movement.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                movement.y -= 1f;
        }

        // Prevent faster diagonal movement
        movement = Vector2.ClampMagnitude(movement, 1f);

        // Existing movement behavior
        transform.Translate(movement * _speed * Time.deltaTime);
    }
}