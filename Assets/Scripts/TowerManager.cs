using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [Header("Targeting")]
    public float range = 10f;
    public Transform gunPivot;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.0f;
    
    private float fireCooldown = 0.0f;
    private Transform target;

    void Update()
    {
        // Calls all the functions needed to find a target,
        // rotate towards it, and shoot while updating the cooldown
        FindTarget();

        if (target != null)
        {
            RotateToTarget();
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        // Find the enemy closest to the end of the path within range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform frontTarget = null;
        int highestWaypointIndex = -1;

        foreach (GameObject enemyObj in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemyObj.transform.position);

            if (distance > range) continue;

            EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
            if (movement == null) continue;

            if (movement.waypointIndex > highestWaypointIndex)
            {
                highestWaypointIndex = movement.waypointIndex;
                frontTarget = enemyObj.transform;
            }
        }

        target = frontTarget;
    }

    void RotateToTarget()
    {
        // Rotate the gun pivot to face the target
        Vector3 direction = target.position - gunPivot.position;
        direction.y = direction.y + 1.0f;
        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, targetRotation, Time.deltaTime * 8.0f);
    }

    void Shoot()
    {
        // Shoot a projectile towards the current target
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = bullet.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetTarget(target);
        }
    }
}
