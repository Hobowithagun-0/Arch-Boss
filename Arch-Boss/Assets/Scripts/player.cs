using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerStates {
        Attacking,
        Approaching,
        Escaping
    }
    // Movement
    public float TargetSpacing = 2f;
    public float MaxTargetSpacingDeviation = 0.5f;
    public float MovementSpeed = 2f;
    /// <summary> How smoothly the player will approach the target, larger = faster acceleration </summary>
    public float ApproachDamping = 1f;
    /// <summary> How far the player tries to go away from the target when escaping </summary>
    public float EscapeTargetDistance = 100f;
    public float MaxJumpForce = 5f;
    public GroundChecker GroundChecker;
    private float rateX;

    // Attack Hitbox
    public GameObject AtkHitbox;
    public GameObject Target;

    // Declare variables
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    public CompositeCollider2D SafeZone;
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

        if (!SafeZone.OverlapPoint(transform.position)) {
            var hit = Physics2D.Raycast(transform.position,
                Vector2.right,
                Mathf.Infinity,
                LayerMask.GetMask("Safe Zone"));
            float dist = hit.distance;
            var hit2 = Physics2D.Raycast(transform.position,
                Vector2.left,
                Mathf.Infinity,
                LayerMask.GetMask("Safe Zone"));
            float dist2 = hit2.distance;
            Debug.Log($"left edge is {dist2} away, right edge is {dist} away");
        }

        float playerTargetDistanceX = Target.transform.position.x - transform.position.x;
        float playerTargetDistanceY = Target.transform.position.y - transform.position.y;

        // Flip Player Object
        FlipPlayer(playerTargetDistanceX);

        // Player Movement AI
        switch (playerState) {
            case PlayerStates.Approaching:
                if (Mathf.Abs(playerTargetDistanceX) > TargetSpacing + MaxTargetSpacingDeviation) { // too far
                    MoveCloser(playerTargetDistanceX, TargetSpacing);
                } else {
                    if (Mathf.Abs(playerTargetDistanceX) < TargetSpacing - MaxTargetSpacingDeviation) { // too close
                        MoveAway(playerTargetDistanceX);
                    }
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
                playerState = PlayerStates.Approaching;
                if (attackCooldown <= 0f) {
                    attackCooldown = AttackDuration + AttackCooldown;
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private void MoveCloser(float currentDistance, float targetDistance) { // if currentDistance < 0 it means target is left
        float newX = Mathf.SmoothDamp(rigidbody.position.x, // current position
                rigidbody.position.x + currentDistance - Mathf.Sign(currentDistance) * targetDistance, // target position
                ref rateX, ApproachDamping, // time to accelerate to max speed (not really but close enough)
                MovementSpeed // max speed
                );
        rigidbody.linearVelocityX = (newX - rigidbody.position.x) / Time.fixedDeltaTime;
    }

    private void MoveAway(float currentDistance) { // if currentDistance < 0 it means target is left
        float newX = Mathf.SmoothDamp(rigidbody.position.x, // current position
            rigidbody.position.x - Mathf.Sign(currentDistance) * EscapeTargetDistance, // arbitrary large distance away
            ref rateX, ApproachDamping, // time to accelerate to max speed (not really but close enough)
            MovementSpeed // max speed
            );
        rigidbody.linearVelocityX = (newX - rigidbody.position.x) / Time.fixedDeltaTime;
    }

    private void Jump(float distance) {
        if (distance <= 0) {
            return;
        }
        rigidbody.linearVelocityY = Mathf.Min(MaxJumpForce,
            Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y * rigidbody.gravityScale) * distance));
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
    }

    private void OnDestroy() {
        health.OnDeath -= Die;
    }

    /// <summary>
    /// A simple die function that gets called when the player's health reaches 0
    /// </summary>
    private void Die() {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

}
