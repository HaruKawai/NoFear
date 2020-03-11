using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI textDiam;
    public TextMeshProUGUI textFood;
    public GameObject diamImage;
    public GameObject cherryImage;
    int scoreDiam = 0;
    int scoreFood = 0;
    
    void Start() 
    {
        if (instance == null) 
        {
            instance = this;
        }
    }

    public void PickDiam() {
        diamImage.GetComponent<Animator>().SetTrigger("Diam");
    }
    public void PickFood() {
        cherryImage.GetComponent<Animator>().SetTrigger("Cherry");
    }

    public void ChangeDiam(int coinValue) 
    {
        scoreDiam = scoreDiam + coinValue;
        textDiam.text = "X" + scoreDiam.ToString();
    }

    public void ChangeFood(int foodValue)
    {
        scoreFood += foodValue;
        textFood.text = "X" + scoreFood.ToString();
    }
}
