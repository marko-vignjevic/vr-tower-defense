using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public int maxHealth = 100;
    public int coins = 10;
    public int damage = 1;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Reduce the enemy's health by the specified damage amount
        // and checks if the enemy's health has reached 0
        currentHealth -= damage;
        
        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    void Die()
    {
        // Add coins to the player's total and destroy the enemy game object
        PlayerManager.Instance.AddCoins(coins);
        Destroy(gameObject);
    }
}
