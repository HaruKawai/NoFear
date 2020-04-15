using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask agro;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    public bool lookingRight;
    public bool canBePossesed = true;
    private Animator anim;
    private bool moving = true;
    private Coroutine attackCoroutine;
    private bool attacking;
    private bool shooting;
    private bool trackingPLayer;
    GameObject player;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        var position = (Vector2)transform.position;

        bool isGrounded = Physics2D.Raycast(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down, 2f, ground);
        Debug.DrawRay(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down * 2f, Color.red);

        bool hasFreePath = Physics2D.Raycast(position, transform.right, 1.5f, ground);
        Debug.DrawRay(position, transform.right);

        bool playerAgro = Physics2D.Raycast(position, (Vector2)player.transform.position - position, 15f, agro);
        Debug.DrawRay(position, (Vector2)player.transform.position - position,Color.yellow);
        if (playerAgro) {
            if (Physics2D.Raycast(position, (Vector2)player.transform.position - position, 15f, agro).collider.gameObject.CompareTag("Player"))
                TrackPlayer();
            /*
            else
            {
                if (trackingPLayer && (!shooting || !attacking))
                {
                    moving = false;
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    moving = true;
                }
            }*/
            

        }
        else
        {
            trackingPLayer = false;
            sprite.color = Color.white;
        }

        if (!isGrounded || hasFreePath)
            Flip();
        if (moving)
            anim.SetFloat("Speed", 1f);
        else
            anim.SetFloat("Speed", 0f);
    }

    private void Flip()
    {
        lookingRight = !lookingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void FixedUpdate()
    {
        if (moving)
            rb.velocity = speed * Time.fixedDeltaTime * transform.right;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPLayer = collision.gameObject.CompareTag("Player");
        if (isPLayer && speed < 450 && !shooting)
            speed += 2;

        bool melee = Physics2D.Raycast((Vector2)transform.position, transform.right, 2f, playerMask);
        if (melee)
        {
            if (!attacking && !shooting)
            {
                EnemyAttack();
            }
            else
                Player2DControll.Instance.TakeDamage();
        }
        else
        {
            if (isPLayer)
            {
                if (!shooting && !attacking)
                {
                    EnemyShoot();
                }
                else
                    Player2DControll.Instance.TakeDamage();
            }
        }

       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speed = 300;
        }
    }

    private void EnemyAttack()
    {
        moving = false;
        shooting = false;
        attacking = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.SetTrigger("attack");
    }

    private void EnemyShoot()
    {
        moving = false;
        shooting = true;
        attacking = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.SetTrigger("Shoot");
    }

    public void DamageAttackFrame()
    {
        attacking = true;
    }

    public void EndShootEvent()
    {
        shooting = false;
        moving = true;
        rb.isKinematic = false;
    }

    //Animation event so it can move once the attack animation is over
    public void EndAttackEvent()
    {
        attacking = false;
        moving = true;
        rb.isKinematic = false;
    }

    private void TrackPlayer()
    {
        trackingPLayer = true;
        sprite.color = Color.red;
        if (moving)
        {
            if(Mathf.Sign(((Vector2)(transform.position - player.transform.position)).x) == (Mathf.Sign(transform.right.x)))
            {
                Flip();
            }
        }

    }
}