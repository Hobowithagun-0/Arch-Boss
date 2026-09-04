using UnityEngine;

public class GroundChecker : MonoBehaviour {
    public Vector2 GroundCheckSize = new Vector2(2.0f, 2.0f);
    public LayerMask GroundLayer;

    public bool IsGrounded = false;

    // Update is called once per frame
    void Update() {
        Collider2D collider = Physics2D.OverlapBox(transform.position, GroundCheckSize, 0f, GroundLayer);
        IsGrounded = (collider != null);
    }
    private void OnDrawGizmos() {
        if (IsGrounded) {
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawCube(transform.position, GroundCheckSize);
    }
}
