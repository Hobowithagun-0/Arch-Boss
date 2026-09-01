using UnityEngine;

public abstract class BossBehaviour:MonoBehaviour
{
    protected Vector2 moveInput;
    public abstract void Move(Vector2 moveInput);
    public abstract void Attack();
   
}
