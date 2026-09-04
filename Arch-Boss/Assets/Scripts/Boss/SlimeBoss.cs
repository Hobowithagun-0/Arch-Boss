using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ProjectilePool))]
public class SlimeBoss : MonoBehaviour {
    private readonly float[] directions = { -1f, 1f };
    private Rigidbody2D body;
    private InputAction jump;
    private InputAction move;
    private InputAction special;
    private WaitForSeconds slamDelay;
    private ContactFilter2D groundFilter;
    private ProjectilePool projPool;
    private Vector2 previousVelo;
    private bool jumped = false;
    private float jumpChargeTime = 0f;
    private float teleportChargeTime = 0f;
    public float MaxJumpChargeTime = 1f;
    public float ChargeMult = 1f;
    public float JumpHeight = 4f;
    public float FastFallMult = 1f;
    public float MoveSpeed = 1f;
    public float SlamYdamp = 2f;
    public float SlamYmult = 1f;
    public float SlamYmin = 1f;
    public float SlamDelay = 0.1f;
    public float TeleportTime = 1f;
    private void Start() {
        body = GetComponent<Rigidbody2D>();
        projPool = GetComponent<ProjectilePool>();

        jump = InputSystem.actions.FindAction("Jump", true);
        move = InputSystem.actions.FindAction("MoveX", true);
        special = InputSystem.actions.FindAction("Special", true);

        slamDelay = new WaitForSeconds(SlamDelay);

        groundFilter = new ContactFilter2D();

        groundFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        groundFilter.useLayerMask = true;

        groundFilter.SetNormalAngle(45f, 135f);
        groundFilter.useNormalAngle = true;
    }

    private void Update() {
        // jump charger
        if (jump.IsPressed() && body.IsTouching(groundFilter)) {
            jumpChargeTime += Time.deltaTime;
        } else {
            if (jump.WasReleasedThisFrame() && body.IsTouching(groundFilter)) {
                Jump();
            }
            jumpChargeTime = 0f;
        }
        // teleport charger
        if (special.IsPressed()) {
            teleportChargeTime += Time.deltaTime;
        } else {
            if (special.WasReleasedThisFrame() && teleportChargeTime >= TeleportTime) {
                Teleport();
            }
            teleportChargeTime = 0f;
        }
    }

    private void FixedUpdate() {
        // fast fall
        if (body.linearVelocityY < 0f) {
            body.linearVelocityY *= FastFallMult;
        }
        // can only move in air
        if (!body.IsTouching(groundFilter)) {
            body.linearVelocityX = move.ReadValue<float>() * MoveSpeed;
        } else {
            body.linearVelocityX = 0f;        
        }
        // ground slam attack
        if (body.IsTouching(groundFilter) && previousVelo.y < -0.1f && jumped) {
            jumped = false;
            StartCoroutine(Slam(previousVelo.y));
        }
        previousVelo = body.linearVelocity;
    }

    private void Jump() {
        float jumpMult = 1f + ChargeMult * Mathf.Min(1f, jumpChargeTime / MaxJumpChargeTime);
        body.linearVelocityY = JumpHeight * jumpMult;
        jumped = true;
    }
    private IEnumerator Slam(float yVelo) {
        Debug.Log(yVelo);
        yVelo += SlamYmin;
        float offsetMult = 1.1f;
        Vector3 slamOrigin = gameObject.transform.position + Vector3.down;
        while (yVelo < 0f) {
            foreach (float direction in directions) {
            GameObject slamProj = projPool.Get();
            slamProj.transform.position = slamOrigin + Vector3.right * direction * offsetMult;
            slamProj.GetComponent<Rigidbody2D>().linearVelocityY = -yVelo * SlamYmult;
            slamProj.GetComponent<ProjectileEffects>().PoolingSystem = projPool;
            slamProj.GetComponent<ProjectileEffects>().OwnerTag = gameObject.tag;
            }
            offsetMult++;
            yVelo += SlamYdamp;
            yield return slamDelay;
        }
    }
    private void Teleport() {
        Vector3 tpTarget = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) + Vector3.back * -10f;
        if (Physics2D.OverlapBox(tpTarget, transform.localScale, 0f, LayerMask.GetMask("Ground"))) {
            return; // exits if it would tp into the ground
        }
        transform.position = tpTarget;
        body.linearVelocity = Vector2.zero;
    }
}
