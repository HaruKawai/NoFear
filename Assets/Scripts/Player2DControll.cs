using System;
using System.Collections;
using UnityEngine;

public class Player2DControll : MonoBehaviour
{
	public static Player2DControll Instance { get; private set; }
	
    [SerializeField] private float m_JumpForce = 300f; 
    [SerializeField] private bool m_AirControl; 
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private GameObject slimePrefab;
    private paredAtrac[] scriptPared;
    
    //General
    private Animator anim;
    private float distance;
    private float verticalMove;
    private Rigidbody2D rb;
    private float horizontalMove;
    private float runSpeed = 50f;
    private bool canJump;
    private bool isGrounded;
	private bool m_FacingRight = true;
	private Vector3 m_Velocity = Vector3.zero;
    private bool damaged;
    [SerializeField] private LayerMask ground;

    public enum PlayerMode
    {
	    Slime, Human
    }
    
    //Slime
    private PlayerMode playerMode;
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


    private void Awake()
    {
	    Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scriptPared = FindObjectsOfType<paredAtrac>();
    }

    private void Start()
    {
	    playerMode = PlayerMode.Human;
    }

    private void Update()
    {
	    var position = transform.position;
	    bool leftCollision = Physics2D.Raycast(position,  Vector2.left, 0.25f, ground);
	    bool rightCollision = Physics2D.Raycast(position,  Vector2.right, 0.25f, ground);
	    bool upCollision = Physics2D.Raycast(position, Vector2.up, 0.25f, ground);

	    if (playerMode == PlayerMode.Slime && (leftCollision || rightCollision))
	    {
		    rb.velocity = Vector2.zero;
		    rb.gravityScale = 0;
		    stickOnWall = true;
	    }
	    else if (playerMode == PlayerMode.Slime && upCollision)
	    {
		    rb.gravityScale = 0;
		    stickOnWall = false;
		    stickOnCealing = true;
	    }
	    else
	    {
		    rb.gravityScale = 1;
		    stickOnWall = false;
		    stickOnCealing = false;
	    }


	    horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
	    verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;
        
        if (Input.GetButtonDown("Change"))
	        ChangeFunction();

        if (playerMode == PlayerMode.Slime)
        {
	        if (Input.GetButtonDown("Bubble") && canInflate)
		        StartCoroutine(InflationCoroutine());

	        if (Input.GetButtonUp("Bubble") && inflated) 
		        StartCoroutine(DesInflationCoroutine());
        }
        else  //Interact Actions
        {
            //Interact
        }
        
        //Jump/Unstick
        if (Input.GetButtonDown("Jump"))
        {
            canJump = true;
            //animator.SetBool("IsJumping", true);
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
			   playerMode = PlayerMode.Human;
	        else if (playerMode == PlayerMode.Human) 
		        playerMode = PlayerMode.Slime;
        } else {
            var position = transform.position;
            position.x += 1.0f;
            position.y += 1.2f;
            GetComponent<Collider2D>().enabled = false;
            Instantiate(slimePrefab, position, Quaternion.identity);
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
	    StopCoroutine(InflationCoroutine());
	    anim.SetTrigger(DesInflation);
	    parry = false;
	    inflated = false; 
	    canInflate = false;

	    yield return new WaitForSeconds(2f);
	    
	    canInflate = true;
    }
    

    private void OnCollisionEnter2D(Collision2D coll)
    {
	    if (coll.gameObject.layer == 8)
		    isGrounded = true;
		else if (coll.gameObject.layer == 9)
		{
			isGrounded = true;
			transform.parent = coll.gameObject.transform;
		}
		    
	}
	private void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.gameObject.layer == 9)
			transform.parent = null;
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
		    if (move > 0 && !m_FacingRight) 
			    Flip();
		    else if (move < 0 && m_FacingRight)
			    Flip();
	    }
	    if (isGrounded && jump)
	    {
		    isGrounded = false;
		    rb.AddForce(new Vector2(0f, m_JumpForce));
	    }
	    
	    if (jump && playerMode == PlayerMode.Slime)
	    {
		    foreach (var script in scriptPared)
		    {
			    script.soltar();  
		    }
	    }
    }
    
	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		transform.Rotate(0f,180f,0);
	}
 
    
}