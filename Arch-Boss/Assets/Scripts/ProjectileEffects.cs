using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileEffects : MonoBehaviour {
    public int Damage = 0;

    /// <summary> Type of dmg reduction the entity will use against this projectile. </summary>
    public DamageType Type = DamageType.Physical;

    /// <summary> number of entities it can pass through before dying. Negative for infinite </summary>
    public int Pierce = 0;
    public float TimeToLive = 1f;

    /// <summary> Tag of owner, projectile will not interact with owner tag </summary>
    public string OwnerTag = " ";

    private void Update() {
        TimeToLive -= Time.deltaTime;
        if (TimeToLive <= 0) {
            Destroy(transform.root.gameObject); // CHANGE THIS WHEN WE USE POLLING SYSTEM
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        GameObject hitObject = collider.gameObject;
        if (hitObject.layer == LayerMask.NameToLayer("Entity") && !hitObject.CompareTag(OwnerTag)) {
            Interact(hitObject);
            if (Pierce-- == 0) {
                Destroy(transform.root.gameObject); // CHANGE THIS WHEN WE USE POLLING SYSTEM
            }
        }
    }

    protected virtual void Interact(GameObject target) {
        Health hp = target.GetComponent<Health>();
        hp.TakeDamage(Damage, Type);
    }
}
