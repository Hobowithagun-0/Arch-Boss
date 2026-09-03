using UnityEngine;

public class HurtboxCode : MonoBehaviour
{
    public Health health;
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
