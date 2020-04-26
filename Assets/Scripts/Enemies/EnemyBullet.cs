using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    public Vector2 direction;
    private Rigidbody2D rb;
    public bool shootByPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = speed * Time.fixedDeltaTime * direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !shootByPlayer)
        {
            if(Player2DControll.Instance.playerMode == Player2DControll.PlayerMode.Human)
                Player2DControll.Instance.TakeDamage();
            Destroy(gameObject);
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 11 && shootByPlayer)
        {
            if (collision.gameObject.name == "Enemy1")
                collision.gameObject.GetComponent<EnemyScript>().Die();
            if (collision.gameObject.name == "Enemy2")
                collision.gameObject.GetComponent<Enemy2Script>().Die();
            Destroy(gameObject);
        }
    }
}
