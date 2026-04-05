using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20.0f;
    public int damage = 10;
    public float lifetime = 2.0f;

    private Transform target;


    public void SetTarget(Transform t)
    {
        // Set the target for the projectile to follow
        target = t;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move towards the target if it exists
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.position + Vector3.up * 1.0f;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.LookAt(target);
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
