using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : MonoBehaviour
{
    private GameObject player;
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player2DControll>().canTakeDamage == true)
        {
            player = other.gameObject;
            ScoreManager.instance.Damaged();
	        StartCoroutine(player.GetComponent<Player2DControll>().TakeDamageCoroutine());

            Vector3 direction = player.transform.position - transform.position;
            knockBack(player,direction,0.3f, 0.1f);
        }
    }
 
 
    private void  knockBack(GameObject target, Vector3 direction, float length, float overTime)
    {
        direction = direction.normalized;
        StartCoroutine(knockBackCoroutine(target, direction, length, overTime));
    }
 
    IEnumerator knockBackCoroutine(GameObject target, Vector3 direction, float length, float overTime)
    {
        float timeleft = overTime;
        while (timeleft>0)
        {
 
           if(timeleft>Time.deltaTime)
            target.transform.Translate(direction*Time.deltaTime/overTime*length);
           else
                target.transform.Translate(direction * timeleft / overTime * length);
            timeleft -= Time.deltaTime;
 
            yield return null;
        }
       
    }
}
