using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI textLife;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }

    public void ChangeUiHealth()
    {
        textLife.text = "" + PlayerStats.Instance.CurrentHealth;
    }
}
