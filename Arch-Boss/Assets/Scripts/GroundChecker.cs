using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public Vector2 groundCheckSize = new Vector2(2.0f, 2.0f);
    public LayerMask groundLayer;

    public bool isGrounded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, groundCheckSize, 0f, groundLayer);
        isGrounded = (collider != null);
    }
    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawCube(transform.position, groundCheckSize);
    }
}
