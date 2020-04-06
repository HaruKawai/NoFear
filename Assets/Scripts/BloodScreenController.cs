using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodScreenController : MonoBehaviour
{
    public Image Screen;
    public Sprite BloodSprite1;
    public Sprite BloodSprite2;
    public Sprite BloodSprite3;
    public Sprite BloodSprite4;
    public Sprite transp;
    public bool prova;


void Update()
{
    if(prova)
    {
        StartCoroutine(DamagedCoroutine());
        prova = false;
    }
}
    public IEnumerator DamagedCoroutine() 
    {
        Debug.Log("sip");
        Screen.sprite = BloodSprite4;
        yield return new WaitForSeconds(0.5f);
        Screen.sprite = BloodSprite3;
        yield return new WaitForSeconds(0.5f);
        Screen.sprite = BloodSprite2;
        yield return new WaitForSeconds(0.5f);
        Screen.sprite = BloodSprite1;
        yield return new WaitForSeconds(0.5f);
        Screen.sprite = transp;
    }
}
