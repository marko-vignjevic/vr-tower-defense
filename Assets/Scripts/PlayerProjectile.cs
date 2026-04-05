using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage = 25;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

     void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit an enemy and apply damage
        if (other.CompareTag("Enemy"))
        {   
            EnemyManager enemy = other.GetComponent<EnemyManager>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
