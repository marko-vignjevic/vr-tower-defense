using UnityEngine;
using TMPro;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Currency")]
    public int startingCoins = 500;
    private int currentCoins;

    public TextMeshProUGUI healthText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentCoins = startingCoins;
    }

    void Update()
    {
        // Update the health display
        healthText.text = "HP: " + currentHealth;
    }

    public void TakeDamage(int amount)
    {
        // Reduce the player's health by the specified amount and ends the game if health reaches 0
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameOver();
        }
    }


    public bool SpendCoins(int amount)
    {
        // Returns true or false depending on if the player has enough coins to spend
        if (currentCoins < amount)
        {
            return false;
        }
        currentCoins -= amount;
        return true;
    }

    public void AddCoins(int amount)
    {
        // Add coins to the player's total
        currentCoins += amount;
    }

    void GameOver()
    {
        // Stops the game
        Time.timeScale = 0f;
    }

    public int GetCoins()
    {
        return currentCoins;
    }
}
