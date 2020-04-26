using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        Instance = this;
    }

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value < 0 ? 0 : value;
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            /*
            if (value > maxHealth)
                currentHealth = maxHealth;
            else if (currentHealth < 0)
                currentHealth = 0;
            else
            */
                currentHealth = value;
        }
    }

    private void Start()
    {
        MaxHealth = 0;
        CurrentHealth = MaxHealth;
    }

    public void Damage()
    {
        Debug.Log(CurrentHealth);
        CurrentHealth++;
        ScoreManager.Instance.ChangeUiHealth();
    }

}