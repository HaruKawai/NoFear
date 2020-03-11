using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodItem : MonoBehaviour
{
    public enum ItemType{ Cherry=0, Gem=1}
    public ItemType Type;
    public int coinValue = 1;
    public GameObject prota;
    private void Start() {
        GetComponent<Animator>().SetInteger("Type",(int)Type);
    }

    public void PickItem() {
        GetComponent<Animator>().SetTrigger("Pick");
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) PickItem();
        if((int)Type == 0) {
            //ScoreManager.instance.ChangeFood(coinValue);
            collision.gameObject.GetComponent<Prota>().TakeFood(10);
            ScoreManager.instance.PickFood();
        }else if ((int)Type == 1) {
            ScoreManager.instance.ChangeDiam(coinValue);
            ScoreManager.instance.PickDiam();
        }
    }
}
