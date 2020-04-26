using UnityEngine;

public class Pinchos : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Player2DControll.Instance.canTakeDamage)
        { 
            PlayerStats.Instance.Damage(); 
            Player2DControll.Instance.TakeDamage();
            Player2DControll.Instance.KnockBack();
        }
    }
}