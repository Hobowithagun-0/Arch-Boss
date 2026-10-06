using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenu : MonoBehaviour {

    private VisualElement fadeOut;
    private float fadeOutOpacity = 0f;
    public float FadeOutTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        Button start = root.Q<Button>("Start");
        Button binds = root.Q<Button>("Keybinds");

        fadeOut = root.Q<VisualElement>("FadeOut");

        start.clicked += StartButton;
        binds.clicked += KeybindsButton;
    }

    private void StartButton() {
        Debug.Log(" start Button clicked!");
        StartCoroutine(TransitionTo("WizardTestScene"));
    }
    private void KeybindsButton() {
        Debug.Log(" binds Button clicked!");
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
