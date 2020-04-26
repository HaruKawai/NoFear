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
    [SerializeField] private float meleeDistance;
    [SerializeField] private float shootingDistance;
    [SerializeField] private float trackingDistance;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    public bool lookingRight;
    public bool canBePossesed = true;
    private Animator anim;
    private bool moving = true;
    private Coroutine attackCoroutine;
    private bool attacking;
    private bool shooting;
    public  bool tracking;
    GameObject player;

    public EnemyBullet bullet;
    private Vector3 bulletDirection;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {

        Debug.DrawRay((Vector2)transform.position + new Vector2(1f, 0f) * transform.right, Vector2.down);
        if (tracking)
            Chase();
        else
            Patroll();

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

    private void Patroll()
    {
        var position = (Vector2)transform.position;
        bool isGrounded = Physics2D.Raycast(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down, 2f, ground);
        bool hasFreePath = Physics2D.Raycast(position, transform.right, 1.5f, ground);

        if (!isGrounded || hasFreePath)
            Flip();
    }

    private void Chase()
    {
        Vector2 position = (Vector2)transform.position;
        Vector2 playerPosition = (Vector2)player.transform.position;
        bool playerAgroRange = Physics2D.Raycast(position, playerPosition - position, trackingDistance, agro);
        bool isGrounded = Physics2D.Raycast(position + new Vector2(1f, 0f) * transform.right, Vector2.down, 2f, ground);


        //esta dentro del area de agro
        if (playerAgroRange)
        {
            //veo al player
            if (Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro).collider.gameObject.CompareTag("Player"))
            {
                if (Mathf.Sign((playerPosition - position).x) != Mathf.Sign(transform.right.x))
                    Flip();
            }
            else
            {
                moving = false;
                rb.velocity = Vector2.zero;
            }
            //si nos vamos a caer paramos
            if (!isGrounded)
            {
                moving = false;
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (!shooting && !attacking)
            {
                tracking = false;
                moving = true;
                sprite.color = Color.white;
                rb.velocity = speed * Time.fixedDeltaTime * transform.right;
            }


        }
    }

    private void FixedUpdate()
    {
        if (moving)
            rb.velocity = speed * Time.fixedDeltaTime * transform.right;
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 position = (Vector2)transform.position;
        Vector2 playerPosition = (Vector2)player.transform.position;
        bool playerAgro = Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro);
        Debug.DrawRay(position, playerPosition - position);
        
        if (!tracking && playerAgro)
            if (Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro).collider.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    TrackPlayer();
                }
            }
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPLayer = collision.gameObject.CompareTag("Player");
        Vector2 position = (Vector2)transform.position;
        Vector2 playerPosition = (Vector2)player.transform.position;
        bool playerAgro = Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro);
        bool shootingRange = Physics2D.Raycast(position, playerPosition - position, shootingDistance, agro);
        bool isGrounded = Physics2D.Raycast(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down, 2f, ground);


        bool melee = Physics2D.Raycast((Vector2)transform.position, playerPosition - position, meleeDistance, playerMask);
        if (isPLayer && isGrounded)
        {
            if (!tracking && playerAgro)
                if (Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro).collider.gameObject.CompareTag("Player"))
                {
                    if (collision.gameObject.CompareTag("Player"))
                    {
                        TrackPlayer();
                    }
                }
            if (melee)
            {
                if (!attacking && !shooting)
                {
                    EnemyAttack();
                }
                    
            }
            else
            {
                if (shootingRange)
                {
                    if (!shooting && !attacking && Physics2D.Raycast(position, playerPosition - position, shootingDistance, agro).collider.gameObject.CompareTag("Player"))
                    {
                        EnemyShoot();
                    }
                }
            }
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
        //para coger la posición del jugador cuando se inicia la animación de disparo y no se salga del area de agro
        bulletDirection = (player.transform.position + transform.up * 0.3f) - (transform.position + transform.right * 2.6f + transform.up * 0.9f);
    }

    public void DamageAttackFrame()
    {
        Vector2 position = (Vector2)transform.position;
        Vector2 playerPosition = (Vector2)player.transform.position;
        attacking = true;
        if(Physics2D.Raycast((Vector2)transform.position, playerPosition - position, meleeDistance, playerMask))
        {
            Player2DControll.Instance.TakeDamage();
        }
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
        tracking = true;
        sprite.color = Color.red;

    }

    public void FireEvent()
    {
        if (isActiveAndEnabled)
        {
            EnemyBullet ammo = Instantiate<EnemyBullet>(bullet);
            ammo.gameObject.SetActive(true);
            ammo.transform.position = transform.position + transform.right * 2.1f + transform.up * 0.9f;
            ammo.direction = bulletDirection;

        }

    }

    public void Die()
    {
        anim.SetTrigger("Dead");
    }

    public void DieEvent()
    {
        Destroy(this);
    }
}