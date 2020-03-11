using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Starting in 2 seconds.
// a projectile will be launched every 0.3 seconds

public class controladorTiempo : MonoBehaviour
{
    public Animator animator;
    public float speed;
    private Rigidbody2D rb2d;
    public float movementV = 10f;
    public float movementH;
    public bool m_FacingRight;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        InvokeRepeating("LaunchProjectile", 1.6f, 6.0f);
        InvokeRepeating("StopLaunchProjectile", 4.6f, 6.0f);
        
        InvokeRepeating("LaunchProjectile2", 0f, 2.0f);
        InvokeRepeating("StopLaunchProjectile2", 0.5f, 2.0f);

        InvokeRepeating("LaunchProjectile3", 5.0f, 5.0f);
    }

    void LaunchProjectile()
    {
        animator.SetBool("Walk", true);
    }
    void StopLaunchProjectile()
    {
        animator.SetBool("Walk", false);
    }


    void LaunchProjectile2()
    {
        animator.SetBool("Toma", true);
    }
    void StopLaunchProjectile2()
    {
        animator.SetBool("Toma", false);
    }

    void LaunchProjectile3()
    {

        movementH = Random.Range(-10f, 10f);
        if (movementH > 0)
        {
            m_FacingRight = false;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            m_FacingRight = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }


        Vector2 movement = new Vector2(movementH, movementV);
        rb2d.AddForce(movement * speed);
        animator.SetBool("Jump", true);
        //animator.SetBool("Jump", false);
    }
}