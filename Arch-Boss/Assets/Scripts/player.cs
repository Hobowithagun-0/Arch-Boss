using System;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerStates {

    }
    // Movement
    public float MovementSpeed = 2;
    public float JumpForce = 5;
    public GroundChecker GroundChecker;

    // Attack Hitbox
    public HitboxCode AtkHitbox;

    public GameObject Target;

    // Declare variables
    private Rigidbody2D rigidbody;
    private Collider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

    }

    // Update is called once per frame
    void Update() {
        float playerTargetDistanceX = Target.transform.position.x - transform.position.x;
        float playerTargetDistanceY = Target.transform.position.y - transform.position.y;

        // Flip Player Object
        FlipPlayer(playerTargetDistanceX);

        // Player Movement AI
        if (Mathf.Abs(playerTargetDistanceX) > 2) {
            MoveCloser(playerTargetDistanceX);
        } else {
            if (playerTargetDistanceY > 3 && GroundChecker.IsGrounded) // currently jumping many times before isGrounded is false
            {
                rigidbody.linearVelocityY = JumpForce;
            }
        }

    }

    private String movingDirection;
    void MoveCloser(float distance) {
        if (distance > 0) {
            rigidbody.linearVelocityX = MovementSpeed;
        } else if (distance < 0) {
            rigidbody.linearVelocityX = -MovementSpeed;
        }

    }

    void FlipPlayer(float playerTargetDistanceX) {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(playerTargetDistanceX) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
