﻿using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask ground;
    private Rigidbody2D rb;
    public bool lookingRight;
    public bool canBePossesed = true;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        var position = (Vector2)transform.position;

        bool isGrounded = Physics2D.Raycast(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down, 2f, ground);
        Debug.DrawRay(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down * 2f, Color.red);
        bool hasFreePath = Physics2D.Raycast(position, transform.right, 1.5f, ground);
        Debug.DrawRay(position, transform.right);
        if (!isGrounded || hasFreePath)
            Flip();
        anim.SetFloat("Speed", 1f);
    }

    private void Flip()
    {
        lookingRight = !lookingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void FixedUpdate()
    {
        rb.velocity = speed * Time.fixedDeltaTime * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entering collision");
            anim.SetTrigger("attack");
        }
        /*
        else if (collider.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Shoot", true);
        }*/
    }

    private void OnTriggerExit2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exiting collision");
            anim.SetTrigger("Attack");
        }
        /*
        else if (collider.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Shoot", false);
        }*/
    }
}