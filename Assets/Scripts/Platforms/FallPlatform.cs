using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    Rigidbody2D rb;
    private bool cantBack;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && !cantBack)
        {
            cantBack = true;
            PlatformManager.Instance.StartCoroutine("SpawnPlatform",
                    new Vector2 (transform.position.x, transform.position.y));
            Invoke("DropPlatform", 0.5f);
            Destroy(gameObject, 2f);
        }
    }

    void DropPlatform() 
    {
        rb.isKinematic = false;
    }
}
