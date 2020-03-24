using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Basic : MonoBehaviour
{
    public float speed;
    public Transform platformDetector;

    Rigidbody2D rb2d;
    Vector2 direction;
   

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        direction = Vector2.left;
    }

    void Update()
    {
        if (Physics2D.Raycast(platformDetector.position, Vector2.down, 1f).collider == null)
        {
            direction *= -1;
            rb2d.transform.Rotate(0f, 180f, 0f);
        }
        Vector2 newPosition = Vector2.MoveTowards(rb2d.position, rb2d.position + direction, speed * Time.deltaTime);
        rb2d.MovePosition(newPosition);
    }
}
