using UnityEngine;

public abstract class BossBehaviour : MonoBehaviour {
    /// <summary> Time since last attack </summary>
    public float AttackDelta;

    /// <summary> Time since last special </summary>
    public float SpecialDelta;

    /// <summary> Time to charge attack </summary>
    public float AttackCooldown {
        get => attackCooldown;
        set => attackCooldown = Mathf.Max(value, 0.0001f);
    }

    /// <summary> Time to charge special </summary>
    public float SpecialCooldown {
        get => specialCooldown;
        set => specialCooldown = Mathf.Max(value, 0.0001f);
    }

    private float attackCooldown = 1f;
    private float specialCooldown = 1f;
}