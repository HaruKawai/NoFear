using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    void Start()
    {
        rb.velocity = transform.right * speed;
    }
    void OnTriggerEnter2D (Collider2D collision) 
    {
        //Debug.Log(collision.name);
        Enemy enemy = collision.GetComponent<Enemy>();
        if(enemy != null) {
            enemy.TakeDamage(damage);
        }
        if (collision.tag != "Item") {
        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        }
    }

}
