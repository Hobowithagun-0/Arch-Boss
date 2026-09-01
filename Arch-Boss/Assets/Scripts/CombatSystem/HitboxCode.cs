using UnityEngine;

public class HitboxCode : MonoBehaviour
{
    [SerializeField] private string[] TagToIgnore;
    [SerializeField] private int damage = 10;
    [SerializeField] private DamageType damageType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If there are no Tag to Ignore
        if(TagToIgnore != null)
        {
            foreach (string layer in TagToIgnore)
            {
                //If the object is the same Tag, return;
                if(collision.transform.root.CompareTag(tag))
                {
                    return;
                }
            }
        }

        //If the code goes past this point, then it means it is a different Tag
        HurtboxCode hurtbox = collision.GetComponent<HurtboxCode>();

        //if hurtbox has something, run this bit of code
        if(hurtbox != null)
        {
            hurtbox.health.TakeDamage(damage, damageType);
            Hide();
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
