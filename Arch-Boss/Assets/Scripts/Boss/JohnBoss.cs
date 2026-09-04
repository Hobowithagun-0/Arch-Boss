using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JohnBoss : BossBehaviour {

    private Rigidbody2D body;
    public GroundChecker GroundChecker;

    [Header("Movement Stuff")]
    public float MoveSpeed = 5.0f;
    public float JumpVelocity = 4.0f;

    /// <summary>
    /// JUMPING RELATED INPUTS
    /// </summary>
    private bool jumpPressed = false;
    private bool jumpReleased = false;

    /// <summary>
    /// Attack Related INPUTs
    /// </summary>
    private bool attackPressed = false;

    /// <summary>
    /// ATTACK HITBOXES
    /// </summary>
    [Header("Attack HITBOX Stuff")]
    public HitboxCode AttackHitBox;


    /// <summary>
    /// NORMAL ATTACK TIMINGS
    /// </summary>
    [Header("Attack Timings Stuff")]
    public float WindUpTiming = 0.2f;
    public float AttackTiming = 0.6f;
    private float attackTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        body = GetComponent<Rigidbody2D>();
        AttackHitBox.Hide();
    }

    // Update is called once per frame
    void Update() {
        moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();

        if (moveInput.x != 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        if (InputSystem.actions["Jump"].WasPressedThisFrame()) {
            jumpPressed = true;
        }

        if (InputSystem.actions["Attack"].WasPressedThisFrame()) {
            attackPressed = true;
        }
        if (attackPressed) {
            Attack();
        }
    }
    private void FixedUpdate() {
        Move(moveInput);

    }
    public override void Attack() {
        attackTimer += Time.deltaTime;
        if (attackTimer >= WindUpTiming && attackTimer < AttackTiming) {
            AttackHitBox.Show();
        }
        if (attackTimer >= AttackTiming) {
            AttackHitBox.Hide();
            attackTimer = 0;
            attackPressed = false;

        }
    }
    public override void Move(Vector2 moveInput) {
        body.linearVelocityX = moveInput.x * MoveSpeed;
        if (GroundChecker.IsGrounded && jumpPressed) {
            body.linearVelocityY = JumpVelocity;
            jumpPressed = false;
        }
    }
}
