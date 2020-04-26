using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;

public class Player2DControll : MonoBehaviour
{
	public static Player2DControll Instance { get; private set; }
	
    [SerializeField] private float m_JumpForce = 300f; 
    [SerializeField] private bool m_AirControl; 
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector2 targetVelocity;

    //General
	public Canvas elCanvas;
	public bool canTakeDamage = true;
    private Animator anim;
    private float distance;
    private float verticalMove;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float horizontalMove;
	private bool topSpeed;
    [SerializeField] private float runSpeed = 300f;
    private bool canJump;
    private bool isGrounded;
    private bool onPlatform;
    private Vector2 m_Velocity = Vector2.zero;
    private bool damaged;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask platform;
    public bool lookingRight;
    public bool knockBacked;
    public bool knockBacKStun;
    private bool knockBackStun2;

    public enum PlayerMode
    {
	    Slime, Human
    }

	//Human

	public BoxCollider2D colliderProta;
	public BoxCollider2D colliderSlime;
    
    //Slime
    public PlayerMode playerMode;
    public bool slime;
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
	    sr = GetComponentInChildren<SpriteRenderer>();
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
		if(!topSpeed && playerMode != PlayerMode.Slime) checkSpeed();


	    var position = (Vector2)transform.position + Vector2.down * 0.5f;
	    bool leftCollision = Physics2D.Raycast(position,  Vector2.left, 1f, ground);
	    bool rightCollision = Physics2D.Raycast(position,  Vector2.right, 1f, ground);
	    bool upCollision = Physics2D.Raycast(position, Vector2.up, 1.15f, ground);
	    //Problema.
	    isGrounded = Physics2D.Raycast(position + new Vector2(-0.3f, 0f) * transform.right + Vector2.up * 0.5f, Vector2.down, 2f,
	     ground);
	    onPlatform = Physics2D.Raycast(position + new Vector2(-0.3f, 0f) * transform.right + Vector2.up * 0.5f, Vector2.down, 2f, platform);
	    Debug.DrawRay(position + new Vector2(-0.3f, 0) * transform.right + Vector2.up * 0.5f, Vector2.down * 2f);
	    Debug.DrawRay(position, Vector2.up * 1.15f, Color.red);
	    Debug.DrawRay(position, Vector2.right * 1f, Color.blue);
	    Debug.DrawRay(position, Vector2.left * 1f, Color.blue);

	    if (!knockBacked)
	    {
		    anim.SetBool("IsGrounding", isGrounded);
		    anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
	    }

	    transform.parent = onPlatform ? Physics2D.Raycast(position + new Vector2(-0.3f, 0) * transform.right, Vector2.down, 2f, platform).collider.gameObject.transform : null;
	    
	    switch (playerMode)
	    {
		    case PlayerMode.Slime when (leftCollision || rightCollision) && !knockBacked:
			    anim.SetBool("slimeWall", true);
			    anim.SetBool("slimeUp", false);
			    rb.velocity = Vector2.zero;
			    rb.gravityScale = 0;
			    stickOnWall = true;
			    break;
		    case PlayerMode.Slime when upCollision && !knockBacked:
			    anim.SetBool("slimeUp", true);
			    anim.SetBool("slimeWall", false);
			    rb.gravityScale = 0;
			    stickOnWall = false;
			    stickOnCealing = true;
			    break;
		    default:
			    anim.SetBool("slimeUp", false);
			    anim.SetBool("slimeWall", false);
			    rb.gravityScale = 5;
			    stickOnWall = false;
			    stickOnCealing = false;
			    break;
	    }


	    horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
	    verticalMove = Input.GetAxisRaw("Vertical") * runSpeed * 2;
        
        if (Input.GetButtonDown("Change") && !inflated)
	        ChangeFunction();

        
        if (playerMode == PlayerMode.Slime)
        {
			topSpeed = false;

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

	//Comprobar velocidad en eje y
	private void checkSpeed() 
	{
		if(rb.velocity.y < -40) topSpeed = true;
	}

    private void FixedUpdate()
    {
	    Move(horizontalMove, canJump, verticalMove);
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
				colliderSlime.enabled = false;
				colliderProta.enabled = true;
			    anim.SetBool("Slime", false);
			}
	        else if (playerMode == PlayerMode.Human)
			{
				playerMode = PlayerMode.Slime;
			    anim.SetBool("Slime", true);
				colliderSlime.enabled = true;
				colliderProta.enabled = false;
			}
        }
        else
	        GetComponent<Collider2D>().enabled = false;
      
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

