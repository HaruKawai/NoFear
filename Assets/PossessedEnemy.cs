using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PossessedEnemy : MonoBehaviour
{
    private bool isGrounded;
    private bool onPlatform;
    public LayerMask ground;
    public LayerMask platform;
    public bool canJump;
    private float horizontalMove;
    private float verticalMove;
    public float runSpeed;
    private Rigidbody2D rb;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    private bool lookingRight;
    [SerializeField] private float m_JumpForce = 300f;
    private Animator anim;
    private SpriteRenderer sr;
    public Color possessedColor;
    public Color normalColor;
    public CinemachineVirtualCamera vCamera;
    public Light2D light;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }
 
    private void OnEnable()
    {
        lookingRight = GetComponent<EnemyScript>().lookingRight;
        sr.color = possessedColor;
        vCamera.m_Follow = gameObject.transform;
        light.enabled = true;
    }

    private void OnDisable()
    {
        Player2DControll.Instance.lookingRight = !lookingRight;
        light.enabled = false;
    }

    private void Update()
    {
        var position = transform.position;
        isGrounded = Physics2D.Raycast(position, Vector2.down, 0.2f, ground);
        onPlatform = Physics2D.Raycast(position, Vector2.down, 0.2f, platform);

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("Jump");
            canJump = true;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Player2DControll.Instance.gameObject.SetActive(true);
            Player2DControll.Instance.transform.parent = null;
            vCamera.m_Follow = Player2DControll.Instance.transform;
            sr.color = normalColor;
            enabled = false;
            GetComponent<EnemyScript>().enabled = true;
            
        }
    }

    private void FixedUpdate()
    {
        Move(horizontalMove * Time.fixedDeltaTime, canJump);
        canJump = false;
    }

    private  void Move(float move, bool jump)
    {

        var velocity = rb.velocity;
        var targetVelocity = new Vector2(move * 7f, velocity.y);
        rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
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
        lookingRight = !lookingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
