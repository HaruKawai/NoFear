using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask ground;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        var position = transform.position;
        bool isGrounded = Physics2D.Raycast(position, Vector2.down * 0.5f, 1f, ground);
        Debug.DrawRay(position, Vector2.down * 0.5f, Color.red);
        if (!isGrounded)
            sr.flipX = !sr.flipX;
    }

    private void FixedUpdate()
    {
        if(sr.flipX)
            rb.velocity = speed * Time.fixedDeltaTime * transform.right;
        else
            rb.velocity = speed * Time.fixedDeltaTime * -transform.right;
    }
}