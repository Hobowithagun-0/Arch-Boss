using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileEffects : MonoBehaviour {
    private int pierce;
    private float ttl;
    public int Damage = 0;

    /// <summary> Type of dmg reduction the entity will use against this projectile. </summary>
    public DamageType Type = DamageType.Physical;

    /// <summary> number of entities it can pass through before dying. Negative for infinite </summary>
    public int Pierce = 0;
    public float TimeToLive = 1f;
    /// <summary> Tag of owner, projectile will not interact with owner tag </summary>
    public string OwnerTag = "Placeholder";
    public ProjectilePool PoolingSystem;
    /// <summary> Used by projectiles to create a danger zone for player to avoid </summary>
    public DangerZone DangerZoneSystem;
    private int dangerZonePath = -1;

    private void OnEnable() { // stores initial values for reset & gets a dangerzone path
        pierce = Pierce;
        ttl = TimeToLive;
    }
    private void Update() {
        TimeToLive -= Time.deltaTime;
        if (TimeToLive <= 0) {
            ReturnToPool();
        }
    }

    private void OnTriggerStay2D(Collider2D collider) {
        GameObject hitObject = collider.gameObject;
        HurtboxCode hurtbox = hitObject.GetComponent<HurtboxCode>();
        if (hurtbox && !hitObject.CompareTag(OwnerTag)) { 
            Interact(hurtbox);
            if (Pierce-- == 0) {
                ReturnToPool();
            }
        }
    }

    public void CreateDangerZone(Vector2 velocity) {
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 halfSize = boxCollider.size / 2f;
        Vector2 offset = boxCollider.offset;

        Vector2 bottomLeft = boxCollider.transform.TransformPoint(
            offset + new Vector2(-halfSize.x, -halfSize.y)
        );

        Vector2 topLeft = boxCollider.transform.TransformPoint(
            offset + new Vector2(-halfSize.x, halfSize.y)
        );

        Vector2 topRight = boxCollider.transform.TransformPoint(
            offset + new Vector2(halfSize.x, halfSize.y)
        );

        Vector2 bottomRight = boxCollider.transform.TransformPoint(
            offset + new Vector2(halfSize.x, -halfSize.y)
        );

        dangerZonePath = DangerZoneSystem.NewPath(new Vector2[4] {
            bottomLeft,
            topLeft,
            topRight + velocity * TimeToLive,
            bottomRight + velocity * TimeToLive
        });
    }

    protected void ReturnToPool() {
        Pierce = pierce;
        TimeToLive = ttl;
        if (dangerZonePath > -1) {
            DangerZoneSystem.FreePath(dangerZonePath);
        }
        PoolingSystem.Release(gameObject);
    }
    protected virtual void Interact(HurtboxCode target) {
        target.Health.TakeDamage(Damage, Type);
    }

}
