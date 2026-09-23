using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ProjectilePool))]
public class SlimeBoss : BossBehaviour {
    private readonly float[] directions = { -1f, 1f };
    private Vector3 iniScale;
    private Rigidbody2D body;
    private BoxCollider2D col;
    private InputAction jump;
    private InputAction move;
    private InputAction special;
    private WaitForSeconds slamDelay;
    private ContactFilter2D groundFilter;
    private ProjectilePool projPool;
    public DangerZone DangerZone;
    private int dangerZoneIndex;
    private SortedList<float, float> iniToFinalSpeed = new();
    // iniSpeed will be > 0 if needs to be cached in the above dictionary for dangerzone creation
    private float iniSpeed;
    private Vector2 previousVelo;
    private bool jumped = false;
    private float jumpChargeTime = 0f;
    public float MaxJumpChargeTime = 1f;
    public float ChargeMult = 1f;
    public float JumpHeight = 4f;
    public float FastFallMult = 1f;
    public float MoveSpeed = 1f;
    public float SlamYdamp = 2f;
    public float SlamYmult = 1f;
    public float SlamYmin = 1f;
    public float SlamDelay = 0.1f;
    public float TeleportStun = 1f;
    private void Start() {
        body = GetComponent<Rigidbody2D>();
        projPool = GetComponent<ProjectilePool>();
        col = GetComponent<BoxCollider2D>();
        iniScale = transform.localScale;

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
        // Block all actions just after teleported
        if (transform.localScale != iniScale) {
            return;
        }
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
        if (special.IsPressed()) { // specialDelta decreases here to allow "charging up" button animation
            SpecialDelta = Mathf.MoveTowards(SpecialDelta, 0f, Time.deltaTime);
        } else {
            if (special.WasReleasedThisFrame() && SpecialDelta <= 0f) {
                Teleport();
            }
            SpecialDelta = SpecialCooldown;
        }
    }

    private void FixedUpdate() {
        // Block all actions just after teleported
        float curScale = transform.localScale.x;
        float targetScale = iniScale.x;
        if (curScale < targetScale) {
            transform.localScale = iniScale * Mathf.MoveTowards(curScale, targetScale,
                targetScale * Time.deltaTime / TeleportStun);
            if (iniScale == transform.localScale) {
                body.simulated = true;
            }
            return;
        }
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
        iniSpeed = body.linearVelocityY;

        // dangerzone creation
        if (iniToFinalSpeed.ContainsKey(iniSpeed)) {
            iniSpeed = 0f;
        }
        CreateJumpDangerZone(GuessFinalVelo(iniSpeed));

        jumped = true;
    }
    private IEnumerator Slam(float yVelo) {
        Queue<GameObject> projQueue = new();
        // remember that yVelo is negative 
        if (iniSpeed > 0f) {
            iniToFinalSpeed.Add(iniSpeed, yVelo);
            iniSpeed = 0f;
        }
        DangerZone.FreePath(dangerZoneIndex);
        yVelo += SlamYmin;
        float offsetMult = 1.1f;
        Vector3 slamOrigin = gameObject.transform.position + Vector3.down;
        while (yVelo < 0f) {
            foreach (float direction in directions) {
                GameObject slamProj = projPool.Get();
                ProjectileEffects projEffects = slamProj.GetComponent<ProjectileEffects>();
                slamProj.transform.position = slamOrigin + direction * offsetMult * Vector3.right;
                slamProj.GetComponent<Rigidbody2D>().linearVelocityY = -yVelo * SlamYmult;
                slamProj.GetComponent<FallingProjectile>().StoredVelo.y = -yVelo * SlamYmult;
                projEffects.PoolingSystem = projPool;
                projEffects.OwnerTag = gameObject.tag;
                projEffects.DangerZoneSystem = DangerZone;
                projEffects.CreateDangerZone();
                slamProj.SetActive(false);
                projQueue.Enqueue(slamProj);
            }
            offsetMult++;
            yVelo += SlamYdamp;
        }
        while (projQueue.Count > 0) {
            yield return slamDelay;
            foreach (float direction in directions) {
                GameObject fallingProjectile = projQueue.Dequeue();
                fallingProjectile.SetActive(true);
                fallingProjectile.GetComponent<FallingProjectile>().RestoreVelo();
            }
        }
    }
    private void Teleport() {
        iniSpeed = 0f;
        Vector3 tpTarget = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) + Vector3.back * -10f;
        if (Physics2D.OverlapBox(tpTarget, transform.localScale, 0f, LayerMask.GetMask("Ground"))) {
            return; // exits if it would tp into the ground
        }
        transform.position = tpTarget;
        transform.localScale = Vector3.zero;
        body.linearVelocity = Vector2.zero;
        body.simulated = false; // turn off physics simulations (collisions and movement)
    }

    private float GuessFinalVelo(float ini) { // uses past slams to guess new value
        if (iniToFinalSpeed.Count < 1) {
            return 0f;
        }
        int low = 0;
        int high = iniToFinalSpeed.Count - 1;

        while (low <= high) {
            int mid = (low + high) / 2;
            float key = iniToFinalSpeed.Keys[mid];

            if (key == ini) {
                return iniToFinalSpeed.Values[mid];
            }

            if (key < ini) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }

        if (low == 0) {
            return iniToFinalSpeed.Values[0];
        }

        if (low == iniToFinalSpeed.Count) {
            return iniToFinalSpeed.Values[iniToFinalSpeed.Count - 1];
        }

        float lowerKey = iniToFinalSpeed.Keys[low - 1];
        float upperKey = iniToFinalSpeed.Keys[low];

        if (ini - lowerKey < upperKey - ini) {
            return iniToFinalSpeed.Values[low - 1];
        } else { 
            return iniToFinalSpeed.Values[low];
        }
    }

    private void CreateJumpDangerZone(float yVelo) {
        // remember that yVelo is negative 
        int projWaveCount = -Mathf.FloorToInt((yVelo + SlamYmin) / SlamYdamp);
        if (projWaveCount < 1) {
            dangerZoneIndex = -1;
            return;
        }
        var bounds = col.bounds;
        Vector2 bottomLeft = bounds.min;
        Vector2 topLeft = new(bounds.min.x, bounds.max.y);
        Vector2 topRight = bounds.max;
        Vector2 bottomRight = new(bounds.max.x, bounds.min.y);
        dangerZoneIndex = DangerZone.NewPath(new Vector2[4] {
            bottomLeft + projWaveCount * Vector2.left,
            topLeft + projWaveCount * Vector2.left,
            topRight + projWaveCount * Vector2.right,
            bottomRight + projWaveCount * Vector2.right
        });
    }
}
