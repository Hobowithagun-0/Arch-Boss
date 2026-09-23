using UnityEngine;

public class FloatyArrows : ProjectileEffects {

    private void Start() {
        boxCollider.includeLayers |= LayerMask.GetMask("Ground"); // adds ground layer to collision checks
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            ReturnToPool();
        }
    }
}
