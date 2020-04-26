using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float force;
    public Vector2 playerPos;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animation anim;
    public bool lookingRight;
    public float grenadeX;
    public float grenadeY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animation>();
        
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
       
        Debug.Log(playerPos);
        rb.AddForce(direction.normalized * force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player2DControll.Instance.TakeDamage();
            //Animation.SetTrigger("Explode");
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            //Animation.SetTrigger("Explode");
            Destroy(gameObject);
        }
    }

}
