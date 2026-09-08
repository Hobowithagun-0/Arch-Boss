using UnityEngine;

public abstract class BossBehaviour : MonoBehaviour {
    /// <summary> Time since last attack </summary>
    public float AttackDelta;
    /// <summary> Time since last special </summary>
    public float SpecialDelta;
    /// <summary> Time to charge attack </summary>
    public float AttackCooldown;
    /// <summary> Time to charge special </summary>
    public float SpecialCooldown;
}
