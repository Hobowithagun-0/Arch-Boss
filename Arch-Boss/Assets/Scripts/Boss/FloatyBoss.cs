using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ProjectilePool))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FloatyBoss : BossBehaviour {
    private readonly float[] directions = { -1f, 1f };
    private Rigidbody2D body;
    private InputAction move;
    private InputAction attack;
    private InputAction special;
    private ProjectilePool projPool;
    private float rateX;
    private float rateY;
    public float MaxSpeed = 1f;
    public float TimeToMaxSpeed = 1f;
    public float ArrowSpeed = 1f;
    public float ArrowSpread = 1f;
    public float DashSpeed = 1f;
    /// <summary> the object that contains the dangerzone for player to avoid </summary>
    public DangerZone DangerZone;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        projPool = GetComponent<ProjectilePool>();

        move = InputSystem.actions.FindAction("Move", true);
        attack = InputSystem.actions.FindAction("Attack", true);
        special = InputSystem.actions.FindAction("Special", true);
    }

    private void Update() {
        if (special.WasPressedThisFrame() && SpecialDelta >= SpecialCooldown) {
            Dash();
            SpecialDelta = 0f;
        } else {
            SpecialDelta += Time.deltaTime;
        }
        if (attack.WasPressedThisFrame() && AttackDelta >= AttackCooldown) {
            ShootArrows();
            AttackDelta = 0f;
        } else {
            AttackDelta += Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        Vector2 targetVelo = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
        body.linearVelocityY = Mathf.SmoothDamp(body.linearVelocity.y, MaxSpeed * targetVelo.y,
            ref rateY, TimeToMaxSpeed);
        body.linearVelocityX = Mathf.SmoothDamp(body.linearVelocity.x, MaxSpeed * targetVelo.x,
            ref rateX, TimeToMaxSpeed);
    }

    private void Dash() {
        Vector2 direction = Vector2.Normalize(Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue())
            - transform.position);
        body.linearVelocity = direction * DashSpeed;
    }

    private void ShootArrows() {
        Vector2 direction = Vector2.Normalize(Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue())
            - transform.position);
        for (int i = 0; i < 3; i++) { // 3 arrows on each side, 6 total
            foreach (float side in directions) {
                if (i == 0 && side == 1f) {
                    continue; // discard 1 middle arrow so 5 total
                }
                
                GameObject Arrow = projPool.Get(); // assume all values except speed, position, projpool and dangerzone are set
                var projEffects = Arrow.GetComponent<ProjectileEffects>();
                var rb = Arrow.GetComponent<Rigidbody2D>();
                float angle = Mathf.Atan2(direction.y, direction.x);

                Arrow.transform.position = transform.position;
                Arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + ArrowSpread * side * i);

                rb.linearVelocity = Arrow.transform.rotation * Vector3.right * ArrowSpeed;
                
                projEffects.PoolingSystem = projPool;
                projEffects.DangerZoneSystem = DangerZone;
                projEffects.CreateDangerZone();
            }
        }
    }
}
