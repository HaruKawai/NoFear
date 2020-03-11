using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DControll : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 300f;                          // Amount of force added when the player jumps.
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    //[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private GameObject slimePrefab;
    private paredAtrac[] scriptPared;


    //General
    public Animator animator;
    public float distance;
    float verticalMove = 0f;
    private Rigidbody2D rb;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    public bool isGrounded;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    public bool damaged;


    //Slime
    public bool slime = false;
    public bool potPujar;
    public bool stickOnWall = false;
    public bool parry;
    
    private bool inflated;


    //Controladores de Tiempo
    private bool timear;
    private bool chapat;
    private float parryFinish = 1.0f;
    private float Timer = 0f;
    private float Timer2 = 0f;
    private float inflationFinish = 5.0f;
    private float cdInflation;
    private float cdInflationMin = 2.0f;
    private bool inflationControl;
    private bool canInflate = true;

    
    
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        scriptPared = FindObjectsOfType<paredAtrac>();
    }

    void Update()
    {


        Debug.Log(transform.rotation);
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;

        //if(Input.GetButtonDown("Interact"))
        if (Input.GetButtonDown("Change")) ChangeFunction();


        //Inflation time, parry Controller
        if(timear)
        {
            if(Timer >= inflationFinish)
            {
                timear = false;
                cdInflation = Timer;
                inflated = false;
                canInflate = false;
                Timer2 = 0f;
                animator.SetTrigger("DesInflation");
            }else if(Timer >= parryFinish)
            {
                parry = false;
                Timer += Time.deltaTime;
            }else
            {
                Timer += Time.deltaTime;
            }
        }

        //Inflation Cooldown Controller
        if(!canInflate)
        {
            if(cdInflation < cdInflationMin) cdInflation = cdInflationMin;
            if(Timer2 >= cdInflation)
            {
                canInflate = true;
            }else{Timer2 += Time.deltaTime;}
        }


   
        if (slime)
        {
            if(canInflate)  //Inflate Actions
            {
                if (Input.GetButtonDown("Bubble"))  InflationFunction();

                if (Input.GetButtonUp("Bubble"))
                {
                    if(inflated) DesInflationFunction();
                }
            }
        }else  //Interact Actions
        {
            //interactuar
        }
        
        //Jump/Unstick
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            //animator.SetBool("IsJumping", true);
        }

        if(damaged) {
            //animator.SetBool("Damaged", true);
            damaged = false;
        }




        //Move our character
        Move(horizontalMove * Time.fixedDeltaTime, jump, verticalMove * Time.fixedDeltaTime);

        
        jump = false;
    }


    //Cambio de forma
    void ChangeFunction()
    {
        if(!gameObject.GetComponent<Enemy>())
        {
            if(slime) 
            {
                foreach (paredAtrac script in scriptPared)
                {
                    script.soltar();  
                }
            }
            slime = !slime;
        }else {
            Vector3 posicion = transform.position;
            posicion.x = posicion.x + 1.0f;
            posicion.y = posicion.y + 1.2f;
            GetComponent<Collider2D>().enabled = false;
            Instantiate(slimePrefab, posicion, Quaternion.identity);
        }
    }

    //Inflar
    void InflationFunction()
    {
        animator.SetTrigger("Inflation");
        parry = true;
        Timer = 0f;
        timear = true;
        inflated = true;
    }

    //Desinflar
    void DesInflationFunction()
    {
        animator.SetTrigger("DesInflation");
        timear = false;
        parry = false;
        cdInflation = Timer;
        inflated = false; 
        canInflate = false;
        Timer2 = 0f;
    }
 
    void OnCollisionEnter2D(Collision2D coll)
    {
         
        if (coll.gameObject.tag == "ground")
        {
             isGrounded = true;
        }
    }


     public void Move(float move, bool jump, float move2)
	{
		if(!slime){
			//only control the player if grounded or airControl is turned on
			if (isGrounded || m_AirControl)
			{

				// Move the character by finding the target velocity
				Vector2 targetVelocity = new Vector2(move * 7f, rb.velocity.y);
				// And then smoothing it out and applying it to the character
				rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}
			// If the player should jump...
			if (isGrounded && jump)
			{
				// Add a vertical force to the player.
				isGrounded = false;
				rb.AddForce(new Vector2(0f, m_JumpForce));
			}
		}
		else {
            if (isGrounded || m_AirControl)
			{
                Vector2 targetVelocity;
                if(stickOnWall)
                {
                    targetVelocity = new Vector2(move * 7f, move2 * 7f);
                }else
                {
                    // Move the character by finding the target velocity
                    targetVelocity = new Vector2(move * 3f, rb.velocity.y);
                }

				// And then smoothing it out and applying it to the character
				rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}
			// If the player should jump...
			if (jump)
			{
                foreach (paredAtrac script in scriptPared)
                {
                script.soltar();  
                }
			}
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f,180f,0);
	}
 
    
}