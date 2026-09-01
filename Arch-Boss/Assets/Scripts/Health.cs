using UnityEngine;

public class Health : MonoBehaviour {
    private int health;
    private int lastDmgValue = 0;
    private float lastDmgDelta = 0f;
    private readonly float[] resistances = new float[System.Enum.GetValues(typeof(DamageType)).Length]; 

    /// <summary> Runs these functions everytime health is changed (for UI mainly) </summary>
    public event System.Action<int> OnHealthChanged;
    public int MaxHealth = 100;
    public int Value {
        get => health;
        private set {
            health = value;
            OnHealthChanged?.Invoke(health);
        }
    }
    /// <summary> Time before an attack can hit again. <br/> 
    /// If a stronger attack hits during this window, the difference in damage is applied instanly </summary>
    public float InvulnerableDuration = 0.5f;

    private void Start() {
        Value = MaxHealth;
    }

    private void Update() {
        lastDmgDelta += Time.deltaTime;
    }

    [ContextMenu("Check hp")]
    private void PrintHealth() {
        Debug.Log(health);
    }
    /// <summary> Decreases health based on damage and damage multipliers. </summary>
    public void TakeDamage(int dmg, DamageType type) {
        int damage = Mathf.RoundToInt(dmg * (1 - resistances[(int)type]));
        if (lastDmgDelta >= InvulnerableDuration) {
            Value -= damage;
            lastDmgValue = damage;
            lastDmgDelta = 0f;
        } else if (lastDmgValue < damage) {
            Value -= (damage - lastDmgValue);
            lastDmgValue = damage;
            lastDmgDelta = 0f;
        }
              
    }

    /// <summary> Increases health by hp, capped at max health. <br/>
    /// hp = 0 can be used to set health to max health if ever health > max health. <br/>
    /// Negative values of hp can be used to deal damage without damage multipliers. </summary>
    public void Heal(int hp) {
        Value = Mathf.Min(MaxHealth, health + hp);
    }

    /// <summary> Sets resistance of DamageType to value. Ideally values are between 0 and 1 (unenforced) </summary>

    public void SetResistance(int value, DamageType type) {
        resistances[(int)type] = value;
    }
}
