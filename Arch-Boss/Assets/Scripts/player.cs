using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float movementSpeed;
    Rigidbody2D rigidbody;
    Collider2D collider;
    InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        moveAction = InputSystem.actions.FindAction("MoveX");

    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.linearVelocityX = moveAction.ReadValue<float>() * movementSpeed;

    }
}
