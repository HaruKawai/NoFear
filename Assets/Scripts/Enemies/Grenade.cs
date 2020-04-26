using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float force;
    public Vector2 playerPos;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator anim;
    public bool lookingRight;
    public float grenadeX;
    public float grenadeY;
    public bool throwByPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }

    private void Start()
    {
        if (lookingRight)
        {
            direction = new Vector2(-playerPos.x*grenadeX, -playerPos.y*grenadeY);
        }
        else
        {
            direction = new Vector2(playerPos.x*grenadeX, -playerPos.y*grenadeY);
        }
       
        //Debug.Log(playerPos);
        rb.AddForce(direction.normalized * force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !throwByPlayer)
        {
            Debug.Log("imhere");
            Player2DControll.Instance.TakeDamage();
            anim.SetTrigger("Explode");
            rb.velocity = new Vector2(0f, 0f);
            rb.gravityScale = 0;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            anim.SetTrigger("Explode");
            rb.velocity = new Vector2(0f, 0f);
            rb.gravityScale = 0;
        }
        if(collision.gameObject.layer == 11 && throwByPlayer)
        {
            anim.SetTrigger("Explode");
            rb.velocity = new Vector2(0f, 0f);
            rb.gravityScale = 0;
            if (collision.gameObject.name == "Enemy1")
                collision.gameObject.GetComponent<EnemyScript>().Die();
            if (collision.gameObject.name == "Enemy2")
                collision.gameObject.GetComponent<Enemy2Script>().Die();
        }
    }

    public void ExplodeEvent()
    {
        Destroy(gameObject);
    }
}
