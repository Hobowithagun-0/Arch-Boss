using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerStates {
        Attacking,
        Approaching
    }
    // Movement
    public float MovementSpeed = 2;
    public float JumpForce = 5;
    public GroundChecker GroundChecker;

    // Attack Hitbox
    public GameObject AtkHitbox;
    public GameObject Target;

    // Declare variables
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private PlayerStates playerState = PlayerStates.Approaching;

    // Attack Cooldowns
    //public float AttackCooldown; // i personally think there shouldnt be one so that the guy can keep slashing away
    public float AttackDuration; // should try to match the animation of the sword swing
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
                if (Mathf.Abs(playerTargetDistanceX) > 2) {
                    MoveCloser(playerTargetDistanceX);
                } else if (playerTargetDistanceY > 1) {
                    if (GroundChecker.IsGrounded) {
                        rigidbody.linearVelocityY = JumpForce;
                    }
                } else { 
                    playerState = PlayerStates.Attacking;
                }
                break;
            case PlayerStates.Attacking:
                if (attackCooldown <= 0f) {
                    attackCooldown = AttackDuration;
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private void MoveCloser(float distance) {
        if (distance > 0) {
            rigidbody.linearVelocityX = MovementSpeed;
        } else if (distance < 0) {
            rigidbody.linearVelocityX = -MovementSpeed;
        }
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
