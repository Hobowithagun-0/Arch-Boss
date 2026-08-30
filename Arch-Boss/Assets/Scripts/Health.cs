using UnityEngine;

public class Health : MonoBehaviour {
    private int health;
    private float[] resistances = new float[System.Enum.GetValues(typeof(DamageType)).Length];
    public event System.Action<int> OnHealthChanged; // Runs these functions everytime health is changed (for UI mainly)
    public int MaxHealth = 100;
    public int Value {
        get => health;
        private set {
            health = value;
            OnHealthChanged?.Invoke(health);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        Value = MaxHealth;
    }

    [ContextMenu("Check hp")]
    private void PrintHealth() {
        Debug.Log(health);
    }
    /// <summary>
    /// Decreases health based on damage and damage multipliers.
    /// </summary>
    public void TakeDamage(int dmg, DamageType type) {
        Value -= Mathf.RoundToInt(dmg * (1 - resistances[(int)type]));        
    }

    /// <summary>
    /// Increases health by hp, capped at max health. <br/>
    /// hp = 0 can be used to set health to max health if ever health > max health. <br/>
    /// Negative values of hp can be used to deal damage without damage multipliers.
    /// </summary>
    public void Heal(int hp) {
        Value = Mathf.Min(MaxHealth, health + hp);
    }

    /// <summary>
    /// Sets resistance of DamageType to value. Ideally values are between 0 and 1 (unenforced)
    /// </summary>

    public void SetResistance(int value, DamageType type) {
        resistances[(int)type] = value;
    }
}
