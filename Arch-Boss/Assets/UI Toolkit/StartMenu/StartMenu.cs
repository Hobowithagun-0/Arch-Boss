using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour {

    [SerializeField] private VisualTreeAsset settingsUxml;
    private VisualElement menu;
    private VisualElement settings;
    private VisualElement fadeOut;
    private float fadeOutOpacity = 0f;
    public float FadeOutTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        menu = root;

        Button start = root.Q<Button>("Start");
        Button binds = root.Q<Button>("Keybinds");

        fadeOut = root.Q<VisualElement>("FadeOut");

        start.clicked += StartButton;
        binds.clicked += KeybindsButton;
    }

    private void StartButton() {
        StartCoroutine(TransitionTo("WizardTestScene"));
    }
    private void KeybindsButton() {
        if (settings == null) {
            settings = settingsUxml.Instantiate();
            
            settings.style.width = Length.Percent(100);
            settings.style.height = Length.Percent(100);
            settings.style.position = Position.Absolute;

            PopulateSettings(settings.Q<VisualElement>("KeybindsMenu"));

            Button exit = settings.Q<Button>("Exit");
            exit.clicked += () => { settings.RemoveFromHierarchy(); };
        }

        menu.Add(settings);
    }

    private IEnumerator TransitionTo(string sceneName) {
        // start loading next scene
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;

        while (fadeOutOpacity < 1f) {
            fadeOutOpacity += Time.deltaTime / FadeOutTime;
            fadeOut.style.opacity = Mathf.Min(fadeOutOpacity, 1f);
            yield return null;
        }

        // Wait until the scene has finished loading.
        while (loading.progress < 0.9f) {
            yield return null;
        }

        // Both fading and loading are complete.
        loading.allowSceneActivation = true;
    }

    private void PopulateSettings(VisualElement container) {

        foreach (InputAction action in InputSystem.actions.FindActionMap("Player")) {
            for (int i = 0; i < action.bindings.Count; i++) {
                InputBinding binding = action.bindings[i];

                if (binding.isComposite) { // IGNORE "root" binds, eg "WASD"
                    continue;
                }

                Button button = new Button();
                string name;
                int bindingIndex = i;

                if (binding.isPartOfComposite) { // stuff like Up Down Left Right (part of WASD)
                    name = binding.name.ToUpper();
                } else { // stuff like buttons
                    name = action.name.ToUpper();
                }

                button.text = $"{name}: {action.GetBindingDisplayString(bindingIndex)}";

                button.clicked += () => {
                    button.text = $"{name}: Listening...";

                    action.Disable();

                    action.PerformInteractiveRebinding(bindingIndex)
                        .WithCancelingThrough("<Keyboard>/escape")
                        .OnComplete(operation => {
                            operation.Dispose();
                            action.Enable();

                            button.text = $"{name}: {action.GetBindingDisplayString(bindingIndex)}";
                        })
                        .OnCancel(operation => {
                            operation.Dispose();
                            action.Enable();

                            button.text = $"{name}: {action.GetBindingDisplayString(bindingIndex)}";
                        })
                        .Start();
                };

                container.Add(button);
            }
        }
    }
}
