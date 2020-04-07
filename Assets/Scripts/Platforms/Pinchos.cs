using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : MonoBehaviour
{
    private GameObject player;
    public float distancia;
    public float tiempo;
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Player2DControll.Instance.canTakeDamage)
        {
            PlayerStats.Instance.Damage();
	        Player2DControll.Instance.TakeDamage();
            var direction = Player2DControll.Instance.transform.position - transform.position;
            KnockBack(Player2DControll.Instance.gameObject, direction, distancia,  tiempo);
        }
    }
 
 
    private void  KnockBack(GameObject target, Vector3 direction, float length, float overTime)
    {
        direction = direction.normalized;
        StartCoroutine(KnockBackCoroutine(target, direction, length, overTime));
    }
 
    private IEnumerator KnockBackCoroutine(GameObject target, Vector3 direction, float length, float overTime)
    {
        float timeLeft = overTime;
        while (timeLeft>0)
        {
 
           if(timeLeft>Time.deltaTime)
            target.transform.Translate(direction*Time.deltaTime/overTime*length);
           else
                target.transform.Translate(direction * timeLeft / overTime * length);
           timeLeft -= Time.deltaTime;
 
           yield return null;
        }
       
    }
}
