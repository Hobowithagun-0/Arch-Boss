using UnityEngine;

public class HurtboxCode : MonoBehaviour {
    public Health Health;
    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}
