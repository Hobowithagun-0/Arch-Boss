using UnityEngine;

public class FallingProjectile : ProjectileEffects {
    /// <summary> The velocity that will be applied when RestoreVelo() is called </summary>
    public Vector2 StoredVelo;

    /// <summary> Applies the stored velocity to the rigid body </summary>
    public void RestoreVelo() { 
        rigidbody.linearVelocity = StoredVelo;
    }
    protected override Vector2[] GetDangerZonePoints() {
        // we only care about y velo and how high the proj will go before fallin
        float yVelo = rigidbody.linearVelocityY;
        if (yVelo < 0f) {
            Debug.LogWarning("Negative Y velocity will lead to undefined behaviour");
        }
        Vector2 heightIncrease = new(0f, yVelo * yVelo / (2f * -Physics2D.gravity.y * rigidbody.gravityScale));
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

        return new Vector2[4] {
            bottomLeft,
            topLeft + heightIncrease,
            topRight + heightIncrease,
            bottomRight
        };
    }
}
