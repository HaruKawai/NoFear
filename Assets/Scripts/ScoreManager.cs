using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI textLife;
    private int scoreLife = 5;
    
    private void Awake() 
    {
        if (instance == null)
            instance = this;
    }
    
    public void Damaged()
    {
        scoreLife--;
        textLife.text = "" + scoreLife;
    }
}