    private void Move(float move, bool jump, float move2)
    {
	    if (isGrounded || m_AirControl)
	    {
		    var velocity = rb.velocity;
		    if (!knockBacked)
		    {
			    targetVelocity = Vector2.zero;
			    if (playerMode == PlayerMode.Human)
				    targetVelocity = new Vector2(move * 7f * Time.fixedDeltaTime, velocity.y);
			    else if (playerMode == PlayerMode.Slime)
				    if (stickOnWall)
					    targetVelocity = new Vector2(move * 7f, move2 * 7f) * Time.fixedDeltaTime;
				    else
				    {
					    targetVelocity = Vector2.zero;
					    targetVelocity = isGrounded ? new Vector2(move * 3f * Time.fixedDeltaTime, velocity.y) : new Vector2(move * 7f * Time.fixedDeltaTime, velocity.y);
				    }
		    }
		    else
			    targetVelocity = velocity;
		    

		    rb.velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		    
		    if (move > 0 && lookingRight)
			    Flip();
		    else if (move < 0 && !lookingRight)
			    Flip();
	    }
	    
	    if (isGrounded && jump)
	    {
		    isGrounded = false;
		    rb.AddForce(Vector2.up * m_JumpForce);
	    }
    }

    public void KnockBack()
    {
	    rb.AddForce(transform.up * 700f);
	    rb.AddForce(-transform.right * 500f);
	    knockBacked = true;
	    StartCoroutine(KnockBackMovementDelay());
    }
    
    public IEnumerator KnockBackMovementDelay()
    {
	    yield return new WaitForSeconds(1.5f);
	    knockBacked = false;
    }
    
    private void Flip()
    {
	    lookingRight = !lookingRight;
	    transform.Rotate(0f, 180f, 0f);
    }

    public void TakeDamage()
    {
	    Debug.Log("Hit");
	    if(playerMode == PlayerMode.Human)
		  ChangeFunction();
	    StartCoroutine(TakeDamageCoroutine());
		elCanvas.GetComponent<BloodScreenController>().TakeDamageScreen();
    }
    
	private IEnumerator TakeDamageCoroutine() 
	{
		canTakeDamage = false;
		anim.SetTrigger("Damaged");
		GetComponent<CinemachineImpulseSource>().GenerateImpulse();
		for (var i = 0; i < 5; i++)
		{
			sr.color = Color.Lerp(sr.color, Color.black, 1f);
			yield return new WaitForSeconds(0.15f);
			sr.color = Color.Lerp(sr.color, Color.white, 1f);
			yield return new WaitForSeconds(0.15f);
		}

		canTakeDamage = true;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.contacts[0].collider.gameObject.layer == 12 && playerMode == PlayerMode.Slime /*&&
		    other.gameObject.GetComponent<EnemyScript>().canBePossesed*/)
		{
            Debug.Log(other.gameObject.name);
            if(other.gameObject.name == "Enemy1")
                if (other.gameObject.GetComponent<EnemyScript>().canBePossesed)
                {
                    other.gameObject.GetComponent<EnemyScript>().canBePossesed = false;
                    other.gameObject.GetComponent<PossessedEnemy>().enabled = true;
                    other.gameObject.GetComponent<EnemyScript>().enabled = false;
                    gameObject.SetActive(false);
                }
            if(other.gameObject.name == "Enemy2")
                if (other.gameObject.GetComponent<Enemy2Script>().canBePossesed)
                {
                    other.gameObject.GetComponent<Enemy2Script>().canBePossesed = false;
                    other.gameObject.GetComponent<PossesedEnemy2>().enabled = true;
                    other.gameObject.GetComponent<Enemy2Script>().enabled = false;
                    gameObject.SetActive(false);
                }
        }
		if (topSpeed && other.collider.gameObject.layer == 8) TakeDamage();
	}
}