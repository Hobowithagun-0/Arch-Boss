using UnityEngine;
using UnityEngine.UIElements;

public class Healthbar : MonoBehaviour {

    public Health Target;
    private ProgressBar bar;
    private void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        bar = root.Q<ProgressBar>("healthbar");
        Target.OnHealthChanged -= UpdateHealth;
        Target.OnHealthChanged += UpdateHealth;
        Target.Heal(0);
    }

    private void UpdateHealth(int hp) {
        bar.title = $"Health: {Target.Value}";
        bar.highValue = Target.MaxHealth;
        bar.value = Target.Value;
    }
}
