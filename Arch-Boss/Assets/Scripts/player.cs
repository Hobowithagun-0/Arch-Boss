using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerStates {
        Attacking,
        Approaching,
        Escaping
    }
    // Movement
    public float MaxSpacing = 2f;
    public float MinSpacing = 1f;
    public float MovementSpeed = 2f;
    public float TimeToMaxSpeed = 1f;
    public float MaxJumpForce = 5f;
    public float JumpForceMult = 1f;
    public GroundChecker GroundChecker;
    private float rateX;

    // Attack Hitbox
    public GameObject AtkHitbox;
    public GameObject Target;

    // Declare variables
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private PlayerStates playerState = PlayerStates.Approaching;

    // Attack Cooldowns
    public float AttackDuration; // should try to match the animation of the sword swing
    public float AttackCooldown; // should not be 0 else sprite bugs out
    private WaitForSeconds attackDuration;
    private float attackCooldown;

    //Health variables
    [SerializeField] private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        health.OnDeath += Die;

        attackDuration = new WaitForSeconds(AttackDuration);
    }

    private void Update() {
        attackCooldown -= Time.deltaTime;
    }
    private void FixedUpdate() {
        float playerTargetDistanceX = Target.transform.position.x - transform.position.x;
        float playerTargetDistanceY = Target.transform.position.y - transform.position.y;

        // Flip Player Object
        FlipPlayer(playerTargetDistanceX);

        // Player Movement AI
        switch (playerState) {
            case PlayerStates.Approaching:
                if (Mathf.Abs(playerTargetDistanceX) < MinSpacing) { // too close
                    MoveCloser(-playerTargetDistanceX);
                } else if (Mathf.Abs(playerTargetDistanceX) > MaxSpacing) { // too far
                    MoveCloser(playerTargetDistanceX);
                } else { // perfect distance, start jumping/attacking
                    StopMoving();
                    if (playerTargetDistanceY > 1) {
                        if (GroundChecker.IsGrounded) {
                            Jump(playerTargetDistanceY);
                        }
                    } else {
                        playerState = PlayerStates.Attacking;
                    }
                }
                break;
            case PlayerStates.Escaping:
                break;
            case PlayerStates.Attacking:
                if (attackCooldown <= 0f) {
                    attackCooldown = AttackDuration + AttackCooldown;
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private void MoveCloser(float distance) { // distance to target position. negative means to the left
        rigidbody.linearVelocityX = Mathf.SmoothDamp(rigidbody.linearVelocity.x, MovementSpeed * Mathf.Sign(distance),
            ref rateX, TimeToMaxSpeed);
    }

    private void StopMoving() {
        rigidbody.linearVelocityX = Mathf.SmoothDamp(rigidbody.linearVelocity.x, 0f, ref rateX, TimeToMaxSpeed);
    }

    private void Jump(float distance) {
        if (distance <= 0) {
            return;
        }
        rigidbody.linearVelocityY = Mathf.Min(MaxJumpForce, JumpForceMult * distance);
    }

    private void FlipPlayer(float playerTargetDistanceX) {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(playerTargetDistanceX) * Mathf.Abs(scale.x);
        transform.localScale = scale; // if X scale is negative, facing left. Else right
    }

    private IEnumerator Attack() {
        AtkHitbox.SetActive(true);
        yield return attackDuration;
        AtkHitbox.SetActive(false);
        playerState = PlayerStates.Approaching;
    }

    private void OnDestroy()
    {
        health.OnDeath -= Die;
    }

    /// <summary>
    /// A simple die function that gets called when the player's health reaches 0
    /// </summary>
    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
