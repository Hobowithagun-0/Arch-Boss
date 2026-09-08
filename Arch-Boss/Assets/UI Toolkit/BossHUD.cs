using UnityEngine;
using UnityEngine.UIElements;

public class BossHUD : MonoBehaviour {

    [SerializeField] private GameObject target; // only for unity editor to use
    private Health targetHealth;
    private BossBehaviour targetScript;
    private ProgressBar bar;
    private VisualElement attackButton;
    private VisualElement specialButton;

    private void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void Update() {
        specialButton.style.height = Mathf.RoundToInt(specialButton.resolvedStyle.width * 
            Mathf.Max(1 - targetScript.SpecialDelta / targetScript.SpecialCooldown, 0f));
        attackButton.style.height = Mathf.RoundToInt(attackButton.resolvedStyle.width *
            Mathf.Max(1 - targetScript.AttackDelta / targetScript.AttackCooldown, 0f));
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        bar = root.Q<ProgressBar>("BossHealth");
        attackButton = root.Q<Image>("Button1").Q("ButtonOverlay");
        specialButton = root.Q<Image>("Button2").Q("ButtonOverlay");
        Debug.Log("UI RELOADED");
    }

    private void UpdateHealth(int hp) {
        bar.title = $"Health: {targetHealth.Value}";
        bar.highValue = targetHealth.MaxHealth;
        bar.value = targetHealth.Value;
    }

    [ContextMenu("Update Target")]
    private void UpdateTarget() {
        ChangeTarget(target);
    }
    public void ChangeTarget(GameObject newTarget) {
        if (targetHealth) { // already has a target assigned => unsubscribe from previous target
            targetHealth.OnHealthChanged -= UpdateHealth;
        }
        targetHealth = newTarget.GetComponent<Health>();
        targetScript = newTarget.GetComponent<BossBehaviour>();
        targetHealth.OnHealthChanged -= UpdateHealth; // this line is only needed when reload on playmode is off i think
        targetHealth.OnHealthChanged += UpdateHealth;
        targetHealth.Heal(0);
    }
}
