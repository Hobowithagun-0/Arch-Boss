using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FloatyBoss : MonoBehaviour {
    private Rigidbody2D body;
    private InputAction move;
    private InputAction special;
    private float rateX;
    private float rateY;
    public float MoveSpeed = 1f;
    public float Accel = 1f;

    void Start() {
        body = GetComponent<Rigidbody2D>();

        move = InputSystem.actions.FindAction("Move", true);
        special = InputSystem.actions.FindAction("Special", true);
    }

    private void FixedUpdate() {
        Vector2 targetVelo = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
        //body.linearVelocityY = Mathf.MoveTowards(body.linearVelocity.y, MoveSpeed * move.ReadValue<Vector2>().y, Accel);
        //body.linearVelocityX = Mathf.MoveTowards(body.linearVelocity.x, MoveSpeed * move.ReadValue<Vector2>().x, Accel);
        body.linearVelocityY = Mathf.SmoothDamp(body.linearVelocity.y, MoveSpeed * targetVelo.y,
            ref rateY, Accel);
        body.linearVelocityX = Mathf.SmoothDamp(body.linearVelocity.x, MoveSpeed * targetVelo.x,
            ref rateX, Accel);
    }
}
