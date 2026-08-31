using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float movementSpeed;
    public float jumpForce;
    public GameObject atkHitbox;

    public GameObject box;
    Rigidbody2D rigidbody;
    Collider2D collider;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        moveAction = InputSystem.actions.FindAction("MoveX");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");

    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.linearVelocityX = moveAction.ReadValue<float>() * movementSpeed;

        if (jumpAction.WasPressedThisFrame())
        {
            rigidbody.AddForceY(jumpForce);
        }

        if (attackAction.WasPressedThisFrame() && rigidbody)
        {
            GameObject attackHitbox;
            Vector3 offset = Vector3.right * 2;
            attackHitbox = Instantiate(atkHitbox, transform.position + offset,transform.rotation);
            Collider2D boxArea = box.GetComponent<Collider2D>();
            Collider2D attackCollider = attackHitbox.GetComponent<Collider2D>();
            Debug.Log(attackCollider.IsTouching(boxArea));

        }

    }
}
