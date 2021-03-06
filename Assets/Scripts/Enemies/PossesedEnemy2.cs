﻿using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PossesedEnemy2 : MonoBehaviour
{
    [SerializeField] private float grenadeX;
    [SerializeField] private float grenadeY;
    private bool isGrounded;
    private bool onPlatform;
    public LayerMask ground;
    public LayerMask platform;
    public bool canJump;
    public bool canMove;
    private float horizontalMove;
    private float verticalMove;
    public float runSpeed;
    private Rigidbody2D rb;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    public bool lookingRight;
    [SerializeField] private float m_JumpForce = 300f;
    private Animator anim;
    private SpriteRenderer sr;
    public Color possessedColor;
    public CinemachineVirtualCamera vCamera;
    public Light2D light;
    public Grenade grenade;
    private bool dead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        lookingRight = GetComponent<Enemy2Script>().lookingRight;
        sr.color = possessedColor;
        vCamera.m_Follow = gameObject.transform;
        light.enabled = true;
        canMove = true;
        gameObject.layer = 13;
    }

    private void OnDisable()
    {
        light.enabled = false;
    }

    private void Update()
    {
        var position = transform.position;
        isGrounded = Physics2D.Raycast(position, Vector2.down, 1.5f, ground);
        onPlatform = Physics2D.Raycast(position, Vector2.down, 1.5f, platform);

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("Jump");
            canJump = true;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Dispossess();
        }

        if (Input.GetButtonDown("Fire1") && isGrounded && canMove)
        {
            canMove = false;
            anim.SetTrigger("Throw");
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;
        Move(horizontalMove * Time.fixedDeltaTime, canJump);
        canJump = false;
    }

    private void Move(float move, bool jump)
    {

        var velocity = rb.velocity;
        var targetVelocity = new Vector2(move * 7f, velocity.y);
        if (canMove)
            rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        else
            rb.velocity = Vector2.zero;
        
        if (move > 0 && lookingRight)
            Flip();
        else if (move < 0 && !lookingRight)
            Flip();
        if (isGrounded && jump)
        {
            isGrounded = false;
            rb.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        if (!canMove) return;
        lookingRight = !lookingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void EndThrowEvent()
    {
        canMove = true;
    }

    public void FireEvent()
    {
        
        if (isActiveAndEnabled)
        {
            
            Grenade ammo = Instantiate<Grenade>(grenade);
            ammo.gameObject.SetActive(true);
            ammo.transform.position = transform.position + transform.right * 0.2f + transform.up * 1.2f;
            ammo.lookingRight = lookingRight;
            ammo.playerPos = new Vector2(5f, -5f);
            ammo.grenadeX = 10;
            ammo.grenadeY = 5;
            ammo.throwByPlayer = true;
        }
    }
    
    private void Dispossess()
    {
        Player2DControll.Instance.gameObject.SetActive(true);
        var transform1 = Player2DControll.Instance.transform;
        transform1.parent = null;
        vCamera.m_Follow = transform1;
        sr.color = Color.Lerp(sr.color, Color.white, 1f);
        Player2DControll.Instance.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        Player2DControll.Instance.lookingRight = false;
        Die();
    }

    public void Die()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.SetBool("Dead", true);
        dead = true;
    }

    public void DieEvent()
    {
        if (isActiveAndEnabled)
            Destroy(gameObject);
    }
}
