using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using UnityEditor.PackageManager;

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
        }

        VisualElement container = settings.Q<VisualElement>("KeybindsMenu");

        menu.Add(settings);

        foreach (InputAction map in InputSystem.actions.FindActionMap("Player")) {
            Button button = new Button();
            button.text = map.name;

            container.Add(button);
        }

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
}
