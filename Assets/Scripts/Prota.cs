using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prota : MonoBehaviour
{
    
    public int health = 100;
    public HealthBar healthBar;
    public GameObject deathEffect;
    
    public void TakeDamage (int damage)
    {
        gameObject.GetComponent<Player2DControll>().damaged = true;
        health -= damage;
        healthBar.SetHealth(health);
        if(health <=0) {
            Die();
        }
    }
    public void TakeFood(int points) 
    {
        health += points;
        if(health >= 100) {
            health = 100;
        }
        healthBar.SetHealth(health);
    }
    void Die() 
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision) {
        var relativePosition = transform.InverseTransformPoint(collision.transform.position);
        if (relativePosition.y < -0.1)
        {
            if (collision.gameObject.tag == "Enemy") {
            collision.gameObject.GetComponent<CharacterController2D>().enabled = true;
            collision.gameObject.GetComponent<Player2DControll>().enabled = true;
            Destroy(transform.parent.gameObject);
            }
        }
    }
}
