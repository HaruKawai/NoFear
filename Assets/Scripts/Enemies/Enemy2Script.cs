using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class Enemy2Script : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask agro;
    [SerializeField] private float grenadeX;
    [SerializeField] private float grenadeY;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    public bool lookingRight;
    public bool canBePossesed = true;
    private Animator anim;
    private bool moving = true;
    private Coroutine attackCoroutine;
    GameObject player;
    private bool attacking;
    private Vector2 playerPos;
    public Grenade grenade;


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
        if (!attacking)
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

    /*
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
                //tracking = false;
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
            //if (!throwing && !running)
            //{
                tracking = false;
                moving = true;
                sprite.color = Color.white;
                rb.velocity = speed * Time.fixedDeltaTime * transform.right;
            //}


        }
    }*/


    private void Attack()
    {
        moving = false;
        attacking = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.SetTrigger("Throw");
        //para coger la posición del jugador cuando se inicia la animación de disparo y no se salga del area de agro
        playerPos = player.transform.position;
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
    }
    */
    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPLayer = collision.gameObject.CompareTag("Player");
        Vector2 position = (Vector2)transform.position;
        Vector2 playerPosition = (Vector2)player.transform.position;
        bool playerAgro = Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro);
        bool isGrounded = Physics2D.Raycast(position + new Vector2(0.2f, 0f) * transform.right, Vector2.down, 2f, ground);


        if (isPLayer && isGrounded && playerAgro)
        {
            if (!attacking)
            {
                if (Physics2D.Raycast(position, playerPosition - position, Mathf.Infinity, agro).collider.gameObject.CompareTag("Player"))
                {
                    Attack();
                }
                else
                {
                    //attacking = false;
                }
            }
            /*

            if (throwingRange)
            {
                if (!throwing && !running && Physics2D.Raycast(position, playerPosition - position, throwingDistance, agro).collider.gameObject.CompareTag("Player"))
                {
                    EnemyThrow();
                }
                else
                    Player2DControll.Instance.TakeDamage();
            }*/
        }
    


    }

    private void EnemyThrow()
    {
        moving = false;
        attacking = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.SetTrigger("Throw");
        //para coger la posición del jugador cuando se inicia la animación de disparo y no se salga del area de agro
        playerPos = transform.InverseTransformPoint(player.transform.position);
    }


    public void EndThrowEvent()
    {
        attacking = false;
        moving = true;
        rb.isKinematic = false;
    }


    public void FireEvent()
    {
        if (isActiveAndEnabled)
        {
            Grenade ammo = Instantiate<Grenade>(grenade);
            ammo.gameObject.SetActive(true);
            ammo.transform.position = transform.position + transform.right * 0.2f + transform.up * 1.2f;
            ammo.lookingRight = lookingRight;
            ammo.playerPos = transform.InverseTransformPoint(player.transform.position);
            ammo.grenadeX = grenadeX;
            ammo.grenadeY = grenadeY;
            ammo.throwByPlayer = false;
        }

    }

    public void Die()
    {
        anim.SetTrigger("Dead");
    }

    public void DieEvent()
    {
        if (isActiveAndEnabled)
            Destroy(gameObject);
    }
}