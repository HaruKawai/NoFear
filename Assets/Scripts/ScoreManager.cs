using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; set; }
    public TextMeshProUGUI textLife;
    private int scoreLife = 5;
    
    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }
    
    public void Damaged()
    {
        scoreLife--;
        textLife.text = "" + scoreLife;
    }
}
