using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    void Awake()
    {
        GetComponentInChildren<Canvas>().enabled = false;
    }
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInChildren<Canvas>().enabled = true;
            GetComponent<Animator>().SetBool("Talking", true);
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInChildren<Canvas>().enabled = false;
            GetComponent<Animator>().SetBool("Talking", false);
        }
    }

}
