using UnityEngine;

public class HitboxCode : MonoBehaviour {
    [SerializeField] private string[] tagToIgnore;
    [SerializeField] private int damage = 10;
    [SerializeField] private int pierce = 0; //number of extra hurtboxes it can hit before hiding. Negative for infinite
    [SerializeField] private DamageType damageType;
    private void OnTriggerStay2D(Collider2D collision) {
        //If there are no Tag to Ignore
        if (tagToIgnore != null) {
            foreach (string tag in tagToIgnore) {
                //If the object is the same Tag, return;
                if (collision.transform.root.CompareTag(tag)) {
                    return;
                }
            }
        }

        //If the code goes past this point, then it means it is a different Tag
        HurtboxCode hurtbox = collision.GetComponent<HurtboxCode>();

        //if that gameobject has a hurtbox, run this bit of code
        if (hurtbox != null) {
            hurtbox.Health.TakeDamage(damage, damageType);
            if (pierce-- == 0) {
                Hide();
            }
        }
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}
