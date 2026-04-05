using Unity.AppUI.MVVM;
using UnityEngine;

public class SlowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifetime = 3f;
    public float slowPercent = 0.5f; // 50% slow
    public float slowDuration = 2f;  // lasts 2 seconds

    private Transform target;

    public void SetTarget(Transform t)
    {
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
        // Check if the projectile hit an enemy and apply damage + slow effect
        if (other.CompareTag("Enemy"))
        {   
            EnemyManager enemy = other.GetComponent<EnemyManager>();
            EnemyMovement movement = other.GetComponent<EnemyMovement>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                movement.ApplySlow(slowPercent, slowDuration);
                Destroy(gameObject);
            }
        }
    }
}
