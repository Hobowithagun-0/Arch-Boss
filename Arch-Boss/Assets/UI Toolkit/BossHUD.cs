using UnityEngine;
using UnityEngine.UIElements;

public class BossHUD : MonoBehaviour {

    [SerializeField] private GameObject target; // only for unity editor to use
    private Health targetHealth;
    private ProgressBar bar;
    private void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        bar = root.Q<ProgressBar>("healthbar");
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
        targetHealth.OnHealthChanged -= UpdateHealth; // this line is only needed when reload on playmode is off i think
        targetHealth.OnHealthChanged += UpdateHealth;
        targetHealth.Heal(0);
    }
}
