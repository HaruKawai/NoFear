using System;
using System.Collections;
using UnityEngine;

public class Player2DControll : MonoBehaviour
{
	public static Player2DControll Instance { get; private set; }
	
    [SerializeField] private float m_JumpForce = 300f; 
    [SerializeField] private bool m_AirControl; 
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

    //General
	public bool canTakeDamage = true;
    private Animator anim;
    private float distance;
    private float verticalMove;
    private Rigidbody2D rb;
    private float horizontalMove;
    [SerializeField] private float runSpeed = 300f;
    private bool canJump;
    private bool isGrounded;
    private bool onPlatform;
    private Vector3 m_Velocity = Vector3.zero;
    private bool damaged;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask platform;
    public bool lookingRight;

    public enum PlayerMode
    {
	    Slime, Human
    }

	//Human

	private CapsuleCollider2D colliderProta;
    
    //Slime
    public PlayerMode playerMode;
    public bool slime;
	private CircleCollider2D circleSlime;
    private bool potPujar;
    public bool stickOnWall;
    public bool stickOnCealing;
    private bool parry;
    private bool inflated;
    
    //Controladores de Tiempo
    private bool chapat;
    private bool inflationControl;
    private bool canInflate = true;
    private static readonly int DesInflation = Animator.StringToHash("DesInflation");
    private static readonly int Inflation = Animator.StringToHash("Inflation");
    private IEnumerator inflationCoroutine;


    private void Awake()
    {
	    Instance = this;
	    circleSlime = GetComponent<CircleCollider2D>();
		colliderProta = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
	    playerMode = PlayerMode.Human;
	    ChangeFunction();
    }

    private void Update()
    {
	    var position = (Vector2)transform.position;
	    bool leftCollision = Physics2D.Raycast(position,  Vector2.left, 0.1f, ground);
	    bool rightCollision = Physics2D.Raycast(position,  Vector2.right, 0.1f, ground);
	    bool upCollision = Physics2D.Raycast(position, Vector2.up, 0.1f, ground);
	    isGrounded = Physics2D.Raycast(position, Vector2.down, 0.2f, ground);
	    onPlatform = Physics2D.Raycast(position, Vector2.down, 0.2f, platform);
	    //Debug.Log(upCollision);
	    //Debug.Log(leftCollision);
	    
	    anim.SetBool("IsGrounding", isGrounded);
	    anim.SetFloat("Speed", Mathf.Abs(horizontalMove));

	    transform.parent = onPlatform ? Physics2D.Raycast(position, Vector2.down, 0.2f, platform).collider.gameObject.transform : null;
	    
	    switch (playerMode)
	    {
		    case PlayerMode.Slime when (leftCollision || rightCollision):
			    anim.SetBool("slimeWall", true);
			    anim.SetBool("slimeUp", false);
			    rb.velocity = Vector2.zero;
			    rb.gravityScale = 0;
			    stickOnWall = true;
			    break;
		    case PlayerMode.Slime when upCollision:
			    anim.SetBool("slimeUp", true);
			    anim.SetBool("slimeWall", false);
			    rb.gravityScale = 0;
			    stickOnWall = false;
			    stickOnCealing = true;
			    break;
		    default:
			    anim.SetBool("slimeUp", false);
			    anim.SetBool("slimeWall", false);
			    rb.gravityScale = 1;
			    stickOnWall = false;
			    stickOnCealing = false;
			    break;
	    }


	    horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
	    verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;
        
        if (Input.GetButtonDown("Change") && !inflated)
	        ChangeFunction();

        
        if (playerMode == PlayerMode.Slime)
        {
	        if (Input.GetButtonDown("Bubble") && canInflate)
	        {
		        inflationCoroutine = InflationCoroutine();
		        StartCoroutine(inflationCoroutine);
	        }

	        if (Input.GetButtonUp("Bubble") && inflated)
		        StartCoroutine(DesInflationCoroutine());
	        
        }

        if (Input.GetButtonDown("Jump") && playerMode != PlayerMode.Slime)
        {
	        anim.SetTrigger("Jump");
	        canJump = true;
        }

        if(damaged) {
            //animator.SetBool("Damaged", true);
            damaged = false;
        }
    }

    private void FixedUpdate()
    {
	    Move(horizontalMove * Time.fixedDeltaTime, canJump, verticalMove * Time.fixedDeltaTime);
	    canJump = false;
    }


    //Change form
    private void ChangeFunction()
    {
        if(gameObject.GetComponent<Enemy>() == null)
        {
	        if(playerMode == PlayerMode.Slime)
			{
				playerMode = PlayerMode.Human;
				circleSlime.enabled = false;
				colliderProta.enabled = true;
			    anim.SetBool("Slime", false);
			}
	        else if (playerMode == PlayerMode.Human)
			{
				playerMode = PlayerMode.Slime;
			    anim.SetBool("Slime", true);
				circleSlime.enabled = true;
				colliderProta.enabled = false;
			}
		        
        } else {
            var position = transform.position;
            //position.x += 1.0f;
            //position.y += 1.2f;
            GetComponent<Collider2D>().enabled = false;
            //Instantiate(slimePrefab, position, Quaternion.identity);
        }
    }

    //Inflate
    private IEnumerator InflationCoroutine()
    {
	    parry = true;
	    inflated = true;
	    anim.SetTrigger(Inflation);
	    
	    yield return new WaitForSeconds(1f);
	    
	    parry = false;
	    
	    yield return new WaitForSeconds(4f);
	    
	    inflated = false;
	    canInflate = false;
	    anim.SetTrigger(DesInflation);

	    yield return new WaitForSeconds(2f);

	    canInflate = true;
    }
    
    //DesInflate
    private IEnumerator DesInflationCoroutine()
    {
	    StopCoroutine(inflationCoroutine);
	    anim.SetTrigger(DesInflation);
	    parry = false;
	    inflated = false; 
	    canInflate = false;

	    yield return new WaitForSeconds(2f);
	    
	    canInflate = true;
    }

    private  void Move(float move, bool jump, float move2)
    {
	    if (isGrounded || m_AirControl)
	    { 
		    var velocity = rb.velocity;
		    var targetVelocity = Vector2.zero;
		    if(playerMode == PlayerMode.Human) 
			    targetVelocity = new Vector2(move * 7f, velocity.y);
		    else if (playerMode == PlayerMode.Slime)
			    targetVelocity = stickOnWall ? new Vector2(move * 7f, move2 * 7f) : new Vector2(move * 3f, velocity.y);
		    rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		    if (move > 0 && lookingRight)
			    Flip();
		    else if (move < 0 && !lookingRight)
			    Flip();
	    }
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

	public IEnumerator TakeDamageCoroutine() 
	{
		canTakeDamage = false;
	    yield return new WaitForSeconds(1.5f);
		canTakeDamage = true;
	}
    
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.contacts[0].collider.gameObject.layer == 12 && playerMode == PlayerMode.Slime &&
		    other.gameObject.GetComponent<EnemyScript>().canBePossesed)
		{
			other.gameObject.GetComponent<EnemyScript>().canBePossesed = false;
			other.gameObject.GetComponent<PossessedEnemy>().enabled = true;
			other.gameObject.GetComponent<EnemyScript>().enabled = false;
			transform.parent = other.transform;
			gameObject.SetActive(false);
		}
	}
}