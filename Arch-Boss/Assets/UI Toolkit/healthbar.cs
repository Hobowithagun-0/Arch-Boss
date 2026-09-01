using UnityEngine;
using UnityEngine.UIElements;

public class healthbar : MonoBehaviour {

    public Health target;
    private ProgressBar bar;
    void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        bar = root.Q<ProgressBar>("healthbar");
        target.OnHealthChanged -= UpdateHealth;
        target.OnHealthChanged += UpdateHealth;
        target.Heal(0);
    }

    private void UpdateHealth(int hp) {
        bar.title = $"Health: {target.Value}";
        bar.highValue = target.MaxHealth;
        bar.value = target.Value;
    }
}
