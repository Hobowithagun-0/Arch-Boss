using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileEffects : MonoBehaviour {
    public int Damage = 0;
    public DamageType Type = DamageType.Physical;
    public int Pierce = 0; // number of entities it can pass through before dying. Negative for infinite
    public float TimeToLive = 1f;
    public string Owner = " "; // projectile will not damage its owner

    private void Update() {
        TimeToLive -= Time.deltaTime;
        if (TimeToLive <= 0) {
            Destroy(transform.root.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        GameObject hitObject = collider.gameObject;
        if (hitObject.layer == LayerMask.NameToLayer("Entity") && !hitObject.CompareTag(Owner)) {
            Interact(hitObject);
            if (Pierce-- == 0) {
                Destroy(transform.root.gameObject);
            }
        }
    }

    protected virtual void Interact(GameObject target) {
        Health hp = target.GetComponent<Health>();
        hp.TakeDamage(Damage, Type);
    }
}
