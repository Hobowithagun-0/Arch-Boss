using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class player : MonoBehaviour
{

    enum PlayerStates
    {
        
    }
    // Movement
    public float movementSpeed = 2;
    public float jumpForce = 5;
    public GroundChecker GroundChecker;

    // Attack Hitbox
    public HitboxCode atkHitbox;

    public GameObject target;

    // Declare variables
    Rigidbody2D rigidbody;
    Collider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

    } 

    // Update is called once per frame
    void Update()
    {
        float playerTargetDistanceX = target.transform.position.x - transform.position.x;
        float playerTargetDistanceY = target.transform.position.y - transform.position.y;

        // Flip Player Object
        FlipPlayer(playerTargetDistanceX);

        // Player Movement AI
        if (Mathf.Abs(playerTargetDistanceX) > 2)
        {
            MoveCloser(playerTargetDistanceX);
        }
        else
        {
            if (playerTargetDistanceY > 3 && GroundChecker.isGrounded) // currently jumping many times before isGrounded is false
            {
                rigidbody.linearVelocityY = jumpForce;
            }
        }

    }

    private String movingDirection;
    void MoveCloser(float distance)
    {
        if (distance > 0)
        {
            rigidbody.linearVelocityX = movementSpeed;
        }
        else if (distance < 0)
        {
            rigidbody.linearVelocityX = -movementSpeed;
        }
        
    }

    void FlipPlayer(float playerTargetDistanceX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(playerTargetDistanceX) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
