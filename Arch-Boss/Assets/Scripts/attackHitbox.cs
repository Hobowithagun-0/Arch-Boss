using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class attackHitbox : MonoBehaviour
{
    float timer = 0;
    Collider2D hitbox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitbox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            Destroy(gameObject);
        }
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFil = new ContactFilter2D();
        contactFil.layerMask = LayerMask.GetMask("Default");
        hitbox.Overlap(contactFil,results);

        Debug.Log(results[0]);
    }
}

