using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileEffects : MonoBehaviour {
    private float cooldownTimer = 0f;
    public int Damage = 0;

    /// <summary> Type of dmg reduction the entity will use against this projectile. </summary>
    public DamageType Type = DamageType.Physical;

    /// <summary> number of entities it can pass through before dying. Negative for infinite </summary>
    public int Pierce = 0;
    public float TimeToLive = 1f;

    /// <summary> Time before the same projectile can affect the same entity again </summary>
    public float EffectCooldown = 0.5f;

    /// <summary> Tag of owner, projectile will not interact with owner tag </summary>
    public string OwnerTag = "Placeholder";
    public ProjectilePool PoolingSystem;

    private void Update() {
        cooldownTimer -= Time.deltaTime;
        TimeToLive -= Time.deltaTime;
        if (TimeToLive <= 0) {
            PoolingSystem.Release(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collider) {
        GameObject hitObject = collider.gameObject;
        Debug.Log("hi");
        if (hitObject.layer == LayerMask.NameToLayer("Entity") && !hitObject.CompareTag(OwnerTag) && cooldownTimer <= 0) {
            Interact(hitObject);
            cooldownTimer = EffectCooldown;
            if (Pierce-- == 0) {
                PoolingSystem.Release(gameObject);
            }
        }
    }

    protected virtual void Interact(GameObject target) {
        Health hp = target.GetComponent<Health>();
        hp.TakeDamage(Damage, Type);
    }
}
