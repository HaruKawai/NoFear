using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paredAtrac : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private Player2DControll controll;
    public float speed = 20f;
    public bool rotar;
    struct Wall
    {
        public float left, right, up, down;
    }
    Wall stuckOn;
    public enum ItemType{ Dalt=0, Baix=1, Esquerra = 2, Dreta = 3}
    public ItemType Type;
    private float value = 2f;

    // Update is called once per frame
    void FixedUpdate()
    {
        
        body = GameObject.FindWithTag("Player").GetComponentInChildren<Rigidbody2D>();
        if (rotar){
            body.MoveRotation(body.rotation + speed * Time.fixedDeltaTime);
            }
        //Debug.Log(body.name);
        Debug.Log(stuckOn.left + stuckOn.right + " la y"+ stuckOn.down + stuckOn.up);
        body.AddForce(new Vector2(stuckOn.left + stuckOn.right, stuckOn.down + stuckOn.up), ForceMode2D.Impulse);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
		
		if ( collision.gameObject.transform.parent.CompareTag("Player") )
		{
            player = collision.gameObject;
            controll = player.GetComponent<Player2DControll>();
            body = player.GetComponentInChildren<Rigidbody2D>();
            if (controll.slime)
            {
                    switch ((int)Type)
                {
                    case 0:
                        stuckOn.up = value;
                        break;

                    case 1:
                        //stuckOn.down = -value;
                        break;

                    case 2:
                        controll.stickOnWall = true;
                        stuckOn.left = -value;
                        rotar = true;
                        
                        body.MoveRotation(body.rotation + speed * Time.deltaTime);
                        break;

                    case 3:
                        controll.stickOnWall = true;
                        stuckOn.right = value;
                        rotar = true;
                        break;
                }
            }
		}
		
            
    }
    public void soltar()
    {
        stuckOn.down = 0f;
        stuckOn.up = 0f;
        stuckOn.right = 0f;
        stuckOn.left = 0f;
        Debug.Log(stuckOn.down + stuckOn.up +stuckOn.right + stuckOn.left);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("ix");
        if((int)Type == 2 || (int)Type == 3) collision.gameObject.GetComponent<Player2DControll>().stickOnWall = false;
        soltar();
        
    }
}
