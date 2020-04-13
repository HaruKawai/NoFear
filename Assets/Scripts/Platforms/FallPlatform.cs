using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D collider;
    private bool cantBack;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && !cantBack)
        {
            cantBack = true;
            PlatformManager.Instance.StartCoroutine("SpawnPlatform",
                    new Vector2 (transform.position.x, transform.position.y));
            Invoke("DropPlatform", 1f);
            //GetComponent<Animator>().SetTrigger("Fall");
            GetComponent<Animator>().SetBool("Falling", true);
            Destroy(gameObject, 3f);
        }
    }

    void DropPlatform() 
    {
        rb.isKinematic = false;
        collider.enabled = false;
    }
}
