using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI textLife;
    int scoreLife = 5;
    
    void Start() 
    {
        if (instance == null) 
        {
            instance = this;
        }
    }


    public void Damaged() 
    {
        scoreLife = scoreLife - 1;
        textLife.text = "" + scoreLife.ToString();
    }
}
