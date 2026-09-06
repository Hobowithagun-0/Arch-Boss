using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FloatyArrows : ProjectileEffects {

    private BoxCollider2D col;

    private void Start() {
        col = GetComponent<BoxCollider2D>();
        col.includeLayers |= LayerMask.GetMask("Ground"); // adds ground layer to collision checks
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            ReturnToPool();
        }
    }
}
